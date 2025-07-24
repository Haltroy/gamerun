using System;
using System.Collections.Generic;
using System.IO;

namespace Gamerun.Shared
{
    public class AppSettings : GamerunSettingsAbstract, ICloneable
    {
        private bool _amdPerfLevel = true;
        private bool _blockScreenSaver = true;
        private bool _cpuGovernor = true;
        private bool _disableSplitLockMitigation = true;
        private uint _gpuID;
        private bool _igpuGovernor = true;
        private float _igpuTreshold = 0.3F; // iGPU Watts / CPU Watts
        private uint _nvCoreClockOffset;
        private uint _nvMemClockOffset;
        private bool _nvPowerMizer = true;
        private bool _optimizeGPU = true;

        private bool _parkCores = true;

        private bool _parkCoresAuto = true;

        private Dictionary<uint, bool>
            _parkedCores; // TODO: Handle these init on constructor by adding the cores themselves.

        private bool _pinCores = true;
        private bool _pinCoresAuto = true;

        private Dictionary<uint, bool>
            _pinnedCores; // TODO: also figure out how to make this auto-save aka invoke OnSave()?

        private bool _powerGovernor = true;
        private bool _prioritize = true;
        private bool _prioritizeIO = true;

        private App? _shadowApp;

        private bool _softrealtime = true;
        private string? _startupScriptPath;
        private uint _startupScriptTimeout = 10;
        private string? _stopScriptPath;
        private uint _stopScriptTimeout = 10;

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

        public float iGPUTreshold
        {
            get => iGPUTreshold;
            set
            {
                _igpuTreshold = value;
                OnSave?.Invoke();
            }
        }

        public Dictionary<uint, bool> ParkedCores
        {
            get => _parkedCores;
            set
            {
                _parkedCores = value;
                OnSave?.Invoke();
            }
        }

        public Dictionary<uint, bool> PinnedCores
        {
            get => _pinnedCores;
            set
            {
                _pinnedCores = value;
                OnSave?.Invoke();
            }
        }

        public string StartupScriptPath
        {
            get => _startupScriptPath;
            set
            {
                _startupScriptPath = value;
                OnSave?.Invoke();
            }
        }

        public string StopScriptPath
        {
            get => _stopScriptPath;
            set
            {
                _stopScriptPath = value;
                OnSave?.Invoke();
            }
        }

        public bool AMDPerfLevel
        {
            get => _amdPerfLevel;
            set
            {
                _amdPerfLevel = value;
                OnSave?.Invoke();
            }
        }

        public bool BlockScreenSaver
        {
            get => _blockScreenSaver;
            set
            {
                _blockScreenSaver = value;
                OnSave?.Invoke();
            }
        }

        public bool CPUGovernor
        {
            get => _cpuGovernor;
            set
            {
                _cpuGovernor = value;
                OnSave?.Invoke();
            }
        }

        public bool DisableSplitLockMitigation
        {
            get => _disableSplitLockMitigation;
            set
            {
                _disableSplitLockMitigation = value;
                OnSave?.Invoke();
            }
        }

        public bool iGPUGovernor
        {
            get => _igpuGovernor;
            set
            {
                _igpuGovernor = value;
                OnSave?.Invoke();
            }
        }

        public bool NvPowerMizer
        {
            get => _nvPowerMizer;
            set
            {
                _nvPowerMizer = value;
                OnSave?.Invoke();
            }
        }

        public bool OptimizeGPU
        {
            get => _optimizeGPU;
            set
            {
                _optimizeGPU = value;
                OnSave?.Invoke();
            }
        }

        public bool ParkCores
        {
            get => _parkCores;
            set
            {
                _parkCores = value;
                OnSave?.Invoke();
            }
        }

        public bool ParkCoresAuto
        {
            get => _parkCoresAuto;
            set
            {
                _parkCoresAuto = value;
                OnSave?.Invoke();
            }
        }

        public bool PinCores
        {
            get => _pinCores;
            set
            {
                _pinCores = value;
                OnSave?.Invoke();
            }
        }

        public bool PinCoresAuto
        {
            get => _pinCoresAuto;
            set
            {
                _pinCoresAuto = value;
                OnSave?.Invoke();
            }
        }

        public bool PowerGovernor
        {
            get => _powerGovernor;
            set
            {
                _powerGovernor = value;
                OnSave?.Invoke();
            }
        }


        public bool SoftRealTime
        {
            get => _softrealtime;
            set
            {
                _softrealtime = value;
                OnSave?.Invoke();
            }
        }

        public uint StopScriptTimeout
        {
            get => _stopScriptTimeout;
            set
            {
                _stopScriptTimeout = value;
                OnSave?.Invoke();
            }
        }

        public uint GPUID
        {
            get => _gpuID;
            set
            {
                _gpuID = value;
                OnSave?.Invoke();
            }
        }

        public uint NvCoreClockOffset
        {
            get => _nvCoreClockOffset;
            set
            {
                _nvCoreClockOffset = value;
                OnSave?.Invoke();
            }
        }

        public uint NvMemClockOffset
        {
            get => _nvMemClockOffset;
            set
            {
                _nvMemClockOffset = value;
                OnSave?.Invoke();
            }
        }

        public uint StartupScriptTimeout
        {
            get => _startupScriptTimeout;
            set
            {
                _startupScriptTimeout = value;
                OnSave?.Invoke();
            }
        }

        public bool RequireRootPermissions => Prioritize || PrioritizeIO || OptimizeGPU; // TODO

        public object Clone()
        {
            // TODO
            throw new NotImplementedException();
        }

        public AppSettings CreateShadowCopy(App app)
        {
            _shadowApp = app;
            OnSave += ShadowCopySave;
            return this;
        }

        private void ShadowCopySave()
        {
            // TODO
            _shadowApp = null;
            OnSave -= ShadowCopySave;
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

        public override GamerunStartArguments GenerateArgs(GamerunStartArguments args)
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