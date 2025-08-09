using System.Runtime.InteropServices;

namespace Gamerun;
public static class CPUSetting
{
    private const int CPU_SETSIZE = 1024;
    private const int NCPUBITS = 64;
    private const int CPU_MASK_SIZE = CPU_SETSIZE / 8; // 128 bytes

    [DllImport("libc", SetLastError = true)]
    private static extern int sched_setaffinity(int pid, IntPtr cpusetsize, IntPtr mask);

    public static bool PinCores(int pid, bool[] cpuMask)
    {
        if (cpuMask.Length > CPU_SETSIZE)
            throw new ArgumentException($"CPU mask too large. Max supported: {CPU_SETSIZE}");

        var mask = new byte[CPU_MASK_SIZE];

        for (var cpu = 0; cpu < cpuMask.Length; cpu++)
        {
            if (!cpuMask[cpu]) continue;

            var byteIndex = cpu / 8;
            var bitOffset = cpu % 8;
            mask[byteIndex] |= (byte)(1 << bitOffset);
        }

        var handle = GCHandle.Alloc(mask, GCHandleType.Pinned);
        try
        {
            var maskPtr = handle.AddrOfPinnedObject();
            IntPtr size = mask.Length;

            var result = sched_setaffinity(pid, size, maskPtr);
            if (result == 0) return true;
            var errno = Marshal.GetLastWin32Error();
            Console.Error.WriteLine($"sched_setaffinity failed (errno={errno})");
            return false;

        }
        finally
        {
            handle.Free();
        }
    }
    public static void ParkCores(bool[] parkMask, bool enabled)
    {
        for (var cpu = 1; cpu < parkMask.Length; cpu++) // Skip CPU0
        {
            var path = $"/sys/devices/system/cpu/cpu{cpu}/online";
            if (!File.Exists(path))
                continue;

            try
            {
                var targetState = (enabled && parkMask[cpu]) ? "0" : "1";
                File.WriteAllText(path, targetState);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to write to cpu{cpu}: {ex.Message}");
            }
        }
    }
    private static void SetCPUScalingGovernor(string mode)
    {
        var systemCPUpath = "/sys/devices/system/cpu/";
        var systemCPUs = Directory.GetDirectories(systemCPUpath, "cpu*", SearchOption.TopDirectoryOnly);
        foreach (var systemCPU in systemCPUs)
        {
            var governorSetting = Path.Combine(systemCPU, "cpufreq", "scaling_governor");
            if (!File.Exists(governorSetting)) continue;
            using var fileStream =
                new FileStream(governorSetting, FileMode.Truncate, FileAccess.Read, FileShare.ReadWrite);
            using var writer = new StreamWriter(fileStream);
            writer.Write(mode);
        }
    }
}