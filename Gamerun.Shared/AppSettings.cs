using System.IO;
using Gamerun.Shared.Exceptions;

namespace Gamerun.Shared;

public class AppSettings : GamerunSettingsAbstract
{
    #region CONSTRUCTORS

    public AppSettings()
    {
        Cores = Tools.DetectCpuTopology();
        foreach (var cpu in Cores) cpu.ParkedPinnedChanged += () => OnSave?.Invoke();
    }

    #endregion CONSTRUCTORS

    #region OTHER SETTINGS

    // ReSharper disable once InconsistentNaming
    // ReSharper disable once MemberCanBePrivate.Global
    public MangoHUDSettings MangoHUD { get; } = new();

    // ReSharper disable once MemberCanBePrivate.Global
    public StrangleSettings Strangle { get; } = new();

    public GamescopeSettings Gamescope { get; } = new();

    #endregion OTHER SETTINGS

    #region PROPERTIES

    // ReSharper disable once InconsistentNaming
    public bool UseGPU
    {
        get => _useGPU ?? Gamerun.Default.UseGPU;
        set
        {
            _useGPU = value;
            OnSave?.Invoke();
        }
    }

    public bool EnablePowerDaemon
    {
        get => _enablePowerDaemon ?? Gamerun.Default.EnablePowerDaemon;
        set
        {
            _enablePowerDaemon = value;
            OnSave?.Invoke();
        }
    }

    public bool EnableFanController
    {
        get => _enableFanController ?? Gamerun.Default.EnableFanController;
        set
        {
            _enableFanController = value;
            OnSave?.Invoke();
        }
    }

    public bool Prioritize
    {
        get => _prioritize ?? Gamerun.Default.Prioritize;
        set
        {
            _prioritize = value;
            OnSave?.Invoke();
        }
    }

    public bool PrioritizeIO
    {
        get => _prioritizeIO ?? Gamerun.Default.PrioritizeIO;
        set
        {
            _prioritizeIO = value;
            OnSave?.Invoke();
        }
    }

    public float iGPUTreshold
    {
        get => _igpuTreshold ?? Gamerun.Default.iGPUTreshold;
        set
        {
            _igpuTreshold = value;
            OnSave?.Invoke();
        }
    }

    public ProcessorCore[] Cores { get; set; }

    public string StartupScriptPath
    {
        get => _startupScriptPath ?? Gamerun.Default.StartupScriptPath;
        set
        {
            _startupScriptPath = value;
            OnSave?.Invoke();
        }
    }

    public string StopScriptPath
    {
        get => _stopScriptPath ?? Gamerun.Default.StopScriptPath;
        set
        {
            _stopScriptPath = value;
            OnSave?.Invoke();
        }
    }

    public bool AMDPerfLevel
    {
        get => _amdPerfLevel ?? Gamerun.Default.AMDPerfLevel;
        set
        {
            _amdPerfLevel = value;
            OnSave?.Invoke();
        }
    }

    public bool BlockScreenSaver
    {
        get => _blockScreenSaver ?? Gamerun.Default.BlockScreenSaver;
        set
        {
            _blockScreenSaver = value;
            OnSave?.Invoke();
        }
    }

    public bool CPUGovernor
    {
        get => _cpuGovernor ?? Gamerun.Default.CPUGovernor;
        set
        {
            _cpuGovernor = value;
            OnSave?.Invoke();
        }
    }

    public bool DisableSplitLockMitigation
    {
        get => _disableSplitLockMitigation ?? Gamerun.Default.DisableSplitLockMitigation;
        set
        {
            _disableSplitLockMitigation = value;
            OnSave?.Invoke();
        }
    }

    public bool iGPUGovernor
    {
        get => _igpuGovernor ?? Gamerun.Default.iGPUGovernor;
        set
        {
            _igpuGovernor = value;
            OnSave?.Invoke();
        }
    }

    public bool NvPowerMizer
    {
        get => _nvPowerMizer ?? Gamerun.Default.NvPowerMizer;
        set
        {
            _nvPowerMizer = value;
            OnSave?.Invoke();
        }
    }

    public bool OptimizeGPU
    {
        get => _optimizeGPU ?? Gamerun.Default.OptimizeGPU;
        set
        {
            _optimizeGPU = value;
            OnSave?.Invoke();
        }
    }

    public bool ParkCores
    {
        get => _parkCores ?? Gamerun.Default.ParkCores;
        set
        {
            _parkCores = value;
            OnSave?.Invoke();
        }
    }

    public bool ParkCoresAuto
    {
        get => _parkCoresAuto ?? Gamerun.Default.ParkCoresAuto;
        set
        {
            _parkCoresAuto = value;
            OnSave?.Invoke();
        }
    }

    public bool PinCores
    {
        get => _pinCores ?? Gamerun.Default.PinCores;
        set
        {
            _pinCores = value;
            OnSave?.Invoke();
        }
    }

    public bool PinCoresAuto
    {
        get => _pinCoresAuto ?? Gamerun.Default.PinCoresAuto;
        set
        {
            _pinCoresAuto = value;
            OnSave?.Invoke();
        }
    }

    public bool PowerGovernor
    {
        get => _powerGovernor ?? Gamerun.Default.PowerGovernor;
        set
        {
            _powerGovernor = value;
            OnSave?.Invoke();
        }
    }


    public bool SoftRealTime
    {
        get => _softrealtime ?? Gamerun.Default.SoftRealTime;
        set
        {
            _softrealtime = value;
            OnSave?.Invoke();
        }
    }

    public uint StopScriptTimeout
    {
        get => _stopScriptTimeout ?? Gamerun.Default.StopScriptTimeout;
        set
        {
            _stopScriptTimeout = value;
            OnSave?.Invoke();
        }
    }

    public uint GPUID
    {
        get => _gpuID ?? Gamerun.Default.GPUID;
        set
        {
            _gpuID = value;
            OnSave?.Invoke();
        }
    }

    public uint NvCoreClockOffset
    {
        get => _nvCoreClockOffset ?? Gamerun.Default.NvCoreClockOffset;
        set
        {
            _nvCoreClockOffset = value;
            OnSave?.Invoke();
        }
    }

    public uint NvMemClockOffset
    {
        get => _nvMemClockOffset ?? Gamerun.Default.NvMemClockOffset;
        set
        {
            _nvMemClockOffset = value;
            OnSave?.Invoke();
        }
    }

    public uint StartupScriptTimeout
    {
        get => _startupScriptTimeout ?? Gamerun.Default.StartupScriptTimeout;
        set
        {
            _startupScriptTimeout = value;
            OnSave?.Invoke();
        }
    }

    public bool DisableNotificationSystem
    {
        get => _notifications ?? Gamerun.Default.DisableNotificationSystem;
        set
        {
            _notifications = value;
            OnSave?.Invoke();
        }
    }

    public bool OptimizeCompositor
    {
        get => _compositor ?? Gamerun.Default.OptimizeCompositor;
        set
        {
            _compositor = value;
            OnSave?.Invoke();
        }
    }

    public bool RequireRootPermissions => Prioritize
                                          || PrioritizeIO
                                          || OptimizeGPU
                                          || (EnablePowerDaemon && InitSystemHelper.IsServiceActiveAsync("tlp"))
                                          || ParkCores
                                          || PinCores
                                          || (Tools.IsIntelCpu() && DisableSplitLockMitigation)
                                          || SoftRealTime
                                          || OptimizeGPU;

    public override bool IsDefaults => Strangle.IsDefaults &&
                                       MangoHUD.IsDefaults &&
                                       _amdPerfLevel == null &&
                                       _blockScreenSaver == null &&
                                       _cpuGovernor == null &&
                                       _disableSplitLockMitigation == null &&
                                       _enableFanController == null &&
                                       _enablePowerDaemon == null &&
                                       _gpuID == null &&
                                       _igpuGovernor == null &&
                                       _igpuTreshold == null &&
                                       _nvCoreClockOffset == null &&
                                       _nvMemClockOffset == null &&
                                       _nvPowerMizer == null &&
                                       _optimizeGPU == null &&
                                       _parkCores == null &&
                                       _parkCoresAuto == null &&
                                       _pinCores == null &&
                                       _pinCoresAuto == null &&
                                       _powerGovernor == null &&
                                       _prioritize == null &&
                                       _prioritizeIO == null &&
                                       _softrealtime == null &&
                                       _startupScriptPath == null &&
                                       _startupScriptTimeout == null &&
                                       _stopScriptPath == null &&
                                       _stopScriptTimeout == null &&
                                       _compositor == null &&
                                       _notifications == null &&
                                       _useGPU == null;

    #endregion PROPERTIES

    #region OVERRIDES

    public override void ReadSettings(Stream stream)
    {
        var bufferByte = stream.ReadByte();
        if (bufferByte == -1) throw new GamerunEndOfStreamException(stream.Position);
        _prioritize = Tools.IsBitSet(bufferByte, 0);
        _useGPU = Tools.IsBitSet(bufferByte, 1);
        // TODO
        Strangle.ReadSettings(stream);
        MangoHUD.ReadSettings(stream);
        Gamescope.ReadSettings(stream);
    }

    public override void WriteSettings(Stream stream)
    {
        var buffer = (byte)(Prioritize ? 1 : 0);
        buffer += (byte)(UseGPU ? 2 : 0);
        // TODO
        stream.WriteByte(buffer);
        Strangle.WriteSettings(stream);
        MangoHUD.WriteSettings(stream);
        Gamescope.WriteSettings(stream);
    }

    public override GamerunStartArguments GenerateArgs(GamerunStartArguments args)
    {
        args = Strangle.GenerateArgs(args);
        args = MangoHUD.GenerateArgs(args);
        args = Gamescope.GenerateArgs(args);

        if (InitSystemHelper.IsServiceActiveAsync("power-profiles-daemon"))
        {
            args.StartDBusCalls.Add(new GamerunDBusCalls
            {
                Destination = "org.freedesktop.PowerProfiles",
                ObjectPath = "/org/freedesktop/PowerProfiles",
                Method = "org.freedesktop.PowerProfiles.SetActiveProfile",
                Arguments = ["performance"]
            });

            args.EndDBusCalls.Add(new GamerunDBusCalls
            {
                Destination = "org.freedesktop.PowerProfiles",
                ObjectPath = "/org/freedesktop/PowerProfiles",
                Method = "org.freedesktop.PowerProfiles.SetActiveProfile",
                Arguments = ["balanced"]
            });
            return args;
        }

        args.DaemonArgs.SetTLP = InitSystemHelper.IsServiceActiveAsync("tlp");

        if (InitSystemHelper.IsServiceActiveAsync("nbfc_service"))
        {
            args.StartCommands.Add($"{Tools.GetCommand("nbfc")} set --speed 100");
            args.EndCommands.Add($"{Tools.GetCommand("nbfc")} set --auto");
        }

        if (InitSystemHelper.IsServiceActiveAsync("asusd"))
        {
            args.StartCommands.Add($"{Tools.GetCommand("asusctl")} profile performance");
            args.EndCommands.Add($"{Tools.GetCommand("asusctl")} profile balanced");
        }

        if (BlockScreenSaver)
        {
            args.StartDBusCalls.Add(new GamerunDBusCalls
            {
                Destination = "org.freedesktop.ScreenSaver",
                ObjectPath = "/org/freedesktop/ScreenSaver",
                Method = "org.freedesktop.ScreenSaver.Inhibit",
                Arguments = ["Gamerun", "App is running..."] // TODO: Translate this
            });
            args.StartDBusCalls.Add(new GamerunDBusCalls
            {
                Destination = "org.freedesktop.ScreenSaver",
                ObjectPath = "/org/freedesktop/ScreenSaver",
                Method = "org.freedesktop.ScreenSaver.UnInhibit",
                Arguments = []
            });
        }

        // Core pinning
        if (PinCoresAuto)
            foreach (var core in Cores)
                core.IsPinned = core.Type != ProcessorCoreType.LowPower;

        // Core parking
        if (ParkCoresAuto)
            foreach (var core in Cores)
                core.IsParked = core.Type == ProcessorCoreType.LowPower;

        // Auto pin & park cores that are not determined yet
        foreach (var core in Cores)
        {
            core.IsPinned ??= core.Type != ProcessorCoreType.LowPower;
            core.IsParked ??= core.Type == ProcessorCoreType.LowPower;
        }

        args.DaemonArgs.SetSplitLockMitigation = Tools.IsIntelCpu() && DisableSplitLockMitigation;

        // TODO: Notification system settings

        // TODO: Compositor Settings

        // TODO: Find and use GPU
        // lspci -k | grep -A3 -i 'VGA\|3D' shows this:
        /*
         00:02.0 VGA compatible controller: Intel Corporation CoffeeLake-H GT2 [UHD Graphics 630]
                   DeviceName: Intel(R) UHD Graphics 630
                   Subsystem: Hewlett-Packard Company Device 85fc
                   Kernel driver in use: i915
           --
           01:00.0 VGA compatible controller: NVIDIA Corporation TU117M [GeForce GTX 1650 Mobile / Max-Q] (rev a1)
                   Subsystem: Hewlett-Packard Company Device 85fc
                   Kernel driver in use: nvidia
                   Kernel modules: nouveau, nvidia_drm, nvidia
         */
        // Use line "Kernel driver in use" to get the driver name. If it's "nvidia" on an Nvidia GPU then it is ready to PRIME otherwise use DRI_PRIME or switcherooctl
        return args;
    }

    public override void SetAsDefault()
    {
        Strangle.SetAsDefault();
        MangoHUD.SetAsDefault();
        _notifications = true;
        _compositor = true;
        _amdPerfLevel = true;
        _blockScreenSaver = true;
        _cpuGovernor = true;
        _disableSplitLockMitigation = Tools.IsIntelCpu();
        _enableFanController = true;
        _enablePowerDaemon = true;
        _igpuGovernor = true;
        _igpuTreshold = 0.3F;
        _nvCoreClockOffset = 0;
        _nvMemClockOffset = 0;
        _nvPowerMizer = true;
        _optimizeGPU = true;
        _parkCores = false;
        _parkCoresAuto = true;
        _pinCores = false;
        _pinCoresAuto = true;
        _powerGovernor = true;
        _prioritize = true;
        _prioritizeIO = true;
        _softrealtime = false;
        _startupScriptPath = string.Empty;
        _startupScriptTimeout = 10;
        _stopScriptPath = string.Empty;
        _stopScriptTimeout = 10;
        _useGPU = true;
        _gpuID = 0;
    }

    public override event GamerunSettingSaveDelegate? OnSave;

    #endregion OVERRIDES

    #region PRIVATES

    private bool? _notifications;
    private bool? _compositor;
    private bool? _amdPerfLevel;
    private bool? _blockScreenSaver;
    private bool? _cpuGovernor;
    private bool? _disableSplitLockMitigation;
    private bool? _enableFanController;
    private bool? _enablePowerDaemon;
    private uint? _gpuID;
    private bool? _igpuGovernor;
    private float? _igpuTreshold; // default 0.3F, iGPU Watts / CPU Watts
    private uint? _nvCoreClockOffset;
    private uint? _nvMemClockOffset;
    private bool? _nvPowerMizer;
    private bool? _optimizeGPU;

    private bool? _parkCores;

    private bool? _parkCoresAuto;

    private bool? _pinCores;
    private bool? _pinCoresAuto;

    private bool? _powerGovernor;
    private bool? _prioritize;
    private bool? _prioritizeIO;

    private bool? _softrealtime;
    private string? _startupScriptPath;
    private uint? _startupScriptTimeout;
    private string? _stopScriptPath;
    private uint? _stopScriptTimeout;

    // ReSharper disable once InconsistentNaming
    private bool? _useGPU;

    #endregion PRIVATES
}