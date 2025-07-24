using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Gamerun;

internal class Program
{
    private static string[] SocketPaths =>
        ["/run/gamerun.sock", $"/run/user/{getuid()}/gamerun.sock", "/tmp/gamerun.sock"];

    private static string GetRunningSocket()
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

    private static void Main(string[] args)
    {
        if (args.Contains("--daemon", StringComparer.OrdinalIgnoreCase))
        {
            Shared.Gamerun.Init();
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
                    case "boost": // boosts PID
                        // TODO
                        break;
                    case "revert": // reverts boost from app
                        // TODO
                        break;
                    case "refresh":
                        Shared.Gamerun.Refresh();
                        Console.WriteLine($"[Daemon] [I] Refreshed with {Shared.Gamerun.Apps.Count} total app(s).");
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

            return;
        }

        var runningSocket = GetRunningSocket();

        if (args.Contains("--shutdown", StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine("[Client] [I] Sending shutdown request...");
            var endPoint = new UnixDomainSocketEndPoint(runningSocket);
            using var client = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
            client.Connect(endPoint);

            var message = "shutdown"u8.ToArray();
            client.Send(message);
            return;
        }

        try
        {
            var appCommandLine = args[0];
            var app = Shared.Gamerun.GetApp(appCommandLine);

            if (app.Settings.RequireRootPermissions)
            {
                var endPoint = new UnixDomainSocketEndPoint(runningSocket);
                using var client = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                client.Connect(endPoint);

                var message = "boost-on"u8.ToArray(); // TODO
                client.Send(message);
            }
            // TODO: Boost app
        }
        catch (SocketException soEx) // added "o" in the middle, you know why
        {
            Console.Error.WriteLine(
                $"[Client] [F] Cannot connect to the socket (located in \"{runningSocket}\"). Exception caught: {soEx}");
        }
        catch (SecurityException secEx)
        {
            Console.Error.WriteLine(
                $"[Client] [F] Cannot connect to the socket (located in \"{runningSocket}\") due to an security error. Exception caught: {secEx}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[Client] [F] Cannot set it to high performance. Exception caught: {ex}");
        }
    }

    private static void EnablePerformanceMode(string commandLine)
    {
        Console.WriteLine("EnablePerformanceMode() requested...");
        SetCPUScalingGovernor("performance");
        // TODO
    }

    private static void RestoreSettings(string commandLine)
    {
        Console.WriteLine("RestoreSettings() requested...");
        SetCPUScalingGovernor("powersave");
        // TODO
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

    // Needs root privileges!
    private static int SetPriority(int pid)
    {
        return setpriority(0, pid, -10);
    }

    [DllImport("libc")]
    private static extern uint getuid();

    // Needs root privileges!
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