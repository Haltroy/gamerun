using System.IO;
using Gamerun.Shared.Exceptions;

namespace Gamerun.Shared;

public class AppConfig : GamerunSettingsAbstract
{
    #region CONSTRUCTORS

    public AppConfig()
    {
        Cores = Tools.DetectCpuTopology();
        foreach (var cpu in Cores) cpu.ParkedPinnedChanged += () => OnSave?.Invoke();
    }

    #endregion CONSTRUCTORS

    #region PROPERTIES

    // ReSharper disable once InconsistentNaming
    public bool UseGPU
    {
        get => _useGPU ?? Gamerun.DefaultAppConfig.UseGPU;
        set
        {
            _useGPU = value;
            OnSave?.Invoke();
        }
    }

    public bool EnablePowerDaemon
    {
        get => _enablePowerDaemon ?? Gamerun.DefaultAppConfig.EnablePowerDaemon;
        set
        {
            _enablePowerDaemon = value;
            OnSave?.Invoke();
        }
    }

    public bool EnableFanController
    {
        get => _enableFanController ?? Gamerun.DefaultAppConfig.EnableFanController;
        set
        {
            _enableFanController = value;
            OnSave?.Invoke();
        }
    }

    public bool Prioritize
    {
        get => _prioritize ?? Gamerun.DefaultAppConfig.Prioritize;
        set
        {
            _prioritize = value;
            OnSave?.Invoke();
        }
    }

    public bool PrioritizeIO
    {
        get => _prioritizeIO ?? Gamerun.DefaultAppConfig.PrioritizeIO;
        set
        {
            _prioritizeIO = value;
            OnSave?.Invoke();
        }
    }

    public uint iGPUTreshold
    {
        get => _igpuTreshold ?? Gamerun.DefaultAppConfig.iGPUTreshold;
        set
        {
            _igpuTreshold = value;
            OnSave?.Invoke();
        }
    }

    public ProcessorCore[] Cores { get; set; }

    public string StartupScriptPath
    {
        get => _startupScriptPath ?? Gamerun.DefaultAppConfig.StartupScriptPath;
        set
        {
            _startupScriptPath = value;
            OnSave?.Invoke();
        }
    }

    public string StopScriptPath
    {
        get => _stopScriptPath ?? Gamerun.DefaultAppConfig.StopScriptPath;
        set
        {
            _stopScriptPath = value;
            OnSave?.Invoke();
        }
    }

    public bool AMDPerfLevel
    {
        get => _amdPerfLevel ?? Gamerun.DefaultAppConfig.AMDPerfLevel;
        set
        {
            _amdPerfLevel = value;
            OnSave?.Invoke();
        }
    }

    public bool BlockScreenSaver
    {
        get => _blockScreenSaver ?? Gamerun.DefaultAppConfig.BlockScreenSaver;
        set
        {
            _blockScreenSaver = value;
            OnSave?.Invoke();
        }
    }

    public bool CPUGovernor
    {
        get => _cpuGovernor ?? Gamerun.DefaultAppConfig.CPUGovernor;
        set
        {
            _cpuGovernor = value;
            OnSave?.Invoke();
        }
    }

    public bool DisableSplitLockMitigation
    {
        get => _disableSplitLockMitigation ?? Gamerun.DefaultAppConfig.DisableSplitLockMitigation;
        set
        {
            _disableSplitLockMitigation = value;
            OnSave?.Invoke();
        }
    }

    public bool iGPUGovernor
    {
        get => _igpuGovernor ?? Gamerun.DefaultAppConfig.iGPUGovernor;
        set
        {
            _igpuGovernor = value;
            OnSave?.Invoke();
        }
    }

    public bool NvPowerMizer
    {
        get => _nvPowerMizer ?? Gamerun.DefaultAppConfig.NvPowerMizer;
        set
        {
            _nvPowerMizer = value;
            OnSave?.Invoke();
        }
    }

    public bool OptimizeGPU
    {
        get => _optimizeGPU ?? Gamerun.DefaultAppConfig.OptimizeGPU;
        set
        {
            _optimizeGPU = value;
            OnSave?.Invoke();
        }
    }

    public bool ParkCores
    {
        get => _parkCores ?? Gamerun.DefaultAppConfig.ParkCores;
        set
        {
            _parkCores = value;
            OnSave?.Invoke();
        }
    }

    public bool ParkCoresAuto
    {
        get => _parkCoresAuto ?? Gamerun.DefaultAppConfig.ParkCoresAuto;
        set
        {
            _parkCoresAuto = value;
            OnSave?.Invoke();
        }
    }

    public bool PinCores
    {
        get => _pinCores ?? Gamerun.DefaultAppConfig.PinCores;
        set
        {
            _pinCores = value;
            OnSave?.Invoke();
        }
    }

    public bool PinCoresAuto
    {
        get => _pinCoresAuto ?? Gamerun.DefaultAppConfig.PinCoresAuto;
        set
        {
            _pinCoresAuto = value;
            OnSave?.Invoke();
        }
    }

    public bool PowerGovernor
    {
        get => _powerGovernor ?? Gamerun.DefaultAppConfig.PowerGovernor;
        set
        {
            _powerGovernor = value;
            OnSave?.Invoke();
        }
    }


    public bool SoftRealTime
    {
        get => _softrealtime ?? Gamerun.DefaultAppConfig.SoftRealTime;
        set
        {
            _softrealtime = value;
            OnSave?.Invoke();
        }
    }

    public uint StopScriptTimeout
    {
        get => _stopScriptTimeout ?? Gamerun.DefaultAppConfig.StopScriptTimeout;
        set
        {
            _stopScriptTimeout = value;
            OnSave?.Invoke();
        }
    }

    public uint GPUID
    {
        get => _gpuID ?? Gamerun.DefaultAppConfig.GPUID;
        set
        {
            _gpuID = value;
            OnSave?.Invoke();
        }
    }

    public uint NvCoreClockOffset
    {
        get => _nvCoreClockOffset ?? Gamerun.DefaultAppConfig.NvCoreClockOffset;
        set
        {
            _nvCoreClockOffset = value;
            OnSave?.Invoke();
        }
    }

    public uint NvMemClockOffset
    {
        get => _nvMemClockOffset ?? Gamerun.DefaultAppConfig.NvMemClockOffset;
        set
        {
            _nvMemClockOffset = value;
            OnSave?.Invoke();
        }
    }

    public uint StartupScriptTimeout
    {
        get => _startupScriptTimeout ?? Gamerun.DefaultAppConfig.StartupScriptTimeout;
        set
        {
            _startupScriptTimeout = value;
            OnSave?.Invoke();
        }
    }

    public bool DisableNotificationSystem
    {
        get => _notifications ?? Gamerun.DefaultAppConfig.DisableNotificationSystem;
        set
        {
            _notifications = value;
            OnSave?.Invoke();
        }
    }

    public bool OptimizeCompositor
    {
        get => _compositor ?? Gamerun.DefaultAppConfig.OptimizeCompositor;
        set
        {
            _compositor = value;
            OnSave?.Invoke();
        }
    }


    public override bool IsDefaults => _amdPerfLevel == null &&
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
    }

    public override void WriteSettings(Stream stream)
    {
        var buffer = (byte)(Prioritize ? 1 : 0);
        buffer += (byte)(UseGPU ? 2 : 0);
        // TODO
        stream.WriteByte(buffer);
    }

    public override GamerunStartArguments GenerateArgs(GamerunStartArguments args)
    {
        args.RequireDaemonUse = RequireRootPermissions;
        if (InitSystemHelper.IsServiceActiveAsync("power-profiles-daemon"))
        {
            args.StartDBusCalls.Add(new GamerunDBusCalls
            (
                "org.freedesktop.PowerProfiles",
                "/org/freedesktop/PowerProfiles",
                "org.freedesktop.PowerProfiles.SetActiveProfile",
                ["performance"]
            ));

            args.EndDBusCalls.Add(new GamerunDBusCalls
            (
                "org.freedesktop.PowerProfiles",
                "/org/freedesktop/PowerProfiles",
                "org.freedesktop.PowerProfiles.SetActiveProfile",
                ["balanced"]
            ));
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
            (
                "org.freedesktop.ScreenSaver",
                "/org/freedesktop/ScreenSaver",
                "org.freedesktop.ScreenSaver.Inhibit",
                ["Gamerun", Translations.Translations.AppIsRunning]
            ));
            args.StartDBusCalls.Add(new GamerunDBusCalls
            (
                "org.freedesktop.ScreenSaver",
                "/org/freedesktop/ScreenSaver",
                "org.freedesktop.ScreenSaver.UnInhibit",
                []
            ));
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

        if (DisableNotificationSystem)
        {
            // TODO: Notification system settings
        }

        if (OptimizeCompositor)
        {
            // TODO: Compositor Settings
        }

        var gpu = Gamerun.GPUs[GPUID];
        if (Gamerun.GPUs.Length > 1)
            switch (gpu.Driver.ToLower())
            {
                case "nvidia":
                    args.Environment["__NV_PRIME_RENDER_OFFLOAD"] = "1";
                    args.Environment["__VK_LAYER_NV_optimus"] = "NVIDIA_only";
                    args.Environment["__GLX_VENDOR_LIBRARY_NAME"] = "nvidia";
                    break;
                default:
                    args.Environment["DRI_PRIME"] = gpu.PciId;
                    break;
            }

        if (OptimizeGPU)
        {
            args.DaemonArgs.OptimizeGPU = true;
            switch (gpu.Driver.ToLower())
            {
                case "nvidia":
                    args.DaemonArgs.NvPowerMizer = true;
                    args.DaemonArgs.NvCoreClockOffset = NvCoreClockOffset;
                    args.DaemonArgs.NvMemClockOffset = NvMemClockOffset;
                    break;
                case "amdgpu":
                    args.DaemonArgs.AMDPerfLevel = true;
                    break;
                case "intel":
                case "xe":
                case "i915":
                    break;
            }
        }

        if (Gamerun.GPUs.Length > 1)
            if (iGPUGovernor)
            {
                args.DaemonArgs.iGPUGovernor = true;
                args.DaemonArgs.iGPUTreshold = iGPUTreshold;
            }

        return args;
    }

    internal override void SetAsDefault()
    {
        base.SetAsDefault();
        _notifications = true;
        _compositor = true;
        _amdPerfLevel = true;
        _blockScreenSaver = true;
        _cpuGovernor = true;
        _disableSplitLockMitigation = Tools.IsIntelCpu();
        _enableFanController = true;
        _enablePowerDaemon = true;
        _igpuGovernor = true;
        _igpuTreshold = 1200;
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

    private bool RequireRootPermissions => Prioritize
                                           || PrioritizeIO
                                           || OptimizeGPU
                                           || (EnablePowerDaemon && InitSystemHelper.IsServiceActiveAsync("tlp"))
                                           || ParkCores
                                           || PinCores
                                           || (Tools.IsIntelCpu() && DisableSplitLockMitigation)
                                           || SoftRealTime
                                           || OptimizeGPU;

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
    private uint? _igpuTreshold;
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