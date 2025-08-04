using System;
using System.IO;
using Gamerun.Shared.Exceptions;

namespace Gamerun.Shared
{
    /// <summary>
    /// Arguments that are passed by "gamerun /path/to/app" to "gamerun --dameon".
    /// </summary>
    public class GamerunDaemonArgs
    {
        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new GamerunDaemonArgs from a specific Base64 string. used by "gamerun --daemon".
        /// </summary>
        /// <param name="base64">Base64 string that contains the required information.</param>
        /// <exception cref="GamerunEndOfStreamException">Thrown when the decoded Base64 string has holes inside of it.</exception>
        public GamerunDaemonArgs(string base64)
        {
            using var memStream = new MemoryStream(Convert.FromBase64String(base64));
            var settingsLength = Settings.Length;
            byte[] buffer = new byte[(int)Math.Ceiling((double)settingsLength / 8)];
            var bufferRead = memStream.Read(buffer);
            if (bufferRead != settingsLength)  throw new GamerunEndOfStreamException(memStream.Position);
            var decoded = Tools.UnpackBytesToBools(buffer, settingsLength);
            SetTLP = decoded[0];
            Prioritize = decoded[1];
            PrioritizeIO = decoded[2];
            ParkCores = decoded[3];
            PinCores = decoded[4];
            OptimizeGPU = decoded[5];
            SetSplitLockMitigation = decoded[6];
            SoftRealtime = decoded[7];
            iGPUGovernor = decoded[8];
            NvPowerMizer = decoded[9];
            AMDPerfLevel = decoded[10];
            var _nvMemHasValue = decoded[11];
            var _nvCoreHasValue = decoded[12];
            var _nvMemIsVLE = decoded[13];
            var _nvCoreIsVLE = decoded[14];
            var _pidHasValue = decoded[15];
            var _pidIsVLE = decoded[16];
            var _pidSign = decoded[17];
            var _igpuTresholdHasValue = decoded[18];
            var _parkedCoresHasValue = decoded[19];
            var _parkedCoresVLE = decoded[20];
            var _pinnedCoresHasValue = decoded[21];
            var _pinnedCoresVLE = decoded[22];
            if (_nvMemHasValue)
            {
                if (_nvMemIsVLE)
                {
                    NvMemClockOffset = Tools.DecodeVarUInt(memStream);
                }
                else
                {
                    buffer = new byte[sizeof(uint)];
                    bufferRead = memStream.Read(buffer);
                    if (bufferRead != buffer.Length) throw new  GamerunEndOfStreamException(memStream.Position);
                    NvMemClockOffset = BitConverter.ToUInt32(buffer);
                }
            }

            if (_nvCoreHasValue)
            {
                if (_nvCoreIsVLE)
                {
                    NvCoreClockOffset = Tools.DecodeVarUInt(memStream);
                }
                else
                {
                    buffer = new byte[sizeof(uint)];
                    bufferRead = memStream.Read(buffer);
                    if (bufferRead != buffer.Length) throw new  GamerunEndOfStreamException(memStream.Position);
                    NvCoreClockOffset = BitConverter.ToUInt32(buffer);
                }
            }

            if (_igpuTresholdHasValue)
            {
                buffer =  new byte[sizeof(float)];
                bufferRead = memStream.Read(buffer);
                if (bufferRead != buffer.Length) throw new   GamerunEndOfStreamException(memStream.Position);
                iGPUTreshold = BitConverter.ToSingle(buffer);
            }

            if (_parkedCoresHasValue)
            {
                var parkedCoresLength = 0;
                if (_parkedCoresVLE)
                {
                    parkedCoresLength = (int)Tools.DecodeVarUInt(memStream);
                }
                else
                {
                    buffer = new byte[sizeof(uint)];
                    bufferRead = memStream.Read(buffer);
                    if (bufferRead != buffer.Length) throw new  GamerunEndOfStreamException(memStream.Position);
                    parkedCoresLength = (int)BitConverter.ToUInt32(buffer);
                }

                if (parkedCoresLength > 0)
                {
                    buffer = new byte[parkedCoresLength / 8];
                    bufferRead = memStream.Read(buffer);
                    if (bufferRead != buffer.Length) throw new  GamerunEndOfStreamException(memStream.Position);
                    ParkedCores = Tools.UnpackBytesToBools(buffer, parkedCoresLength);
                }
            }

            
            if (_pinnedCoresHasValue)
            {
                var pinnedCoresLength = 0;
                if (_pinnedCoresVLE)
                {
                    pinnedCoresLength = (int)Tools.DecodeVarUInt(memStream);
                }
                else
                {
                    buffer = new byte[sizeof(uint)];
                    bufferRead = memStream.Read(buffer);
                    if (bufferRead != buffer.Length) throw new  GamerunEndOfStreamException(memStream.Position);
                    pinnedCoresLength = (int)BitConverter.ToUInt32(buffer);
                }

                if (pinnedCoresLength > 0)
                {
                    buffer = new byte[pinnedCoresLength / 8];
                    bufferRead = memStream.Read(buffer);
                    if (bufferRead != buffer.Length) throw new  GamerunEndOfStreamException(memStream.Position);
                    PinnedCores = Tools.UnpackBytesToBools(buffer, pinnedCoresLength);
                }
            }
            
            if (!_pidHasValue) return;
            if (_pidIsVLE)
            {
                PID = (int)((_pidSign ? -1 : 1) * Tools.DecodeVarUInt(memStream));
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = memStream.Read(buffer);
                if (bufferRead != buffer.Length) throw new   GamerunEndOfStreamException(memStream.Position);
                PID = (int)((_pidSign ? -1 : 1) * BitConverter.ToUInt32(buffer));
            }
        }

        /// <summary>
        /// Creates a new GamerunDaemonArgs. Used by "gamerun /path/to/app" to send settings to "gamerun --daemon".
        /// </summary>
        public GamerunDaemonArgs()
        {
            
        }
        #endregion CONSTRUCTORS
        
        #region PROPERTIES
        /// <summary>
        /// Process ID of the app.
        /// </summary>
        public int PID { get; set; }
        /// <summary>
        /// TLP requires root privileges to set power mode. If on and TLP is available, this will send commands to TLP.
        /// </summary>
        public bool SetTLP { get; set; }
        /// <summary>
        /// Tells system to prioritize the app.
        /// </summary>
        public bool Prioritize { get; set; }
        /// <summary>
        /// Tells system to prioritize Input/Output for the app.
        /// </summary>
        public bool PrioritizeIO { get; set; }
        /// <summary>
        /// Determines if certain processor cores should be parked (disabled) when running this app.
        /// </summary>
        public bool ParkCores { get; set; }
        /// <summary>
        /// Determines if certain processor cores should be pinned to this app. 
        /// </summary>
        public bool PinCores { get; set; }
        /// <summary>
        /// Determines if Split Lock Mitigation should be disabled or enabled on Intel processors.
        /// </summary>
        public bool SetSplitLockMitigation { get; set; }
        /// <summary>
        /// Array of parked (disabled) processor cores.
        /// </summary>
        public bool[] ParkedCores { get; set; }
        /// <summary>
        /// Array of pinned processor cores tp this app.
        /// </summary>
        public bool[] PinnedCores { get; set; }
        /// <summary>
        /// Changes the scheduler policy to SCHED_ISO on kernels that supports it.
        /// </summary>
        public bool SoftRealtime { get; set; }
        /// <summary>
        /// Determines if GPU should be optimized or not.
        /// </summary>
        public bool OptimizeGPU { get; set; }
        /// <summary>
        /// Determines if integrated GPU's power governor should be changed or not.
        /// </summary>
        public bool iGPUGovernor { get; set; }
        /// <summary>
        /// Determines the integrated GPU's treshold. Calculated by integrated GPU's Watts / Processor Watts
        /// </summary>
        public float iGPUTreshold { get; set; }
        /// <summary>
        /// Determines if Nvidia's PowerMizer setting should be changed on Nvidia cards (with Nvidia's drivers, no change on Nouveau and Nova).
        /// </summary>
        public bool NvPowerMizer { get; set; }
        /// <summary>
        /// Determines the memory overclocking on Nvidia cards (with Nvidia's drivers, no change on Nouveau and Nova).
        /// </summary>
        public uint NvMemClockOffset { get; set; }
        /// <summary>
        /// Determines the core overclocking on Nvidia card (with Nvidia's drivers, no change on Nouveau and Nova).
        /// </summary>
        public uint NvCoreClockOffset { get; set; }
        /// <summary>
        /// Determines the performance level on AMD graphics cards.
        /// </summary>
        public bool AMDPerfLevel { get; set; }
        #endregion PROPERTIES

        #region PRIVATE PROPERTIES
        private bool[] Settings =>
        [
            SetTLP,
            Prioritize,
            PrioritizeIO,
            ParkCores,
            PinCores,
            OptimizeGPU,
            SetSplitLockMitigation,
            SoftRealtime,
            iGPUGovernor,
            NvPowerMizer,
            AMDPerfLevel,
            NvMemClockOffset == 0,
            NvCoreClockOffset == 0,
            NvMemClockOffset < Tools.VLEMaxSize,
            NvCoreClockOffset < Tools.VLEMaxSize,
            PID == 0,
            Math.Abs(PID) < Tools.VLEMaxSize,
            PID < 0,
            iGPUTreshold == 0,
            ParkedCores.Length > 0,
            ParkedCores.Length < Tools.VLEMaxSize,
            PinnedCores.Length > 0,
            PinnedCores.Length < Tools.VLEMaxSize,

        ];
        #endregion PRIVATE PROPERTIES
        
        #region FUNCTIONS
        /// <summary>
        /// Converts thess arguments to a Base64 string.
        /// </summary>
        /// <returns>A Base64 encoded string.</returns>

        public string ToBase64()
        {
            using var memstream = new MemoryStream();
            var bufferArray = Tools.PackBoolsToBytes(Settings);
            memstream.Write(bufferArray);
            if (NvMemClockOffset > 0)
            {
                if (NvMemClockOffset < Tools.VLEMaxSize)
                {
                    Tools.WriteVarUInt(memstream, NvMemClockOffset);
                }
                else
                {
                    bufferArray = BitConverter.GetBytes(NvMemClockOffset);
                    memstream.Write(bufferArray);
                }
            }

            if (NvCoreClockOffset > 0)
            {
                if (NvCoreClockOffset < Tools.VLEMaxSize)
                {
                    Tools.WriteVarUInt(memstream, NvCoreClockOffset);
                }
                else
                {
                    bufferArray = BitConverter.GetBytes(NvCoreClockOffset);
                    memstream.Write(bufferArray);
                }
            }

            if (iGPUTreshold != 0)
            {
                bufferArray = BitConverter.GetBytes(iGPUTreshold);
                memstream.Write(bufferArray);
            }

            if (ParkedCores.Length > 0)
            {
                if (ParkedCores.Length < Tools.VLEMaxSize)
                {
                    Tools.WriteVarUInt(memstream, (uint)ParkedCores.Length);
                }
                else
                {
                    bufferArray = BitConverter.GetBytes((uint)ParkedCores.Length);
                    memstream.Write(bufferArray);
                }

                memstream.Write(Tools.PackBoolsToBytes(ParkedCores));
            }
            
            if (PinnedCores.Length > 0)
            {
                if (PinnedCores.Length < Tools.VLEMaxSize)
                {
                    Tools.WriteVarUInt(memstream, (uint)PinnedCores.Length);
                }
                else
                {
                    bufferArray = BitConverter.GetBytes((uint)PinnedCores.Length);
                    memstream.Write(bufferArray);
                }
                
                memstream.Write(Tools.PackBoolsToBytes(PinnedCores));
            }

            if (PID == 0) return Convert.ToBase64String(memstream.ToArray());
            var uintPID = (uint)Math.Abs(PID);
            if (uintPID < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(memstream, uintPID);
            }
            else
            {
                bufferArray = BitConverter.GetBytes(uintPID);
                memstream.Write(bufferArray);
            }

            return Convert.ToBase64String(memstream.ToArray());
        }
        #endregion FUNCTIONS
    }
}