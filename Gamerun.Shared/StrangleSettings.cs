using System;
using System.IO;

namespace Gamerun.Shared
{
    /// <summary>
    ///     LibStrangle settings. LibStrangle is a tool that limits frame rates in order to use less energy and for longevity.
    /// </summary>
    public class StrangleSettings : GamerunSettingsAbstract
    {
        #region Functions and Events

        public override string GenerateCommand()
        {
            // TODO: Convert this to Environment Variables
            return !Enabled
                ? ""
                : $"{Tools.GetCommand("strangle")} " +
                  $"{(VulkanOnly ? "-k" : "")} " +
                  $"{(VulkanOnly && Trilinear ? "-t" : "")} " +
                  $"{(NoDlsym ? "-n" : "")} " +
                  $"{(PICMIP != 0 ? $"-p {PICMIP}" : "")} " +
                  $"{(AnisotropicFiltering > 0 ? $"-a {AnisotropicFiltering}" : "")} " +
                  $"{(VSync > 0 ? $"-v {VSync - 1}" : "")}" +
                  $"{FPS}:{BatteryFPS}";
        }

        public override void ReadSettings(Stream stream)
        {
            var bufferByte = stream.ReadByte();
            int bufferRead;
            byte[] buffer;
            if (bufferByte == -1) throw new Exception(); // TODO
            _enabled = Tools.IsBitSet(bufferByte, 0);
            _vulkanOnly = Tools.IsBitSet(bufferByte, 1);
            _noDlsym = Tools.IsBitSet(bufferByte, 2);
            _trilinear = Tools.IsBitSet(bufferByte, 3);
            var afHasValue = Tools.IsBitSet(bufferByte, 4);
            var battFpsHasValue = Tools.IsBitSet(bufferByte, 5);
            var fpsHasValue = Tools.IsBitSet(bufferByte, 6);
            var vsyncHasValue = Tools.IsBitSet(bufferByte, 7);
            bufferByte = stream.ReadByte();
            if (bufferByte == -1) throw new Exception(); // TODO
            var afVLE = Tools.IsBitSet(bufferByte, 0);
            var battfpsVLE = Tools.IsBitSet(bufferByte, 0);
            var fpsVLE = Tools.IsBitSet(bufferByte, 0);
            var vsyncVLE = Tools.IsBitSet(bufferByte, 0);
            var picmipHasValue = Tools.IsBitSet(bufferByte, 0);
            var picmipVLE = Tools.IsBitSet(bufferByte, 0);
            var picmipNegative = Tools.IsBitSet(bufferByte, 0);

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
                    if (bufferRead != buffer.Length) throw new Exception(); // TODO
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
                    if (bufferRead != buffer.Length) throw new Exception(); // TODO
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
                    if (bufferRead != buffer.Length) throw new Exception(); // TODO
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
                    if (bufferRead != buffer.Length) throw new Exception(); // TODO
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
                if (bufferRead != buffer.Length) throw new Exception(); // TODO
                _picmip = BitConverter.ToInt32(buffer);
            }
        }

        public override void WriteSettings(Stream stream)
        {
            byte bufferByte = 0;
            byte[] buffer;
            bufferByte += (byte)(Enabled ? 1 : 0);
            bufferByte += (byte)(VulkanOnly ? 2 : 0);
            bufferByte += (byte)(NoDlsym ? 4 : 0);
            bufferByte += (byte)(Trilinear ? 8 : 0);
            bufferByte += (byte)(AnisotropicFiltering > 0 ? 16 : 0);
            bufferByte += (byte)(BatteryFPS > 0 ? 32 : 0);
            bufferByte += (byte)(FPS > 0 ? 64 : 0);
            bufferByte += (byte)(VSync > 0 ? 128 : 0);
            stream.WriteByte(bufferByte);
            bufferByte = 0;
            bufferByte += (byte)(AnisotropicFiltering < Tools.VLEMaxSize ? 1 : 0);
            bufferByte += (byte)(BatteryFPS < Tools.VLEMaxSize ? 2 : 0);
            bufferByte += (byte)(FPS < Tools.VLEMaxSize ? 4 : 0);
            bufferByte += (byte)(VSync < Tools.VLEMaxSize ? 8 : 0);
            bufferByte += (byte)(PICMIP != 0 ? 16 : 0);
            bufferByte += (byte)(Math.Abs(PICMIP) < Tools.VLEMaxSize ? 32 : 0);
            bufferByte += (byte)(PICMIP < 0 ? 64 : 0);
            stream.WriteByte(bufferByte);

            if (AnisotropicFiltering > 0)
            {
                if (AnisotropicFiltering < Tools.VLEMaxSize)
                {
                    Tools.WriteVarUInt(stream, AnisotropicFiltering);
                }
                else
                {
                    buffer = BitConverter.GetBytes(AnisotropicFiltering);
                    stream.Write(buffer, 0, buffer.Length);
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
                    stream.Write(buffer, 0, buffer.Length);
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
                    stream.Write(buffer, 0, buffer.Length);
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
                    stream.Write(buffer, 0, buffer.Length);
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
                stream.Write(buffer, 0, buffer.Length);
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
            get => _enabled;
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
            get => _batteryFps;
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
            get => _fps;
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
            get => _noDlsym;
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
            get => _trilinear;
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
            get => _vulkanOnly;
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
            get => _vSync;
            set
            {
                // TODO: more safety when setting this, this is only limiting vulkan
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
            get => _picmip;
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
            get => _anisotropicFiltering;
            set
            {
                _anisotropicFiltering = value > 16 ? 16 : value;
                OnSave?.Invoke();
            }
        }

        #endregion Properties

        #region PRIVATE FIELDS

        private uint _anisotropicFiltering;
        private uint _batteryFps;
        private bool _enabled;
        private uint _fps;
        private bool _noDlsym;
        private int _picmip;
        private bool _trilinear;
        private uint _vSync;
        private bool _vulkanOnly;

        #endregion PRIVATE FIELDS
    }
}