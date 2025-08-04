using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Gamerun.Shared;

namespace Gamerun;

public static class Daemon
{
    private const int SCHED_RR = 2;
    [DllImport("libc")]
    private static extern uint getuid();
    private static string[] SocketPaths =>
        ["/run/gamerun.sock", $"/run/user/{getuid()}/gamerun.sock", "/tmp/gamerun.sock"];

    public static string GetRunningSocket()
    {
        foreach (var path in SocketPaths)
        {
            if (!File.Exists(path)) continue;
            try
            {
                var endPoint = new UnixDomainSocketEndPoint(path);
                using var client = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                client.Connect(endPoint);

                var message = "alive"u8.ToArray();
                client.Send(message);
                var buffer = new byte[1024];
                var received = client.Receive(buffer);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                if (response != "I AM ALIVE") continue;
                return path;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        throw new Exception("Cannot find socket.");
    }

    private static string GetRunnableSocket()
    {
        foreach (var path in SocketPaths)
            try
            {
                if (File.Exists(path))
                {
                    using var fs = File.Open(path, FileMode.Open, FileAccess.Write);
                    return path;
                }

                var dir = Path.GetDirectoryName(path);
                var testFile = Path.Combine(dir!, $".test_write_{Guid.NewGuid():N}");
                using (File.Create(testFile))
                {
                }

                File.Delete(testFile);
                return path;
            }
            catch
            {
                // ignored
            }

        throw new Exception("Cannot find socket.");
    }
    public static void DaemonMode()
    {
        Console.WriteLine($"[Daemon] [I] Initialized with {Shared.Gamerun.Apps.Count} total app(s).");
        var runInSocket = GetRunnableSocket();
        Console.WriteLine($"[Daemon] [I] Running in socket \"{runInSocket}\"...");
        var endPoint = new UnixDomainSocketEndPoint(runInSocket);
        if (File.Exists(runInSocket))
        {
            var skipShutdownRequest = false;
            using var client = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
            try
            {
                Console.WriteLine(
                    $"[Daemon] [I] Connecting to socket \"{runInSocket}\" for replacement shutdown request...");
                client.Connect(endPoint);
            }
            catch (SocketException)
            {
                Console.WriteLine(
                    $"[Daemon] [I] Socket \"{runInSocket}\" exist but nothing is running. Deleting file...");
                File.Delete(runInSocket);
                skipShutdownRequest = true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(
                    $"[Daemon] [F] Cannot replace socket (\"{runInSocket}\"). Exception caught: {ex}");
                Environment.ExitCode = 1;
                return;
            }

            if (!skipShutdownRequest)
            {
                var message = "shutdown"u8.ToArray();
                client.Send(message);
                Console.WriteLine($"[Daemon] [I] Socket \"{runInSocket}\" killed.");
            }
        }
        
        var currentApps =  new List<GamerunDaemonArgs>();

        var listener = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
        listener.Bind(endPoint);
        listener.Listen(5);
        Console.WriteLine($"[Daemon] [I] Started listening on socket \"{runInSocket}\".");

        while (true)
        {
            Console.WriteLine("[Daemon] [I] Client connected...");
            var client = listener.Accept();
            var buffer = new byte[1024];
            var bytesRead = client.Receive(buffer);
            var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            if (message.StartsWith("shutdown", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[Daemon] [I] Shutting down socket \"{runInSocket}\".");
                client.Close();
                listener.Disconnect(true);
                listener.Shutdown(SocketShutdown.Both);
                listener.Close();
                break;
            }

            if (message.StartsWith("alive", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("[Daemon] [I] Alive request received.");
                client.Send("I AM ALIVE"u8.ToArray());
                continue;
            }

            var arguments = message.Split(' ');

            switch (arguments[0])
            {
                case "boost": // boost <base64>
                    if (arguments.Length <= 1) break;
                    var decoded = new GamerunDaemonArgs(arguments[1]);
                    currentApps.Add(decoded);
                    if (decoded.SetTLP)
                    {
                        Process.Start(new ProcessStartInfo(Tools.GetCommand("tlp"), "ac"));
                    }

                    if (decoded.Prioritize)
                    {
                        SetPriority(decoded.PID);
                    }

                    if (decoded.PrioritizeIO)
                    {
                        SetIOPriority(decoded.PID);
                    }

                    if (decoded.SetSplitLockMitigation)
                    {
                        SetSplitLockMitigation(true);
                    }

                    if (decoded.ParkCores)
                    {
                        CPUSetting.ParkCores(decoded.ParkedCores, false);
                    }

                    if (decoded.PinCores)
                    {
                        CPUSetting.PinCores(decoded.PID, decoded.PinnedCores);
                    }

                    if (decoded.OptimizeGPU)
                    {
                        // TODO: Optimize GPUs here
                        if (decoded.AMDPerfLevel)
                        {
                            // AMD performance level
                        }

                        if (decoded.NvPowerMizer)
                        {
                            // Nvidia PowerMizer
                        }

                        if (decoded.NvMemClockOffset > 0)
                        {
                            // Nvidia mem clock overclock
                        }
                        
                        if (decoded.NvCoreClockOffset > 0)
                        {
                            // Nvidia core clock overclock
                        }
                    }

                    if (decoded.iGPUGovernor)
                    {
                        // TODO: iGPU stuff here
                    }

                    break;
                case "revert": // revert <pid>
                    if (arguments.Length <= 1) break;
                    if (!int.TryParse(arguments[1], out var pid)) break;
                    var found = currentApps.Find(it => it.PID == pid);
                    if (found == null) break;
                    if (found.SetTLP && currentApps.FindAll(it => it.SetTLP).Count <= 1)
                    {
                        Process.Start(new ProcessStartInfo(Tools.GetCommand("tlp"), "bat"));
                    }
                    if (found.SetSplitLockMitigation && currentApps.FindAll(it => it.SetSplitLockMitigation).Count <= 1)
                    {
                        SetSplitLockMitigation(false);
                    }
                    if (found.ParkCores &&  currentApps.FindAll(it => it.ParkCores).Count <= 1)
                    {
                        CPUSetting.ParkCores(found.ParkedCores, true);
                    }

                    if (found.OptimizeGPU)
                    {
                        // TODO: revert GPUs here
                        if (found.AMDPerfLevel)
                        {
                            // AMD performance level
                        }

                        if (found.NvPowerMizer)
                        {
                            // Nvidia PowerMizer
                        }

                        if (found.NvMemClockOffset > 0)
                        {
                            // Nvidia mem clock overclock
                        }
                        
                        if (found.NvCoreClockOffset > 0)
                        {
                            // Nvidia core clock overclock
                        }
                    }

                    if (found.iGPUGovernor)
                    {
                        // TODO: iGPU stuff here
                    }
                    currentApps.Remove(found);
                    break;

                case null:
                    Console.WriteLine($"[Daemon] [E] Received message \"{message}\" is not well-formed. Ignored.");
                    break;

                default:
                    Console.WriteLine($"[Daemon] [E] Message \"{runInSocket}\" unknown. Ignored.");
                    break;
            }

            client.Close();
        }
    }

    [DllImport("libc", SetLastError = true)]
    private static extern int syscall(ulong number, int which, int who, int ioprio);

    // Needs root privileges!
    private static int SetIOPriority(int pid)
    {
        const ulong sys_ioprio_set = 251;
        const int IOPRIO_CLASS_RT = 1;
        return syscall(sys_ioprio_set, 1, pid, (IOPRIO_CLASS_RT << 13) | 0);
    }

    [DllImport("libc", SetLastError = true)]
    private static extern int setpriority(int which, int who, int prio);
    
    [DllImport("libc")]
    private static extern int sched_setscheduler(int pid, int policy, ref sched_param param);


    // TODO: iGPU Governor and Treshold
    // /sys/class/drm/card0/device/power_dpm_force_performance_level
    // low auto high normal

    private static void SetSplitLockMitigation(bool disabled)
    {
        if (!File.Exists("/proc/sys/kernel/split_lock_mitigate")) return;
        using var fileStream = new FileStream("/proc/sys/kernel/split_lock_mitigate",  FileMode.Truncate, FileAccess.Write);
        fileStream.SetLength(0);
        using var writer = new StreamWriter(fileStream);
        writer.Write(disabled ? "0" : "1");
    }

    // TODO: GPU Overclocking

    // Needs root privileges!
    private static int SetPriority(int pid)
    {
        return setpriority(0, pid, -10);
    }

    private struct sched_param
    {
        public int sched_priority;
    }
}