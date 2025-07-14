using System;
using System.IO;

namespace Gamerun.Shared
{
    public class AppSettings : GamerunSettingsAbstract
    {
        private bool _amdPerfLevel = true;
        private bool _blockScreenSaver = true;
        private bool _cpuGovernor = true;
        private bool _disableSplitLOckMitigation = true;
        private uint _gpuID;
        private bool _igpuGovernor = true;
        private float _igpuTreshold = 0.3F; // iGPU Watts / CPU Watts
        private uint _nvCoreClockOffset;
        private uint _nvMemClockOffset;
        private bool _nvPowerMizer = true;
        private bool _optimizeGPU = true;

        private bool _parkCores = true;
        private bool _pinCores = true;
        private bool _powerGovernor = true;
        private bool _prioritize = true;
        private bool _prioritizeIO = true;

        private bool _softrealtime = true;
        // TODO: Park & Pin cores list
        // TODO: Start and Stop script & script timeouts


        // ReSharper disable once InconsistentNaming
        private bool _useGPU = true;

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        public MangoHUDSettings MangoHUD { get; } = new MangoHUDSettings();

        // ReSharper disable once MemberCanBePrivate.Global
        public StrangleSettings Strangle { get; } = new StrangleSettings();

        // ReSharper disable once InconsistentNaming
        public bool UseGPU
        {
            get => _useGPU;
            set
            {
                _useGPU = value;
                OnSave?.Invoke();
            }
        }

        public bool Prioritize
        {
            get => _prioritize;
            set
            {
                _prioritize = value;
                OnSave?.Invoke();
            }
        }

        public bool PrioritizeIO
        {
            get => _prioritizeIO;
            set
            {
                _prioritizeIO = value;
                OnSave?.Invoke();
            }
        }

        public override void ReadSettings(Stream stream)
        {
            var bufferByte = stream.ReadByte();
            if (bufferByte == -1) throw new Exception(); // TODO
            _prioritize = Tools.IsBitSet(bufferByte, 0);
            _useGPU = Tools.IsBitSet(bufferByte, 1);
            Strangle.ReadSettings(stream);
            MangoHUD.ReadSettings(stream);
        }

        public override void WriteSettings(Stream stream)
        {
            var buffer = (byte)(Prioritize ? 1 : 0);
            buffer += (byte)(UseGPU ? 2 : 0);
            stream.WriteByte(buffer);
            Strangle.WriteSettings(stream);
            MangoHUD.WriteSettings(stream);
        }

        public override string GenerateCommand()
        {
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
            throw new NotImplementedException();
        }

        public override event GamerunSettingSaveDelegate? OnSave;
    }
}