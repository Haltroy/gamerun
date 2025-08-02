using System.Net.Sockets;
using System.Security;

namespace Gamerun;

internal class Program
{
    private static void Main(string[] args)
    {
        // TODO: better arg reading
        // gamerun [--daemon|--editor|--help|--shutdown|--alive] [COMMAND]
        if (args.Contains("--daemon", StringComparer.OrdinalIgnoreCase))
        {
            Daemon.DaemonMode();
            return;
        }

        var runningSocket = Daemon.GetRunningSocket();

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
            var settings = app.Settings ?? Shared.Gamerun.Default;

            if (settings.RequireRootPermissions)
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
}