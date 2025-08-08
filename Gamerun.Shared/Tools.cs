using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Gamerun.Shared;

public static class Tools
{
    public static readonly uint VLEMaxSize = 268435455;

    // We need this for Gamescope, compositors, power management, fan control and notification daemons
    public static string GetCommand(string command)
    {
        if (File.Exists(command)) return command; // Return same command if the path is already correct.
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


    public static bool IsGnomeSession()
    {
        return Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP")?.Contains("GNOME") == true;
    }

    public static bool IsKDESession()
    {
        return Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP")?.Contains("KDE") == true;
    }

    public static bool IsRunning(string processName)
    {
        return !string.IsNullOrEmpty(Run(GetCommand("pgrep"), $"-x {processName}"));
    }

    public static GpuInfo[] GetAllGpus()
    {
        var gpus = new List<GpuInfo>();
        foreach (var devPath in Directory.GetDirectories("/sys/bus/pci/devices"))
        {
            var classPath = Path.Combine(devPath, "class");
            if (!File.Exists(classPath)) continue;

            var classCode = File.ReadAllText(classPath).Trim();
            if (!classCode.StartsWith("0x03")) continue; // GPU class

            var vendor = File.ReadAllText(Path.Combine(devPath, "vendor")).Trim(); // e.g., 0x10de
            var device = File.ReadAllText(Path.Combine(devPath, "device")).Trim(); // e.g., 0x1f82
            var pciId = Path.GetFileName(devPath); // PCI ID like 0000:01:00.0

            var driver = "";
            var driverLink = Path.Combine(devPath, "driver");
            if (Directory.Exists(driverLink))
                driver = Path.GetFileName(Path.GetFullPath(driverLink)); // e.g., nvidia, i915, amdgpu

            // Attempt to find matching DRM card and read first mode
            var defaultMode = TryGetDrmDefaultMode(pciId);

            gpus.Add(new GpuInfo(devPath, pciId, driver, vendor, device, defaultMode.mode, defaultMode.drm));
        }

        return gpus.OrderByDescending(gpu => GetGpuPriority(gpu.Driver)).ToArray();
        ;
    }

    private static int GetGpuPriority(string driver)
    {
        return driver switch
        {
            var d when d.Contains("nvidia", StringComparison.OrdinalIgnoreCase) => 3,
            var d when d.Contains("nouveau", StringComparison.OrdinalIgnoreCase) => 3,
            var d when d.Contains("nova", StringComparison.OrdinalIgnoreCase) => 3,
            var d when d.Contains("amdgpu", StringComparison.OrdinalIgnoreCase) => 2,
            var d when d.Contains("radeon", StringComparison.OrdinalIgnoreCase) => 2,
            var d when d.Contains("i915", StringComparison.OrdinalIgnoreCase) => 1,
            var d when d.Contains("intel", StringComparison.OrdinalIgnoreCase) => 1,
            _ => 0
        };
    }


    private static (string mode, string drm) TryGetDrmDefaultMode(string pciId)
    {
        foreach (var cardPath in Directory.GetDirectories("/sys/class/drm/", "card*"))
        {
            var deviceLink = Path.Combine(cardPath, "device");
            if (!Directory.Exists(deviceLink)) continue;

            var resolvedPci = Path.GetFileName(Path.GetFullPath(deviceLink));
            if (!resolvedPci.Equals(pciId, StringComparison.OrdinalIgnoreCase)) continue;

            var statusPath = Path.Combine(cardPath, "status");
            if (File.Exists(statusPath))
            {
                var status = File.ReadAllText(statusPath).Trim();
                if (status != "connected") continue;
            }

            var modesPath = Path.Combine(cardPath, "modes");
            if (!File.Exists(modesPath)) continue;
            var modes = File.ReadAllLines(modesPath);
            if (modes.Length > 0)
                return (modes[0], cardPath); // first mode is typically current
        }

        return (string.Empty, string.Empty);
    }

    private static string Run(string command, string args)
    {
        var psi = new ProcessStartInfo(command, args)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        using var proc = Process.Start(psi);
        if (proc == null) return string.Empty;
        var output = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();
        return output.Trim();
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

    public static byte[] PackBoolsToBytes(bool[] bools)
    {
        var byteCount = (bools.Length + 7) / 8; // Round up
        var bytes = new byte[byteCount];

        for (var i = 0; i < bools.Length; i++)
            if (bools[i])
                bytes[i / 8] |= (byte)(1 << (i % 8));

        return bytes;
    }

    public static bool[] UnpackBytesToBools(byte[] bytes, int originalLength)
    {
        var bools = new bool[originalLength];

        for (var i = 0; i < originalLength; i++) bools[i] = (bytes[i / 8] & (1 << (i % 8))) != 0;

        return bools;
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
                    core.Type = capacity switch
                    {
                        >= 1024 => ProcessorCoreType.HighPower,
                        <= 512 => ProcessorCoreType.LowPower,
                        _ => ProcessorCoreType.Normal
                    };
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