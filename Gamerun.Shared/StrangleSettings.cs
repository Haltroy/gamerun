using System;
using System.IO;
using Gamerun.Shared.Exceptions;

namespace Gamerun.Shared;

/// <summary>
///     LibStrangle settings. LibStrangle is a tool that limits frame rates in order to use less energy and for longevity
///     of hardware as well as making less noise.
/// </summary>
public class StrangleSettings : GamerunSettingsAbstract
{
    #region PRIVATE PROPERTIES

    private bool[] Settings =>
    [
        Enabled,
        VulkanOnly,
        NoDlsym,
        Trilinear,
        AnisotropicFiltering > 0,
        BatteryFPS > 0,
        FPS > 0,
        VSync > 0,
        AnisotropicFiltering < Tools.VLEMaxSize,
        BatteryFPS < Tools.VLEMaxSize,
        FPS < Tools.VLEMaxSize,
        VSync < Tools.VLEMaxSize,
        PICMIP != 0,
        Math.Abs(PICMIP) < Tools.VLEMaxSize,
        PICMIP < 0,
        GLFinish,
        Bicubic,
        Retro
    ];

    #endregion PRIVATE PROPERTIES

    #region Functions and Events

    public override GamerunStartArguments GenerateArgs(GamerunStartArguments args)
    {
        if (!Enabled)
        {
            args.Environment["DISABLE_VK_LAYER_TORKEL104_libstrangle"] = "1";
            return args;
        }

        args.Environment["ENABLE_VK_LAYER_TORKEL104_libstrangle"] = "1";
        if (VulkanOnly) args.Environment["STRANGLE_VKONLY"] = "1";
        if (VulkanOnly && Trilinear) args.Environment["STRANGLE_TRILINEAR"] = "1";
        if (VulkanOnly && Bicubic) args.Environment["STRANGLE_BICUBIC"] = "1";
        if (VulkanOnly && Retro) args.Environment["STRANGLE_RETRO"] = "1";
        if (GLFinish) args.Environment["STRANGLE_GLFINISH"] = "1";
        if (NoDlsym) args.Environment["STRANGLE_NODLSYM"] = "1";
        if (PICMIP != 0) args.Environment["STRANGLE_PICMIP"] = PICMIP.ToString();
        if (AnisotropicFiltering > 0) args.Environment["STRANGLE_AF"] = AnisotropicFiltering.ToString();
        if (VSync > 0) args.Environment["STRANGLE_VSYNC"] = (VSync - 1).ToString();
        args.Environment["STRANGLE_FPS"] = FPS.ToString();
        args.Environment["STRANGLE_FPS_BATTERY"] = BatteryFPS.ToString();
        return args;
    }

    public override void SetAsDefault()
    {
        _vulkanOnly = false;
        _enabled = true;
        _trilinear = false;
        _noDlsym = false;
        _picmip = 0;
        _anisotropicFiltering = 0;
        _vSync = 0;
        _fps = 60;
        _batteryFps = _fps / 2;
        _glfinish = false;
        _bicubic = false;
        _retro = false;
    }

    public override bool IsDefaults => _vulkanOnly == null && _enabled == null && _trilinear == null &&
                                       _noDlsym == null && _picmip == null && _anisotropicFiltering == null &&
                                       _vSync == null && _fps == null && _batteryFps == null && _glfinish == null &&
                                       _bicubic == null && _retro == null;

    public override void ReadSettings(Stream stream)
    {
        var settingLength = Settings.Length;
        var buffer = new byte[(int)Math.Ceiling((double)settingLength / 8)];
        var bufferRead = stream.Read(buffer);
        if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
        var decoded = Tools.UnpackBytesToBools(buffer, settingLength);
        _enabled = decoded[0];
        _vulkanOnly = decoded[1];
        _noDlsym = decoded[2];
        _trilinear = decoded[3];
        var afHasValue = decoded[4];
        var battFpsHasValue = decoded[5];
        var fpsHasValue = decoded[6];
        var vsyncHasValue = decoded[7];
        var afVLE = decoded[8];
        var battfpsVLE = decoded[9];
        var fpsVLE = decoded[10];
        var vsyncVLE = decoded[11];
        var picmipHasValue = decoded[12];
        var picmipVLE = decoded[13];
        var picmipNegative = decoded[14];
        _glfinish = decoded[15];
        _bicubic = decoded[16];
        _retro = decoded[17];

        if (afHasValue)
        {
            if (afVLE)
            {
                _anisotropicFiltering = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer, 0, buffer.Length);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _anisotropicFiltering = BitConverter.ToUInt32(buffer);
            }
        }

        if (battFpsHasValue)
        {
            if (battfpsVLE)
            {
                _batteryFps = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer, 0, buffer.Length);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _batteryFps = BitConverter.ToUInt32(buffer);
            }
        }

        if (fpsHasValue)
        {
            if (fpsVLE)
            {
                _fps = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer, 0, buffer.Length);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _fps = BitConverter.ToUInt32(buffer);
            }
        }

        if (vsyncHasValue)
        {
            if (vsyncVLE)
            {
                _vSync = Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer, 0, buffer.Length);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                _vSync = BitConverter.ToUInt32(buffer);
            }
        }

        if (!picmipHasValue) return;
        if (picmipVLE)
        {
            _picmip = (int)((picmipNegative ? -1 : 1) * Tools.DecodeVarUInt(stream));
        }
        else
        {
            buffer = new byte[sizeof(int)];
            bufferRead = stream.Read(buffer, 0, buffer.Length);
            if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
            _picmip = BitConverter.ToInt32(buffer);
        }
    }

    public override void WriteSettings(Stream stream)
    {
        var buffer = Tools.PackBoolsToBytes(Settings);
        stream.Write(buffer);

        if (AnisotropicFiltering > 0)
        {
            if (AnisotropicFiltering < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, AnisotropicFiltering);
            }
            else
            {
                buffer = BitConverter.GetBytes(AnisotropicFiltering);
                stream.Write(buffer);
            }
        }

        if (BatteryFPS > 0)
        {
            if (BatteryFPS < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, BatteryFPS);
            }
            else
            {
                buffer = BitConverter.GetBytes(BatteryFPS);
                stream.Write(buffer);
            }
        }

        if (FPS > 0)
        {
            if (FPS < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, FPS);
            }
            else
            {
                buffer = BitConverter.GetBytes(FPS);
                stream.Write(buffer);
            }
        }

        if (VSync > 0)
        {
            if (VSync < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, VSync);
            }
            else
            {
                buffer = BitConverter.GetBytes(VSync);
                stream.Write(buffer);
            }
        }

        // Since the sign is saved somewhere else we can just convert it to an unsigned number safely and save it
        if (PICMIP == 0) return;
        var uintPicmip = (uint)Math.Abs(PICMIP);
        if (uintPicmip < Tools.VLEMaxSize)
        {
            Tools.WriteVarUInt(stream, uintPicmip);
        }
        else
        {
            buffer = BitConverter.GetBytes(uintPicmip);
            stream.Write(buffer);
        }
    }

    public override event GamerunSettingSaveDelegate? OnSave;

    #endregion Functions and Events

    #region Properties

    /// <summary>
    ///     Determines if LibStrangle should be used or not.
    /// </summary>
    public bool Enabled
    {
        get => _enabled ?? Gamerun.Default.Strangle.Enabled;
        set
        {
            _enabled = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Determines the FPS value to limit on battery. Value of 0 disables FPS limiting.
    /// </summary>

    // ReSharper disable once InconsistentNaming
    public uint BatteryFPS
    {
        get => _batteryFps ?? Gamerun.Default.Strangle.BatteryFPS;
        set
        {
            _batteryFps = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Determines the FPS value to limit. Value of 0 disables FPS limiting.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public uint FPS
    {
        get => _fps ?? Gamerun.Default.Strangle.FPS;
        set
        {
            _fps = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Disable Dlsym hijacking in the OpenGL library.
    /// </summary>

    public bool NoDlsym
    {
        get => _noDlsym ?? Gamerun.Default.Strangle.NoDlsym;
        set
        {
            _noDlsym = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Forces trilinear filtering (Only works on Vulkan).
    /// </summary>

    public bool Trilinear
    {
        get => _trilinear ?? Gamerun.Default.Strangle.Trilinear;
        set
        {
            _trilinear = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Prevents OpenGL libraries from loading.
    /// </summary>
    public bool VulkanOnly
    {
        get => _vulkanOnly ?? Gamerun.Default.Strangle.VulkanOnly;
        set
        {
            _vulkanOnly = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Determines the VSync mode.
    ///     <para />
    ///     For OpenGL:
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Value</term>
    ///             <description>OpenGL mode</description>
    ///         </listheader>
    ///         <item>
    ///             <term>0</term>
    ///             <description>Disables setting Vsync</description>
    ///         </item>
    ///         <item>
    ///             <term>1</term>
    ///             <description>Force off</description>
    ///         </item>
    ///         <item>
    ///             <term>2</term>
    ///             <description>Force on</description>
    ///         </item>
    ///         <item>
    ///             <term>n</term>
    ///             <description>Sync to refresh rate / n - 1</description>
    ///         </item>
    ///     </list>
    ///     <para />
    ///     For Vulkan:
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Value</term>
    ///             <description>Vulkan mode</description>
    ///         </listheader>
    ///         <item>
    ///             <term>0</term>
    ///             <description>Disables setting VSync</description>
    ///         </item>
    ///         <item>
    ///             <term>1</term>
    ///             <description>Force off</description>
    ///         </item>
    ///         <item>
    ///             <term>2</term>
    ///             <description>Mailbox mode. Uncapped framerate</description>
    ///         </item>
    ///         <item>
    ///             <term>3</term>
    ///             <description>Traditional mode. Capped framerate</description>
    ///         </item>
    ///         <item>
    ///             <term>4</term>
    ///             <description>Adaptive mode. Tearing at lower frames.</description>
    ///         </item>
    ///     </list>
    ///     <para />
    ///     NOTE: On Vulkan, if the value is bigger than 4, it will force off VSync if
    ///     FPS is zero and run in adaptive mode if it's not zero.
    /// </summary>

    public uint VSync
    {
        get => _vSync ?? Gamerun.Default.Strangle.VSync;
        set
        {
            _vSync = VulkanOnly && value > 4 ? (uint)(FPS > 0 ? 4 : 0) : value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Set the mipmap LoD bias to PICMIP. A higher value means blurrier textures. Ranges from -16 to 16.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public int PICMIP
    {
        get => _picmip ?? Gamerun.Default.Strangle.PICMIP;
        set
        {
            if (value < -16) _picmip = -16;
            else if (value > 16) _picmip = 16;
            else _picmip = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Sets the anisotropic filtering level. Makes textures sharper at an angle. Limited OpenGL support. Ranges from 1 to
    ///     16. (0 disables it)
    /// </summary>
    public uint AnisotropicFiltering
    {
        get => _anisotropicFiltering ?? Gamerun.Default.Strangle.AnisotropicFiltering;
        set
        {
            _anisotropicFiltering = value > 16 ? 16 : value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Forces glFinish() to run after every frame. OpenGL only.
    /// </summary>
    public bool GLFinish
    {
        get => _glfinish ?? Gamerun.Default.Strangle.GLFinish;
        set
        {
            _glfinish = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Disables linear texture filtering. Makes textures look blocky. Vulkan only.
    /// </summary>
    public bool Retro
    {
        get => _retro ?? Gamerun.Default.Strangle.Retro;
        set
        {
            _retro = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Forces Bicubic filtering. Vulkan only.
    /// </summary>
    public bool Bicubic
    {
        get => _bicubic ?? Gamerun.Default.Strangle.Bicubic;
        set
        {
            _bicubic = value;
            OnSave?.Invoke();
        }
    }

    #endregion Properties

    #region PRIVATE FIELDS

    private uint? _anisotropicFiltering;
    private uint? _batteryFps;
    private bool? _enabled;
    private uint? _fps;
    private bool? _noDlsym;
    private int? _picmip;
    private bool? _trilinear;
    private uint? _vSync;
    private bool? _vulkanOnly;
    private bool? _glfinish;
    private bool? _bicubic;
    private bool? _retro;

    #endregion PRIVATE FIELDS
}