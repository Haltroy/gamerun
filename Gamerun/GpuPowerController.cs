using System.Collections.Concurrent;
using Gamerun.Shared;

namespace Gamerun.GPU;

public class GpuPowerController : IDisposable
{
    // store previous sysfs values so we can restore them later
    private readonly ConcurrentDictionary<string, string> _previousValues = new();

    public void Dispose()
    {
        // Attempt to restore all stored values
        foreach (var kv in _previousValues.ToArray())
            try
            {
                File.WriteAllText(kv.Key, kv.Value);
            }
            catch
            {
                /* swallow; best-effort */
            }

        _previousValues.Clear();
    }

    private static string SysfsPathForDrmDevice(GpuInfo gpu)
    {
        // prefer DRMPath if your GpuInfo has it (e.g. "/sys/class/drm/card0")
        if (!string.IsNullOrEmpty(gpu.DRMPath) && Directory.Exists(gpu.DRMPath))
            return Path.GetFullPath(gpu.DRMPath);

        // fallback: find device via /sys/bus/pci/devices/<pci>/... as earlier
        var candidate = $"/sys/bus/pci/devices/{gpu.PciId}";
        if (Directory.Exists(candidate))
            return candidate;

        return null;
    }

    #region AMD perf level

    // Typical file:
    // /sys/class/drm/cardX/device/power_dpm_force_performance_level
    // values: "auto", "low", "high", "manual"
    public bool TrySetAmdPerfLevel(GpuInfo gpu, string level)
    {
        if (gpu == null) return false;
        var drm = SysfsPathForDrmDevice(gpu);
        if (string.IsNullOrEmpty(drm)) return false;

        // file is under device/ for DRM path; if drm points to cardX, we want cardX/device/
        var devPath = drm.EndsWith("/device") ? drm : Path.Combine(drm, "device");
        var path = Path.Combine(devPath, "power_dpm_force_performance_level");

        // older kernels or drivers might use other files
        if (!File.Exists(path))
        {
            // try alternate path names
            var alt1 = Path.Combine(devPath, "power_profile");
            if (File.Exists(alt1)) path = alt1;
            else
                // no supported attribute
                return false;
        }

        try
        {
            // save current
            var current = File.ReadAllText(path).Trim();
            _previousValues.TryAdd(path, current);

            // write new value
            File.WriteAllText(path, level);
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"TrySetAmdPerfLevel failed for {gpu.PciId}: {ex.Message}");
            return false;
        }
    }

    public bool TryRestoreAmdPerfLevel(GpuInfo gpu)
    {
        var drm = SysfsPathForDrmDevice(gpu);
        if (string.IsNullOrEmpty(drm)) return false;
        var devPath = drm.EndsWith("/device") ? drm : Path.Combine(drm, "device");
        var path = Path.Combine(devPath, "power_dpm_force_performance_level");
        if (!_previousValues.TryRemove(path, out var previous)) return false;

        try
        {
            File.WriteAllText(path, previous);
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"TryRestoreAmdPerfLevel failed for {gpu.PciId}: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region iGPU governor & threshold (Intel / AMD)

    // For Intel i915 we try gt_min_freq_mhz / gt_max_freq_mhz if available.
    // For AMD iGPU, we try the same power_dpm_force_performance_level as dGPU.
    public bool TrySetIgpuGovernorAndThreshold(GpuInfo igpu, string governor, int? thresholdMhz = null)
    {
        if (igpu == null) return false;
        var drm = SysfsPathForDrmDevice(igpu);
        if (string.IsNullOrEmpty(drm)) return false;

        var devPath = drm.EndsWith("/device") ? drm : Path.Combine(drm, "device");

        // Detect driver
        var driver = (igpu.Driver ?? "").ToLowerInvariant();

        try
        {
            if (driver.Contains("i915") || driver.Contains("intel"))
            {
                // Intel: prefer gt_min_freq_mhz / gt_max_freq_mhz
                var minPath = Path.Combine(devPath, "gt_min_freq_mhz");
                var maxPath = Path.Combine(devPath, "gt_max_freq_mhz");

                if (File.Exists(minPath) && File.Exists(maxPath))
                {
                    // save previous values if not saved
                    _previousValues.TryAdd(minPath, File.ReadAllText(minPath).Trim());
                    _previousValues.TryAdd(maxPath, File.ReadAllText(maxPath).Trim());

                    // governor semantics: "performance" -> set min = max or high value
                    if (string.Equals(governor, "performance", StringComparison.OrdinalIgnoreCase))
                    {
                        var toSet = thresholdMhz ?? int.Parse(File.ReadAllText(maxPath).Trim());
                        File.WriteAllText(minPath, toSet.ToString());
                        // optionally bump max as well
                    }
                    else if (string.Equals(governor, "powersave", StringComparison.OrdinalIgnoreCase))
                    {
                        // reduce max down to threshold if provided
                        if (thresholdMhz.HasValue) File.WriteAllText(maxPath, thresholdMhz.Value.ToString());
                        // set default low value (leave unchanged if not desirable)
                    }

                    return true;
                }

                // Fallback: use energy_performance_preference on CPUs (applies to CPU, not GPU)
                // but some newer intel stacks have power_policy files; we check for them:
                var eppPath = "/sys/devices/system/cpu/cpu0/power/energy_performance_preference";
                if (File.Exists(eppPath))
                {
                    // we can just set the general CPU EPP (less ideal)
                    _previousValues.TryAdd(eppPath, File.ReadAllText(eppPath).Trim());
                    File.WriteAllText(eppPath, governor); // "performance", "balance-power", etc.
                    return true;
                }

                return false;
            }

            if (driver.Contains("amdgpu") || driver.Contains("radeon"))
            {
                // AMD iGPU: same as AMD dGPU if supported
                var perfPath = Path.Combine(devPath, "power_dpm_force_performance_level");
                if (File.Exists(perfPath))
                {
                    _previousValues.TryAdd(perfPath, File.ReadAllText(perfPath).Trim());
                    File.WriteAllText(perfPath, governor);
                    return true;
                }

                return false;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"TrySetIgpuGovernorAndThreshold failed: {ex.Message}");
            return false;
        }
    }

    public bool TryRestoreIgpuGovernor(GpuInfo igpu)
    {
        if (igpu == null) return false;
        var drm = SysfsPathForDrmDevice(igpu);
        if (string.IsNullOrEmpty(drm)) return false;
        var devPath = drm.EndsWith("/device") ? drm : Path.Combine(drm, "device");

        // attempt to restore known keys we may have set
        var possibleKeys = new[]
        {
            "gt_min_freq_mhz", "gt_max_freq_mhz", "power_dpm_force_performance_level",
            "/sys/devices/system/cpu/cpu0/power/energy_performance_preference"
        };

        var restoredAny = false;
        foreach (var key in possibleKeys)
        {
            var path = key.StartsWith("/") ? key : Path.Combine(devPath, key);
            if (_previousValues.TryRemove(path, out var prev))
                try
                {
                    File.WriteAllText(path, prev);
                    restoredAny = true;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed restoring {path}: {ex.Message}");
                }
        }

        return restoredAny;
    }

    #endregion
}