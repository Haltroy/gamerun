using System;
using System.Diagnostics;
using System.IO;
namespace Gamerun.Shared
{
    public enum InitSystem
    {
        Systemd,
        OpenRC,
        Runit,
        S6,
        BusyBox,
        Unknown
    }
    public static class InitSystemHelper
    {
        private static InitSystem GetInitSystem()
        {
            if (Directory.Exists("/run/systemd/system"))
                return InitSystem.Systemd;

            if (File.Exists("/sbin/openrc") || File.Exists("/bin/openrc"))
                return InitSystem.OpenRC;

            if (Directory.Exists("/etc/runit"))
                return InitSystem.Runit;

            if (Environment.GetEnvironmentVariable("S6_OVERLAY_VERSION") != null ||
                Directory.Exists("/run/service") && Directory.Exists("/etc/s6"))
                return InitSystem.S6;

            try
            {
                var initPath = File.ReadAllText("/proc/1/comm").Trim();
                if (initPath.Equals("busybox", StringComparison.OrdinalIgnoreCase))
                    return InitSystem.BusyBox;
            }
            catch
            {
                // ignored
            }

            return InitSystem.Unknown;
        }

        public static bool IsServiceActiveAsync(string serviceName)
        {
            return GetInitSystem() switch
            {
                InitSystem.Systemd =>  IsSystemdServiceActiveAsync(serviceName),
                InitSystem.OpenRC  =>  IsOpenRCServiceActiveAsync(serviceName),
                InitSystem.Runit   =>  IsRunitServiceActiveAsync(serviceName),
                InitSystem.S6      =>  IsS6ServiceActiveAsync(serviceName),
                InitSystem.BusyBox =>  IsBusyBoxServiceActiveAsync(serviceName),
                _                  => false
            };
        }

        private static bool IsSystemdServiceActiveAsync(string serviceName)
        {
            return RunShellCommandAsync("systemctl", $"is-active {(serviceName.EndsWith(".service") ? serviceName : $"{serviceName}.service")}") == "active";
        }

        private static bool IsOpenRCServiceActiveAsync(string serviceName)
        {
            var result = RunShellCommandAsync("rc-service",  $"{serviceName} status");
            return result.Contains("started", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsRunitServiceActiveAsync(string serviceName)
        {
            return File.Exists($"/etc/service/{serviceName}/run") || File.Exists($"/run/runit/service/{serviceName}/run");
        }

        private static bool IsS6ServiceActiveAsync(string serviceName)
        {
            return Directory.Exists($"/run/service/{serviceName}") || Directory.Exists($"/etc/s6/{serviceName}");
        }

        private static bool IsBusyBoxServiceActiveAsync(string serviceName)
        {
            // BusyBox init has no consistent service control. Optionally use pidof
            return !string.IsNullOrWhiteSpace(RunShellCommandAsync("pidof", serviceName));
        }

        private static string RunShellCommandAsync(string command, string arguments)
        {
            var psi = new ProcessStartInfo(Tools.GetCommand(command), arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var process = Process.Start(psi);
            var output =  process == null ? string.Empty : process.StandardOutput.ReadToEnd(); 
            process?.WaitForExit();

            return output.Trim();
        }
    }
}