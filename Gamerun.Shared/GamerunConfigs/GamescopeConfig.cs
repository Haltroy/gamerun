using System;
using System.IO;
using System.Text;
using Gamerun.Shared.Exceptions;

namespace Gamerun.Shared;

public class GamescopeConfig : GamerunSettingsAbstract
{
    public override bool IsDefaults => _outputWidth == null &&
                                       _outputHeight == null &&
                                       _internalWidth == null &&
                                       _internalHeight == null &&
                                       _nestedWidth == null &&
                                       _nestedHeight == null &&
                                       _xwaylandCount == null &&
                                       _vulkanDevice == null &&
                                       _hideCursorDelay == null &&
                                       _cursorPath == null &&
                                       _linearUpscaling == null &&
                                       _hdrEnabled == null &&
                                       _enabled == null &&
                                       _adaptiveSync == null &&
                                       _composite == null &&
                                       _forceGrabCursor == null &&
                                       _debug == null &&
                                       _stats == null &&
                                       _timewarp == null &&
                                       _vkDebugLayers == null &&
                                       _steam == null &&
                                       _filter == null &&
                                       _backend == null &&
                                       _fullscreenMode == null;

    public override void ReadSettings(Stream stream)
    {
        var buffer = new byte[(int)Math.Ceiling((double)Settings.Length / 8)];
        var bufferRead = stream.Read(buffer);
        if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
        var decoded = Tools.UnpackBytesToBools(buffer, Settings.Length);
        LinearUpscaling = decoded[0];
        EnableHDR = decoded[1];
        Enabled = decoded[2];
        AdaptiveSync = decoded[3];
        Composite = decoded[4];
        ForceGrabCursor = decoded[5];
        Debug = decoded[6];
        Stats = decoded[7];
        Timewarp = decoded[8];
        VKDebugLayers = decoded[9];
        Steam = decoded[10];
        var OutputWidth_HasValue = decoded[11];
        var OutputHeight_HasValue = decoded[12];
        var InternalWidth_HasValue = decoded[13];
        var InternalHeight_HasValue = decoded[14];
        var NestedWidth_HasValue = decoded[15];
        var NestedHeight_HasValue = decoded[16];
        var XwaylandCount_HasValue = decoded[17];
        var VulkanDevice_HasValue = decoded[18];
        var HideCursorDelay_HasValue = decoded[19];
        var OutputWidth_IsVLE = decoded[20];
        var OutputHeight_IsVLE = decoded[21];
        var InternalWidth_IsVLE = decoded[22];
        var InternalHeight_IsVLE = decoded[23];
        var NestedWidth_IsVLE = decoded[24];
        var NestedHeight_IsVLE = decoded[25];
        var XwaylandCount_IsVLE = decoded[26];
        var VulkanDevice_IsVLE = decoded[27];
        var HideCursorDelay_IsVLE = decoded[28];
        var CursorPath_HasValue = decoded[29];
        var AdditionalArgs_HasValue = decoded[30];
        var CursorPath_IsVLE = decoded[31];
        var AdditionalArgs_IsVLE = decoded[32];

        if (OutputWidth_HasValue)
        {
            if (OutputWidth_IsVLE)
            {
                _outputWidth = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer, 0, buffer.Length);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _outputWidth = BitConverter.ToUInt32(buffer);
            }
        }

        if (OutputHeight_HasValue)
        {
            if (OutputHeight_IsVLE)
            {
                _outputHeight = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer, 0, buffer.Length);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _outputHeight = BitConverter.ToUInt32(buffer);
            }
        }

        if (InternalWidth_HasValue)
        {
            if (InternalWidth_IsVLE)
            {
                _internalWidth = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer, 0, buffer.Length);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _internalWidth = BitConverter.ToUInt32(buffer);
            }
        }

        if (InternalHeight_HasValue)
        {
            if (InternalHeight_IsVLE)
            {
                _internalHeight = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer, 0, buffer.Length);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _internalHeight = BitConverter.ToUInt32(buffer);
            }
        }

        if (NestedWidth_HasValue)
        {
            if (NestedWidth_IsVLE)
            {
                _nestedWidth = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer, 0, buffer.Length);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _nestedWidth = BitConverter.ToUInt32(buffer);
            }
        }

        if (NestedHeight_HasValue)
        {
            if (NestedHeight_IsVLE)
            {
                _nestedHeight = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer, 0, buffer.Length);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _nestedHeight = BitConverter.ToUInt32(buffer);
            }
        }

        if (XwaylandCount_HasValue)
        {
            if (XwaylandCount_IsVLE)
            {
                _xwaylandCount = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer, 0, buffer.Length);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _xwaylandCount = BitConverter.ToUInt32(buffer);
            }
        }

        if (VulkanDevice_HasValue)
        {
            if (VulkanDevice_IsVLE)
            {
                _vulkanDevice = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer, 0, buffer.Length);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _vulkanDevice = BitConverter.ToUInt32(buffer);
            }
        }

        if (HideCursorDelay_HasValue)
        {
            if (HideCursorDelay_IsVLE)
            {
                _hideCursorDelay = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer, 0, buffer.Length);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _hideCursorDelay = BitConverter.ToUInt32(buffer);
            }
        }

        if (CursorPath_HasValue)
        {
            int cursorPathSize;
            if (CursorPath_IsVLE)
            {
                cursorPathSize = (int)Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                cursorPathSize = (int)BitConverter.ToUInt32(buffer);
            }

            buffer = new byte[cursorPathSize];
            bufferRead = stream.Read(buffer);
            if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
            _cursorPath = Encoding.UTF8.GetString(buffer);
        }

        if (!AdditionalArgs_HasValue) return;
        int additionalArgsSize;
        if (AdditionalArgs_IsVLE)
        {
            additionalArgsSize = (int)Tools.DecodeVarUInt(stream);
        }
        else
        {
            buffer = new byte[sizeof(uint)];
            bufferRead = stream.Read(buffer);
            if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
            additionalArgsSize = (int)BitConverter.ToUInt32(buffer);
        }

        buffer = new byte[additionalArgsSize];
        bufferRead = stream.Read(buffer);
        if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
        _additionalArgs = Encoding.UTF8.GetString(buffer);
    }

    public override void WriteSettings(Stream stream)
    {
        var buffer = Tools.PackBoolsToBytes(Settings);
        stream.Write(buffer);

        if (OutputWidth > 0)
        {
            if (OutputWidth < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, OutputWidth);
            }
            else
            {
                buffer = BitConverter.GetBytes(OutputWidth);
                stream.Write(buffer);
            }
        }

        if (OutputHeight > 0)
        {
            if (OutputHeight < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, OutputHeight);
            }
            else
            {
                buffer = BitConverter.GetBytes(OutputHeight);
                stream.Write(buffer);
            }
        }

        if (InternalWidth > 0)
        {
            if (InternalWidth < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, InternalWidth);
            }
            else
            {
                buffer = BitConverter.GetBytes(InternalWidth);
                stream.Write(buffer);
            }
        }

        if (InternalHeight > 0)
        {
            if (InternalHeight < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, InternalHeight);
            }
            else
            {
                buffer = BitConverter.GetBytes(InternalHeight);
                stream.Write(buffer);
            }
        }

        if (NestedWidth > 0)
        {
            if (NestedWidth < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, NestedWidth);
            }
            else
            {
                buffer = BitConverter.GetBytes(NestedWidth);
                stream.Write(buffer);
            }
        }

        if (NestedHeight > 0)
        {
            if (NestedHeight < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, NestedHeight);
            }
            else
            {
                buffer = BitConverter.GetBytes(NestedHeight);
                stream.Write(buffer);
            }
        }

        if (XwaylandCount > 0)
        {
            if (XwaylandCount < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, XwaylandCount);
            }
            else
            {
                buffer = BitConverter.GetBytes(XwaylandCount);
                stream.Write(buffer);
            }
        }

        if (VulkanDevice > 0)
        {
            if (VulkanDevice < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, VulkanDevice);
            }
            else
            {
                buffer = BitConverter.GetBytes(VulkanDevice);
                stream.Write(buffer);
            }
        }

        if (HideCursorDelay > 0)
        {
            if (HideCursorDelay < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, HideCursorDelay);
            }
            else
            {
                buffer = BitConverter.GetBytes(HideCursorDelay);
                stream.Write(buffer);
            }
        }

        if (string.IsNullOrWhiteSpace(CursorPath))
        {
            var length = Encoding.UTF8.GetByteCount(CursorPath);
            if (length < Tools.VLEMaxSize)
                Tools.WriteVarUInt(stream, (uint)length);
            else
                stream.Write(BitConverter.GetBytes((uint)length));

            buffer = Encoding.UTF8.GetBytes(CursorPath);
            stream.Write(buffer);
        }

        if (!string.IsNullOrWhiteSpace(AdditionalArgs)) return;

        var additionalLength = Encoding.UTF8.GetByteCount(AdditionalArgs);
        if (additionalLength < Tools.VLEMaxSize)
            Tools.WriteVarUInt(stream, (uint)additionalLength);
        else
            stream.Write(BitConverter.GetBytes((uint)additionalLength));

        buffer = Encoding.UTF8.GetBytes(AdditionalArgs);
        stream.Write(buffer);
    }

    public override GamerunStartArguments GenerateArgs(GamerunStartArguments args)
    {
        var command = Tools.GetCommand("gamescope");
        switch (UseFullscreenMode)
        {
            case FullscreenMode.Fullscreen:
                command += " -f";
                break;
            case FullscreenMode.BorderlessFullscreen:
                command += " -b";
                break;
            case FullscreenMode.None:
            default:
                // do nothing
                break;
        }

        switch (UseBackend)
        {
            case Backend.Auto:
                command += " --backend auto";
                break;
            case Backend.DRM:
                command += " --backend drm";
                break;
            case Backend.SDL:
                command += " --backend sdl";
                break;
            case Backend.OpenVR:
                command += " --backend openvr";
                break;
            case Backend.Headless:
                command += " --backend headless";
                break;
            case Backend.Wayland:
                command += " --backend wayland";
                break;
        }

        switch (Filter)
        {
            case UpscalerFilter.FSR:
                command += " -F fsr";
                break;
            case UpscalerFilter.NIS:
                command += " -F nis";
                break;
            case UpscalerFilter.None:
            default:
                // do nothing
                break;
        }

        command += LinearUpscaling ? " --linear-upscaling" : "";
        command += EnableHDR ? " --hdr-enable" : "";
        command += AdaptiveSync ? " --adaptive-sync" : "";
        command += Composite ? " --composite" : "";
        command += ForceGrabCursor ? " --force-grab-cursor" : "";
        command += Debug ? " --debug" : "";
        command += Stats ? " --stats" : "";
        command += Timewarp ? " --timewarp" : "";
        command += VKDebugLayers ? " --vk-debug-layers" : "";
        command += Steam ? " --steam" : "";

        if (OutputWidth > 0 && OutputHeight > 0) command += $" -W {OutputWidth} -H {OutputHeight}";
        if (InternalWidth > 0 && InternalHeight > 0) command += $" -w {InternalWidth} -h {InternalHeight}";
        if (NestedWidth > 0 && NestedHeight > 0)
            command += $" --nested-width {NestedWidth} --nested-height {NestedHeight}";
        command += $" --prefer-vk-device {VulkanDevice}";
        command += $" --xwayland-count {XwaylandCount}";
        command += $" --hide-cursor-delay {HideCursorDelay}";
        if (string.IsNullOrWhiteSpace(CursorPath))
            command += $" --cursor {CursorPath.Replace("\"", "\\\"").Replace("\'", "\\\'")}";
        if (string.IsNullOrWhiteSpace(AdditionalArgs)) command += $" {AdditionalArgs}";

        command += " --";
        args.Prefix = command;
        return args;
    }

    internal override void SetAsDefault()
    {
        base.SetAsDefault();
        var monitor = Gamerun.GPUs[0].Modes.Split('x');
        if (uint.TryParse(monitor[0], out var w) && uint.TryParse(monitor[1], out var h))
        {
            _outputWidth = w;
            _outputHeight = h;
        }
        else
        {
            _outputWidth = 0;
            _outputHeight = 0;
        }

        _internalWidth = 0;
        _internalHeight = 0;
        _nestedWidth = 0;
        _nestedHeight = 0;
        _xwaylandCount = 1;
        _vulkanDevice = 0;
        _hideCursorDelay = 10;
        _cursorPath = string.Empty;
        _linearUpscaling = false;
        _hdrEnabled = true;
        _enabled = false;
        _adaptiveSync = false;
        _composite = false;
        _forceGrabCursor = true;
        _debug = false;
        _stats = false;
        _timewarp = false;
        _vkDebugLayers = false;
        _steam = false;
        _filter = UpscalerFilter.None;
        _backend = Backend.Auto;
        _fullscreenMode = FullscreenMode.Fullscreen;
    }

    public override event GamerunSettingSaveDelegate? OnSave;

    #region PRIVATES

    private uint? _outputWidth;
    private uint? _outputHeight;
    private uint? _internalWidth;
    private uint? _internalHeight;
    private uint? _nestedWidth;
    private uint? _nestedHeight;
    private uint? _xwaylandCount;
    private uint? _vulkanDevice;
    private uint? _hideCursorDelay;
    private string? _cursorPath;
    private string? _additionalArgs;
    private bool? _linearUpscaling;
    private bool? _hdrEnabled;
    private bool? _enabled;
    private bool? _adaptiveSync;
    private bool? _composite;
    private bool? _forceGrabCursor;
    private bool? _debug;
    private bool? _stats;
    private bool? _timewarp;
    private bool? _vkDebugLayers;
    private bool? _steam;
    private UpscalerFilter? _filter;
    private Backend? _backend;
    private FullscreenMode? _fullscreenMode;

    private bool[] Settings =>
    [
        LinearUpscaling,
        EnableHDR,
        Enabled,
        AdaptiveSync,
        Composite,
        ForceGrabCursor,
        Debug,
        Stats,
        Timewarp,
        VKDebugLayers,
        Steam,
        OutputWidth == 0,
        OutputHeight == 0,
        InternalWidth == 0,
        InternalHeight == 0,
        NestedWidth == 0,
        NestedHeight == 0,
        XwaylandCount == 0,
        VulkanDevice == 0,
        HideCursorDelay == 0,
        OutputWidth < Tools.VLEMaxSize,
        OutputHeight < Tools.VLEMaxSize,
        InternalWidth < Tools.VLEMaxSize,
        InternalHeight < Tools.VLEMaxSize,
        NestedWidth < Tools.VLEMaxSize,
        NestedHeight < Tools.VLEMaxSize,
        XwaylandCount < Tools.VLEMaxSize,
        VulkanDevice < Tools.VLEMaxSize,
        HideCursorDelay < Tools.VLEMaxSize,
        CursorPath.Length > 0,
        AdditionalArgs.Length > 0,
        Encoding.UTF8.GetByteCount(CursorPath) < Tools.VLEMaxSize,
        Encoding.UTF8.GetByteCount(AdditionalArgs) < Tools.VLEMaxSize
    ];

    #endregion PRIVATES

    #region PUBLIC PROPERTIES

    public uint OutputWidth
    {
        get => _outputWidth ?? Gamerun.DefaultGamescopeConfig.OutputWidth;
        set
        {
            _outputWidth = value;
            OnSave?.Invoke();
        }
    }

    public uint OutputHeight
    {
        get => _outputHeight ?? Gamerun.DefaultGamescopeConfig.OutputHeight;
        set
        {
            _outputHeight = value;
            OnSave?.Invoke();
        }
    }

    public uint InternalWidth
    {
        get => _internalWidth ?? Gamerun.DefaultGamescopeConfig.InternalWidth;
        set
        {
            _internalWidth = value;
            OnSave?.Invoke();
        }
    }

    public uint InternalHeight
    {
        get => _internalHeight ?? Gamerun.DefaultGamescopeConfig.InternalHeight;
        set
        {
            _internalHeight = value;
            OnSave?.Invoke();
        }
    }

    public uint NestedWidth
    {
        get => _nestedWidth ?? Gamerun.DefaultGamescopeConfig.NestedWidth;
        set
        {
            _nestedWidth = value;
            OnSave?.Invoke();
        }
    }

    public uint NestedHeight
    {
        get => _nestedHeight ?? Gamerun.DefaultGamescopeConfig.NestedHeight;
        set
        {
            _nestedHeight = value;
            OnSave?.Invoke();
        }
    }

    public uint XwaylandCount
    {
        get => _xwaylandCount ?? Gamerun.DefaultGamescopeConfig.XwaylandCount;
        set
        {
            _xwaylandCount = value;
            OnSave?.Invoke();
        }
    }

    public uint VulkanDevice
    {
        get => _vulkanDevice ?? Gamerun.DefaultGamescopeConfig.VulkanDevice;
        set
        {
            _vulkanDevice = value;
            OnSave?.Invoke();
        }
    }

    public uint HideCursorDelay
    {
        get => _hideCursorDelay ?? Gamerun.DefaultGamescopeConfig.HideCursorDelay;
        set
        {
            _hideCursorDelay = value;
            OnSave?.Invoke();
        }
    }


    public UpscalerFilter Filter
    {
        get => _filter ?? Gamerun.DefaultGamescopeConfig.Filter;
        set
        {
            _filter = value;
            OnSave?.Invoke();
        }
    }

    public Backend UseBackend
    {
        get => _backend ?? Gamerun.DefaultGamescopeConfig.UseBackend;
        set
        {
            _backend = value;
            OnSave?.Invoke();
        }
    }

    public FullscreenMode UseFullscreenMode
    {
        get => _fullscreenMode ?? Gamerun.DefaultGamescopeConfig.UseFullscreenMode;
        set
        {
            _fullscreenMode = value;
            OnSave?.Invoke();
        }
    }

    public string CursorPath
    {
        get => _cursorPath ?? Gamerun.DefaultGamescopeConfig.CursorPath;
        set
        {
            _cursorPath = value;
            OnSave?.Invoke();
        }
    }

    public string AdditionalArgs
    {
        get => _additionalArgs ?? Gamerun.DefaultGamescopeConfig.AdditionalArgs;
        set
        {
            _additionalArgs = value;
            OnSave?.Invoke();
        }
    }

    public bool LinearUpscaling
    {
        get => _linearUpscaling ?? Gamerun.DefaultGamescopeConfig.LinearUpscaling;
        set
        {
            _linearUpscaling = value;
            OnSave?.Invoke();
        }
    }

    public bool EnableHDR
    {
        get => _hdrEnabled ?? Gamerun.DefaultGamescopeConfig.EnableHDR;
        set
        {
            _hdrEnabled = value;
            OnSave?.Invoke();
        }
    }

    public bool Enabled
    {
        get => _enabled ?? Gamerun.DefaultGamescopeConfig.Enabled;
        set
        {
            _enabled = value;
            OnSave?.Invoke();
        }
    }

    public bool AdaptiveSync
    {
        get => _adaptiveSync ?? Gamerun.DefaultGamescopeConfig.AdaptiveSync;
        set
        {
            _adaptiveSync = value;
            OnSave?.Invoke();
        }
    }

    public bool Composite
    {
        get => _composite ?? Gamerun.DefaultGamescopeConfig.Composite;
        set
        {
            _composite = value;
            OnSave?.Invoke();
        }
    }

    public bool ForceGrabCursor
    {
        get => _forceGrabCursor ?? Gamerun.DefaultGamescopeConfig.ForceGrabCursor;
        set
        {
            _forceGrabCursor = value;
            OnSave?.Invoke();
        }
    }

    public bool Debug
    {
        get => _debug ?? Gamerun.DefaultGamescopeConfig.Debug;
        set
        {
            _debug = value;
            OnSave?.Invoke();
        }
    }

    public bool Stats
    {
        get => _stats ?? Gamerun.DefaultGamescopeConfig.Stats;
        set
        {
            _stats = value;
            OnSave?.Invoke();
        }
    }

    public bool Timewarp
    {
        get => _timewarp ?? Gamerun.DefaultGamescopeConfig.Timewarp;
        set
        {
            _timewarp = value;
            OnSave?.Invoke();
        }
    }

    public bool VKDebugLayers
    {
        get => _vkDebugLayers ?? Gamerun.DefaultGamescopeConfig.VKDebugLayers;
        set
        {
            _vkDebugLayers = value;
            OnSave?.Invoke();
        }
    }

    public bool Steam
    {
        get => _steam ?? Gamerun.DefaultGamescopeConfig.Steam;
        set
        {
            _steam = value;
            OnSave?.Invoke();
        }
    }

    #endregion PUBLIC PROPERTIES

    #region ENUMS

    /// <summary>
    ///     Upscale filters to use.
    /// </summary>
    public enum UpscalerFilter
    {
        /// <summary>
        ///     Don't upscale.
        /// </summary>
        None,

        /// <summary>
        ///     Uses AMD FidelityFX Super Resolution.
        /// </summary>
        FSR,

        /// <summary>
        ///     Uses Nvidia Image Gen (not DLSS).
        /// </summary>
        NIS
    }

    /// <summary>
    ///     Backends to use.
    /// </summary>
    public enum Backend
    {
        /// <summary>
        ///     Let Gamescope auto-detect the backend to use.
        /// </summary>
        Auto,

        /// <summary>
        ///     Uses Direct Rendering Mode.
        /// </summary>
        DRM,

        /// <summary>
        ///     Uses Simple DirectMedia Layer.
        /// </summary>
        SDL,

        /// <summary>
        ///     Uses OpenVR.
        /// </summary>
        OpenVR,

        /// <summary>
        ///     Make a headless session with no display.
        /// </summary>
        Headless,

        /// <summary>
        ///     Uses Wayland.
        /// </summary>
        Wayland
    }

    /// <summary>
    ///     Fullscreen modes to use.
    /// </summary>
    public enum FullscreenMode
    {
        /// <summary>
        ///     Doesn't uses fullscreen mode. Windowed.
        /// </summary>
        None,

        /// <summary>
        ///     Uses fullscreen mode.
        /// </summary>
        Fullscreen,

        /// <summary>
        ///     Uses borderless fullscreen mode. Makes a window but set it as borderless to imitate a fullscreen.
        /// </summary>
        BorderlessFullscreen
    }

    #endregion ENUMS
}