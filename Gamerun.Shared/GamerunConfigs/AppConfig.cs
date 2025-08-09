using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gamerun.Shared.Exceptions;

namespace Gamerun.Shared;

public class AppConfig : GamerunConfigAbstract
{
    #region CONSTRUCTORS

    public AppConfig()
    {
        Cores = Tools.DetectCpuTopology();
        foreach (var cpu in Cores) cpu.ParkedPinnedChanged += () => OnSave?.Invoke();
    }

    #endregion CONSTRUCTORS

    #region PROPERTIES

    /// <summary>
    ///     Tells Gamerun to run application in a specific GPU.
    /// </summary>
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

    /// <summary>
    ///     Enables setting power daemon to performance when running the app.
    /// </summary>
    public bool EnablePowerDaemon
    {
        get => _enablePowerDaemon ?? Gamerun.DefaultAppConfig.EnablePowerDaemon;
        set
        {
            _enablePowerDaemon = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Enables setting fan control to high when running the app.
    /// </summary>
    public bool EnableFanController
    {
        get => _enableFanController ?? Gamerun.DefaultAppConfig.EnableFanController;
        set
        {
            _enableFanController = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Prioritizes the app in OS-level.
    /// </summary>
    public bool Prioritize
    {
        get => _prioritize ?? Gamerun.DefaultAppConfig.Prioritize;
        set
        {
            _prioritize = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Proitizes Input/Output for the app.
    /// </summary>
    public bool PrioritizeIO
    {
        get => _prioritizeIO ?? Gamerun.DefaultAppConfig.PrioritizeIO;
        set
        {
            _prioritizeIO = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Treshold for the integrated GPU.
    /// </summary>
    public uint iGPUTreshold
    {
        get => _igpuTreshold ?? Gamerun.DefaultAppConfig.iGPUTreshold;
        set
        {
            _igpuTreshold = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Processor cores. Used for parking and pinning cores.
    /// </summary>
    public ProcessorCore[] Cores { get; set; }

    /// <summary>
    ///     Path of the script to run before app launches.
    /// </summary>
    public string StartupScriptPath
    {
        get => _startupScriptPath ?? Gamerun.DefaultAppConfig.StartupScriptPath;
        set
        {
            _startupScriptPath = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Path of the script to run after app stopped.
    /// </summary>
    public string StopScriptPath
    {
        get => _stopScriptPath ?? Gamerun.DefaultAppConfig.StopScriptPath;
        set
        {
            _stopScriptPath = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Sets AMD GPU performance levels when running the app.
    /// </summary>
    public bool AMDPerfLevel
    {
        get => _amdPerfLevel ?? Gamerun.DefaultAppConfig.AMDPerfLevel;
        set
        {
            _amdPerfLevel = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Blocks screensaver when running the app.
    /// </summary>
    public bool BlockScreenSaver
    {
        get => _blockScreenSaver ?? Gamerun.DefaultAppConfig.BlockScreenSaver;
        set
        {
            _blockScreenSaver = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Sets CPU governor to performance when running the app.
    /// </summary>
    public bool CPUGovernor
    {
        get => _cpuGovernor ?? Gamerun.DefaultAppConfig.CPUGovernor;
        set
        {
            _cpuGovernor = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Disables Split Lock mitigation on ıntel CPUs when running apps to increase performance at the cot of security.
    /// </summary>
    public bool DisableSplitLockMitigation
    {
        get => _disableSplitLockMitigation ?? Gamerun.DefaultAppConfig.DisableSplitLockMitigation;
        set
        {
            _disableSplitLockMitigation = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Sets the governor ofr the integrated GPU to lower values to re-route all the power to the main GPU.
    /// </summary>
    public bool iGPUGovernor
    {
        get => _igpuGovernor ?? Gamerun.DefaultAppConfig.iGPUGovernor;
        set
        {
            _igpuGovernor = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Sets Nvidia's PowerMizer to performance on Nvidia GPUs.
    /// </summary>
    public bool NvPowerMizer
    {
        get => _nvPowerMizer ?? Gamerun.DefaultAppConfig.NvPowerMizer;
        set
        {
            _nvPowerMizer = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Determines if GPU optimization features to be applied or not.
    /// </summary>
    public bool OptimizeGPU
    {
        get => _optimizeGPU ?? Gamerun.DefaultAppConfig.OptimizeGPU;
        set
        {
            _optimizeGPU = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Determines if cores should be parked (disabled) or not.
    /// </summary>
    public bool ParkCores
    {
        get => _parkCores ?? Gamerun.DefaultAppConfig.ParkCores;
        set
        {
            _parkCores = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Determines if low-power cores should be parked (disabled) automatically or not.
    /// </summary>
    public bool ParkCoresAuto
    {
        get => _parkCoresAuto ?? Gamerun.DefaultAppConfig.ParkCoresAuto;
        set
        {
            _parkCoresAuto = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Determines if certain cores should be pinned to the app or not.
    /// </summary>
    public bool PinCores
    {
        get => _pinCores ?? Gamerun.DefaultAppConfig.PinCores;
        set
        {
            _pinCores = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Determines if certain cores should be marked for pinning to the app automatically or not.
    /// </summary>
    public bool PinCoresAuto
    {
        get => _pinCoresAuto ?? Gamerun.DefaultAppConfig.PinCoresAuto;
        set
        {
            _pinCoresAuto = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Determines the CPU power governor to performance when running the app.
    /// </summary>
    public bool PowerGovernor
    {
        get => _powerGovernor ?? Gamerun.DefaultAppConfig.PowerGovernor;
        set
        {
            _powerGovernor = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Determines if a custom scheduler should be used when running the app.
    /// </summary>
    public bool SoftRealTime
    {
        get => _softrealtime ?? Gamerun.DefaultAppConfig.SoftRealTime;
        set
        {
            _softrealtime = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Determines the timeout of script that runs when app is closed. Value of <c>0</c> means don't wait for script to
    ///     finish.
    /// </summary>
    public uint StopScriptTimeout
    {
        get => _stopScriptTimeout ?? Gamerun.DefaultAppConfig.StopScriptTimeout;
        set
        {
            _stopScriptTimeout = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Id of the GPU to use.
    /// </summary>
    public uint GPUID
    {
        get => _gpuID ?? Gamerun.DefaultAppConfig.GPUID;
        set
        {
            _gpuID = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Core clock offset of an Nvidia GPU, used for overclocking.
    /// </summary>
    public uint NvCoreClockOffset
    {
        get => _nvCoreClockOffset ?? Gamerun.DefaultAppConfig.NvCoreClockOffset;
        set
        {
            _nvCoreClockOffset = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Memory clock offset of an Nvidia GPU, used for overclocking.
    /// </summary>
    public uint NvMemClockOffset
    {
        get => _nvMemClockOffset ?? Gamerun.DefaultAppConfig.NvMemClockOffset;
        set
        {
            _nvMemClockOffset = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Determines the timeout of script that runs before app is started. Value of <c>0</c> means don't wait the script to
    ///     complete.
    /// </summary>
    public uint StartupScriptTimeout
    {
        get => _startupScriptTimeout ?? Gamerun.DefaultAppConfig.StartupScriptTimeout;
        set
        {
            _startupScriptTimeout = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Set notification system to "Do Not Disturb" mode when running the app.
    /// </summary>
    public bool DisableNotificationSystem
    {
        get => _notifications ?? Gamerun.DefaultAppConfig.DisableNotificationSystem;
        set
        {
            _notifications = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Optimizes compositor for performance when running the app.
    /// </summary>
    public bool OptimizeCompositor
    {
        get => _compositor ?? Gamerun.DefaultAppConfig.OptimizeCompositor;
        set
        {
            _compositor = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Determines if the script that runs before app starts should be waited or not.
    /// </summary>
    public bool StartScriptWait
    {
        get => _startScriptWait ?? Gamerun.DefaultAppConfig.StartScriptWait;
        set
        {
            _startScriptWait = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Determines if the script that runs after the app stops should be waited or not.
    /// </summary>
    public bool StopScriptWait
    {
        get => _stopScriptWait ?? Gamerun.DefaultAppConfig.StopScriptWait;
        set
        {
            _stopScriptWait = value;
            OnSave?.Invoke();
        }
    }

    #endregion PROPERTIES

    #region OVERRIDES

    public override byte CurrentVersion => 0;

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
                                       _useGPU == null &&
                                       _startScriptWait == null &&
                                       _stopScriptWait == null;

    public override void ReadSettings(Stream stream)
    {
        var bufferByte = stream.ReadByte();
        if (bufferByte == -1) throw new GamerunEndOfStreamException(stream.Position);
        if (bufferByte > CurrentVersion)
            throw new GamerunVersionNotSupportedException(bufferByte, CurrentVersion, nameof(AppConfig));
        var settingsLength = Settings.Length;
        var buffer = new byte[(int)Math.Ceiling((double)settingsLength / 8)];
        var bufferRead = stream.Read(buffer);
        if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
        var decoded = Tools.UnpackBytesToBools(buffer, settingsLength);
        _notifications = decoded[0];
        _compositor = decoded[1];
        _amdPerfLevel = decoded[2];
        _blockScreenSaver = decoded[3];
        _cpuGovernor = decoded[4];
        _disableSplitLockMitigation = decoded[5];
        _enableFanController = decoded[6];
        _enablePowerDaemon = decoded[7];
        _useGPU = decoded[8];
        _igpuGovernor = decoded[9];
        _nvPowerMizer = decoded[10];
        _optimizeGPU = decoded[11];
        _parkCores = decoded[12];
        _pinCores = decoded[13];
        _parkCoresAuto = decoded[14];
        _pinCoresAuto = decoded[15];
        _powerGovernor = decoded[16];
        _prioritize = decoded[17];
        _prioritizeIO = decoded[18];
        _softrealtime = decoded[19];
        _startScriptWait = decoded[20];
        _stopScriptWait = decoded[21];
        var gpuid_HasValue = decoded[22];
        var GPUID_IsVLE = decoded[23];
        var iGPUTreshold_HasValue = decoded[24];
        var iGPUTreshold_IsVLE = decoded[25];
        var NvCoreClockOffset_HasValue = decoded[26];
        var NvCoreClockOffset_IsVLE = decoded[27];
        var NvMemClockOffset_HasValue = decoded[28];
        var NvMemClockOffset_IsVLE = decoded[29];
        var StartupScriptTimeout_HasValue = decoded[30];
        var StartupScriptTimeout_IsVLE = decoded[31];
        var StopScriptTimeout_HasValue = decoded[32];
        var StopScriptTimeout_IsVLE = decoded[33];
        var StartupScriptPath_HasValue = decoded[34];
        var StopScriptPath_HasValue = decoded[35];
        var StartupScriptPath_IsVLE = decoded[36];
        var StopScriptPath_IsVLE = decoded[37];
        var core_HasValue = decoded[38];
        var core_IsVLE = decoded[39];

        if (gpuid_HasValue)
        {
            if (GPUID_IsVLE)
            {
                _gpuID = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _gpuID = BitConverter.ToUInt32(buffer);
            }
        }

        if (iGPUTreshold_HasValue)
        {
            if (iGPUTreshold_IsVLE)
            {
                _igpuTreshold = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _igpuTreshold = BitConverter.ToUInt32(buffer);
            }
        }

        if (NvCoreClockOffset_HasValue)
        {
            if (NvCoreClockOffset_IsVLE)
            {
                _nvCoreClockOffset = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _nvCoreClockOffset = BitConverter.ToUInt32(buffer);
            }
        }

        if (NvMemClockOffset_HasValue)
        {
            if (NvMemClockOffset_IsVLE)
            {
                _nvMemClockOffset = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _nvMemClockOffset = BitConverter.ToUInt32(buffer);
            }
        }

        if (StartupScriptTimeout_HasValue)
        {
            if (StartupScriptTimeout_IsVLE)
            {
                _startupScriptTimeout = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _startupScriptTimeout = BitConverter.ToUInt32(buffer);
            }
        }

        if (StopScriptTimeout_HasValue)
        {
            if (StopScriptTimeout_IsVLE)
            {
                _stopScriptTimeout = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _stopScriptTimeout = BitConverter.ToUInt32(buffer);
            }
        }

        if (core_HasValue)
        {
            var length = 0;
            if (core_IsVLE)
            {
                length = (int)Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                length = (int)BitConverter.ToUInt32(buffer);
            }

            buffer = new byte[length * 2];
            bufferRead = stream.Read(buffer);
            if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
            decoded = Tools.UnpackBytesToBools(buffer, length * 2);
            for (var i = 0; i < decoded.Length; i += 2)
            {
                Cores[i].IsParked = decoded[i];
                Cores[i].IsPinned = decoded[i + 1];
            }
        }

        if (StartupScriptPath_HasValue)
        {
            var length = 0;
            if (StartupScriptPath_IsVLE)
            {
                length = (int)Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                length = (int)BitConverter.ToUInt32(buffer);
            }

            buffer = new byte[length];
            bufferRead = stream.Read(buffer);
            if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
            _startupScriptPath = Encoding.UTF8.GetString(buffer);
        }

        if (!StopScriptPath_HasValue) return;

        var stopLength = 0;
        if (StopScriptPath_IsVLE)
        {
            stopLength = (int)Tools.DecodeVarUInt(stream);
        }
        else
        {
            buffer = new byte[sizeof(uint)];
            bufferRead = stream.Read(buffer);
            if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
            stopLength = (int)BitConverter.ToUInt32(buffer);
        }

        buffer = new byte[stopLength];
        bufferRead = stream.Read(buffer);
        if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
        _stopScriptPath = Encoding.UTF8.GetString(buffer);
    }

    public override void WriteSettings(Stream stream)
    {
        stream.WriteByte(CurrentVersion);
        var buffer = Tools.PackBoolsToBytes(Settings);
        stream.Write(buffer);

        if (GPUID > 0)
        {
            if (GPUID < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, GPUID);
            }
            else
            {
                buffer = BitConverter.GetBytes(GPUID);
                stream.Write(buffer);
            }
        }

        if (iGPUTreshold > 0)
        {
            if (iGPUTreshold < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, iGPUTreshold);
            }
            else
            {
                buffer = BitConverter.GetBytes(iGPUTreshold);
                stream.Write(buffer);
            }
        }

        if (NvCoreClockOffset > 0)
        {
            if (NvCoreClockOffset < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, NvCoreClockOffset);
            }
            else
            {
                buffer = BitConverter.GetBytes(NvCoreClockOffset);
                stream.Write(buffer);
            }
        }

        if (NvMemClockOffset > 0)
        {
            if (NvMemClockOffset < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, NvMemClockOffset);
            }
            else
            {
                buffer = BitConverter.GetBytes(NvMemClockOffset);
                stream.Write(buffer);
            }
        }

        if (StartupScriptTimeout > 0)
        {
            if (StartupScriptTimeout < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, StartupScriptTimeout);
            }
            else
            {
                buffer = BitConverter.GetBytes(StartupScriptTimeout);
                stream.Write(buffer);
            }
        }

        if (StopScriptTimeout > 0)
        {
            if (StopScriptTimeout < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, StopScriptTimeout);
            }
            else
            {
                buffer = BitConverter.GetBytes(StopScriptTimeout);
                stream.Write(buffer);
            }
        }

        if (Cores.Length > 0)
        {
            if (Cores.Length < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, (uint)Cores.Length);
            }
            else
            {
                buffer = BitConverter.GetBytes((uint)Cores.Length);
                stream.Write(buffer);
            }

            var corePinPark = new List<bool>();
            foreach (var core in Cores)
            {
                corePinPark.Add(core.IsParked ?? false);
                corePinPark.Add(core.IsPinned ?? false);
            }

            stream.Write(Tools.PackBoolsToBytes(corePinPark.ToArray()));
        }

        if (!string.IsNullOrWhiteSpace(StartupScriptPath))
        {
            var length = Encoding.UTF8.GetByteCount(StartupScriptPath);
            if (length < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, (uint)length);
            }
            else
            {
                buffer = BitConverter.GetBytes(length);
                stream.Write(buffer);
            }

            buffer = Encoding.UTF8.GetBytes(StartupScriptPath);
            stream.Write(buffer);
        }

        if (string.IsNullOrWhiteSpace(StopScriptPath)) return;
        var stopLength = Encoding.UTF8.GetByteCount(StopScriptPath);
        if (stopLength < Tools.VLEMaxSize)
        {
            Tools.WriteVarUInt(stream, (uint)stopLength);
        }
        else
        {
            buffer = BitConverter.GetBytes(stopLength);
            stream.Write(buffer);
        }

        buffer = Encoding.UTF8.GetBytes(StopScriptPath);
        stream.Write(buffer);
    }

    public override GamerunStartArguments GenerateArgs(GamerunStartArguments args)
    {
        args.RequireDaemonUse = RequireRootPermissions;
        args.StartScript = StartupScriptPath;
        args.EndScript = StopScriptPath;
        args.StartScriptTimeout = StartScriptWait ? (int)StartupScriptTimeout : -1;
        args.EndScriptTimeout = StopScriptWait ? (int)StopScriptTimeout : -1;
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

        if (DisableNotificationSystem) args = DoNotDisturbManager.GenerateArgs(args);

        if (OptimizeCompositor) args = CompositorManager.GenerateArgs(args);

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

        if (Gamerun.GPUs.Length <= 1 || !iGPUGovernor) return args;
        args.DaemonArgs.iGPUGovernor = true;
        args.DaemonArgs.iGPUTreshold = iGPUTreshold;

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
        _startScriptWait = true;
        _stopScriptPath = string.Empty;
        _stopScriptTimeout = 10;
        _stopScriptWait = false;
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

    private bool[] Settings =>
    [
        DisableNotificationSystem,
        OptimizeCompositor,
        AMDPerfLevel,
        BlockScreenSaver,
        CPUGovernor,
        DisableSplitLockMitigation,
        EnableFanController,
        EnablePowerDaemon,
        UseGPU,
        iGPUGovernor,
        NvPowerMizer,
        OptimizeGPU,
        ParkCores,
        PinCores,
        ParkCoresAuto,
        PinCoresAuto,
        PowerGovernor,
        Prioritize,
        PrioritizeIO,
        SoftRealTime,
        StartScriptWait,
        StopScriptWait,
        GPUID == 0,
        GPUID < Tools.VLEMaxSize,
        iGPUTreshold == 0,
        iGPUTreshold < Tools.VLEMaxSize,
        NvCoreClockOffset == 0,
        NvCoreClockOffset < Tools.VLEMaxSize,
        NvMemClockOffset == 0,
        NvMemClockOffset < Tools.VLEMaxSize,
        StartupScriptTimeout == 0,
        StartupScriptTimeout < Tools.VLEMaxSize,
        StopScriptTimeout == 0,
        StopScriptTimeout < Tools.VLEMaxSize,
        StartupScriptPath.Length == 0,
        StopScriptPath.Length == 0,
        Encoding.UTF8.GetByteCount(StartupScriptPath) < Tools.VLEMaxSize,
        Encoding.UTF8.GetByteCount(StopScriptPath) < Tools.VLEMaxSize,
        Cores.Length == 0,
        Cores.Length < Tools.VLEMaxSize
    ];


    // ReSharper disable once InconsistentNaming
    private bool? _useGPU;
    private bool? _notifications;
    private bool? _compositor;
    private bool? _amdPerfLevel;
    private bool? _blockScreenSaver;
    private bool? _cpuGovernor;
    private bool? _disableSplitLockMitigation;
    private bool? _enableFanController;
    private bool? _enablePowerDaemon;
    private bool? _igpuGovernor;
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
    private bool? _startScriptWait;
    private bool? _stopScriptWait;
    private uint? _gpuID;
    private uint? _igpuTreshold;
    private uint? _nvCoreClockOffset;
    private uint? _nvMemClockOffset;
    private uint? _startupScriptTimeout;
    private uint? _stopScriptTimeout;
    private string? _startupScriptPath;
    private string? _stopScriptPath;

    #endregion PRIVATES
}