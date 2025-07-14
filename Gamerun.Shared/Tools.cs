using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Gamerun.Shared
{
    public static class Tools
    {
        public static readonly uint VLEMaxSize = 268435455;

        // TODO: Hopefully we won't be needing this in the future since I set libstrangle and MangoHUD stuff with environment variables and use native solutions for nvidia-prime/switcherooctl and gamemode instead.
        [Obsolete("Use this as less as possible. Switch to Environment Variables if possible.")]
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