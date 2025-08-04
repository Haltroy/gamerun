using System.Diagnostics;
using System.Net.Sockets;
using System.Security;
using System.Text;
using Gamerun.Shared.Translations;

namespace Gamerun;

internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length <= 0 || args.Contains("--daemon", StringComparer.OrdinalIgnoreCase))
        {
            var consoleLength = Console.BufferWidth;
            // ReSharper disable once LocalizableElement
            Console.WriteLine($"gamerun [--daemon|--help--shutdown|--alive|--busy] <{Translations.ClientHelpCommand}>");
            Console.WriteLine(Translations.ClientHelpDesc);
            Console.WriteLine();
            Console.WriteLine(Translations.ClientHelpUsage);
            foreach (var arguments in new List<string[]>
                     {
                         new[] { "Daemon", Translations.ClientHelpDaemon },
                         new[] { "Help", Translations.ClientHelpHelp },
                         new[] { "Shutdown", Translations.ClientHelpShutdown },
                         new[] { "Alive", Translations.ClientHelpAlive },
                         new[] { "Busy", Translations.ClientHelpBusy },
                         new[] { "CMD", Translations.ClientHelpCMD }
                     })
            {
                var leftSide = arguments[0];
                var rightSide = arguments[1];
                var spaceLength = consoleLength - (leftSide.Length + rightSide.Length);
                var space = string.Empty;
                if (spaceLength <= 0) space = "|";
                else
                    for (var i = 0; i < spaceLength; i++)
                        space += " ";

                // ReSharper disable once LocalizableElement
                Console.WriteLine($"{leftSide}{space}{rightSide}");
            }

            return;
        }

        if (args.Contains("--daemon", StringComparer.OrdinalIgnoreCase))
        {
            Daemon.DaemonMode();
            return;
        }

        var runningSocket = Daemon.GetRunningSocket();

        if (args.Contains("--shutdown", StringComparer.OrdinalIgnoreCase))
        {
            var endPoint = new UnixDomainSocketEndPoint(runningSocket);
            using var client = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
            client.Connect(endPoint);

            var message = "shutdown"u8.ToArray();
            client.Send(message);
            return;
        }

        if (args.Contains("--alive", StringComparer.OrdinalIgnoreCase))
        {
            var endPoint = new UnixDomainSocketEndPoint(runningSocket);
            using var client = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
            client.Connect(endPoint);

            var message = "alive"u8.ToArray();
            client.Send(message);
            var buffer = new byte[1024];
            var received = client.Receive(buffer);
            var response = Encoding.UTF8.GetString(buffer, 0, received);
            Console.WriteLine(response);
            return;
        }

        if (args.Contains("--busy", StringComparer.OrdinalIgnoreCase))
        {
            var endPoint = new UnixDomainSocketEndPoint(runningSocket);
            using var client = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
            client.Connect(endPoint);

            var message = "busy"u8.ToArray();
            client.Send(message);
            var buffer = new byte[1024];
            var received = client.Receive(buffer);
            var response = Encoding.UTF8.GetString(buffer, 0, received);
            Console.WriteLine(response);
            return;
        }

        try
        {
            var appCommandLine = args[0];
            if (!File.Exists(appCommandLine))
            {
                Console.Error.WriteLine(Translations.ClientCannotFindApp.Replace("%cmd%", appCommandLine));
                return;
            }

            var app = Shared.Gamerun.GetApp(appCommandLine, true, true);
            UnixDomainSocketEndPoint? endPoint = null;
            Socket? client = null;
            var settings = app.Settings ?? Shared.Gamerun.Default;
            var appArgs = new GamerunStartArguments();
            settings.GenerateArgs(appArgs);
            foreach (var env in appArgs.Environment) Environment.SetEnvironmentVariable(env.Key, env.Value);

            foreach (var call in appArgs.StartDBusCalls)
            {
                var callArgs = new object[call.Arguments.Length];
                for (var i = 0; i < call.Arguments.Length; i++) callArgs[i] = call.Arguments[i];
                SimpleDBus.Send(call.Destination, call.ObjectPath, call.Destination, call.Method, callArgs);
            }

            foreach (var startCommand in appArgs.StartCommands) Process.Start(startCommand);

            if (!string.IsNullOrWhiteSpace(appArgs.StartScript))
            {
                var startCommandUser = Process.Start(appArgs.StartScript);
                switch (appArgs.StartScriptTimeout)
                {
                    case -1:
                        startCommandUser.WaitForExit();
                        break;
                    case 0:
                        break;
                    default:
                        Thread.Sleep(1000 * appArgs.StartScriptTimeout);
                        if (!startCommandUser.HasExited) startCommandUser.Kill();
                        break;
                }
            }

            var process = Process.Start(appArgs.Prefix + (appArgs.Prefix.EndsWith(' ') ? "" : " ") + appCommandLine +
                                        (appArgs.Postfix.StartsWith(' ') ? "" : " ") + appArgs.Postfix);
            appArgs.DaemonArgs.PID = process.Id;
            if (settings.RequireRootPermissions)
            {
                endPoint = new UnixDomainSocketEndPoint(runningSocket);
                client = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                client.Connect(endPoint);

                var message = Encoding.UTF8.GetBytes($"boost {appArgs.DaemonArgs.ToBase64()}");
                client.Send(message);
            }

            process.WaitForExit();
            var skipDbus = false;
            if (client != null && endPoint != null)
            {
                client.Send(Encoding.UTF8.GetBytes($"revert {appArgs.DaemonArgs.PID}"));
                client.Send("busy"u8.ToArray());
                var buffer = new byte[1024];
                var received = client.Receive(buffer);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                if (response.Equals("yes", StringComparison.CurrentCultureIgnoreCase)) skipDbus = true;
            }

            if (!skipDbus)
            {
                foreach (var call in appArgs.EndDBusCalls)
                {
                    var callArgs = new object[call.Arguments.Length];
                    for (var i = 0; i < call.Arguments.Length; i++) callArgs[i] = call.Arguments[i];

                    SimpleDBus.Send(call.Destination, call.ObjectPath, call.Destination, call.Method, callArgs);
                }

                foreach (var endCommand in appArgs.EndCommands) Process.Start(endCommand);
            }

            if (string.IsNullOrWhiteSpace(appArgs.EndScript)) return;
            var endCommandUser = Process.Start(appArgs.StartScript);
            switch (appArgs.EndScriptTimeout)
            {
                case -1:
                    endCommandUser.WaitForExit();
                    break;
                case 0:
                    break;
                default:
                    Thread.Sleep(1000 * appArgs.EndScriptTimeout);
                    if (!endCommandUser.HasExited) endCommandUser.Kill();
                    break;
            }
        }
        catch (SocketException soEx) // added "o" in the middle, you know why
        {
            Console.Error.WriteLine(Translations.ClientSocketException.Replace("%l%", runningSocket)
                .Replace("%ex%", soEx.ToString()));
        }
        catch (SecurityException secEx)
        {
            Console.Error.WriteLine(Translations.ClientSecurityException.Replace("%runningSocket%", runningSocket)
                .Replace("%secEx%", secEx.ToString()));
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(Translations.ClientRunException.Replace("%ex%", ex.ToString()));
        }
    }
}