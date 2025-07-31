using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gamerun.Shared
{
    public static class Tools
    {
        public static readonly uint VLEMaxSize = 268435455;

        // We need this for Gamescope, compositors, power management, fan control and notification daemons
        public static string GetCommand(string command)
        {
            var pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrWhiteSpace(pathEnv))
                throw new Exception(
                    "The PATH environment variable is not set. DO NOT TOY WITH THAT ENVIRONMENT VARIABLE! FIX IT!");
            var paths = pathEnv.Split(':');
            if (paths == null || paths.Length <= 0)
                throw new Exception(
                    "The PATH environment variable is not in the standard format. DO NOT TOY WITH THAT ENVIRONMENT VARIABLE! FIX IT!");
            foreach (var path in paths)
            {
                var commandPath = Path.Combine(path, command);
                if (File.Exists(commandPath)) return commandPath;
            }

            throw new FileNotFoundException("Could not find command " + command);
        }

        public static bool IsBitSet(int value, int bitPosition)
        {
            if (bitPosition < 0 || bitPosition > (sizeof(int) - 1) * 8)
                return false;
            var bitmask = 1 << bitPosition;
            return (value & bitmask) != 0;
        }

        public static uint DecodeVarUInt(Stream stream)
        {
            uint value = 0;
            var shift = 0;
            while (true)
            {
                var b = (byte)stream.ReadByte();
                value |= (uint)(b & 0x7F) << shift;
                shift += 7;

                if ((b & 0x80) == 0) break;
            }

            return value;
        }

        public static bool IsIntelCpu()
        {
            try
            {
                var cpuInfo = File.ReadAllText("/proc/cpuinfo");
                return cpuInfo.Contains("GenuineIntel");
            }
            catch
            {
                return false; // Assume not Intel if cannot detect
            }
        }

        public static ProcessorCore[] DetectCpuTopology()
        {
            var cores = new List<ProcessorCore>();
            var cpus = Directory.GetDirectories("/sys/devices/system/cpu", "cpu[0-9]*");

            foreach (var cpuPath in cpus)
            {
                var cpuIdStr = Path.GetFileName(cpuPath).Replace("cpu", "");
                if (!int.TryParse(cpuIdStr, out var cpuId))
                    continue;

                var core = new ProcessorCore { Id = cpuId, Type = ProcessorCoreType.Normal };

                var eppPath = Path.Combine(cpuPath, "cpufreq/energy_performance_preference");
                if (File.Exists(eppPath))
                {
                    using var eppStream = new FileStream(eppPath, FileMode.Open, FileAccess.Read);
                    using var eppReader = new StreamReader(eppStream);
                    var epp = eppReader.ReadToEnd();
                    switch (epp.ToLower())
                    {
                        case "power":
                        case "balance-power":
                            core.Type = ProcessorCoreType.LowPower;
                            break;

                        case "performance":
                            core.Type = ProcessorCoreType.HighPower;
                            break;
                        default:
                            core.Type = ProcessorCoreType.Normal;
                            break;
                    }
                }

                var capPath = Path.Combine(cpuPath, "cpu_capacity");
                if (File.Exists(capPath))
                {
                    using var capStream = new FileStream(capPath, FileMode.Open, FileAccess.Read);
                    using var capReader = new StreamReader(capStream);
                    var capacityString = capReader.ReadToEnd();
                    if (int.TryParse(capacityString, out var capacity))
                    {
                        if (capacity >= 1024)
                            core.Type = ProcessorCoreType.HighPower;
                        else if (capacity <= 512)
                            core.Type = ProcessorCoreType.LowPower;
                        else
                            core.Type = ProcessorCoreType.Normal;
                    }
                }

                cores.Add(core);
            }

            return cores.OrderBy(c => c.Id).ToArray();
        }


        public static void WriteVarUInt(Stream stream, uint value)
        {
            do
            {
                var b = (byte)(value & 0x7F);
                value >>= 7;
                b |= (byte)(value > 0 ? 0x80 : 0);
                stream.WriteByte(b);
            } while (value > 0);
        }

        public static string GenerateRandomText(uint length = 17)
        {
            length = length switch
            {
                0 => 17,
                _ => length
            };
            if (length >= int.MaxValue) length = 17;
            var builder = new StringBuilder();
            Enumerable
                .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, (int)(length - 1)).Select(e => e.ToString()))
                .OrderBy(_ => Guid.NewGuid())
                .Take((int)length)
                .ToList().ForEach(e => builder.Append(e));
            return builder.ToString();
        }
    }
}