using System;
using System.IO;
using System.Text;
using Gamerun.Shared.Exceptions;

namespace Gamerun.Shared
{
    /// <summary>
    ///     MangoHUD is a layer that shows system information on apps.
    /// </summary>
    public class MangoHUDSettings : GamerunSettingsAbstract
    {
        #region PUBLIC PROPERTIES

        /// <summary>
        ///     Configuration of MangoHUD or a path of file.
        /// </summary>
        public string Configuration
        {
            get => _configuration ?? Gamerun.Default.MangoHUD.Configuration;
            set
            {
                _configuration = value;
                OnSave?.Invoke();
            }
        }

        /// <summary>
        ///     Determines if MangoHUD should be enabled or not.
        /// </summary>
        public bool Enabled
        {
            get => _enabled ?? Gamerun.Default.MangoHUD.Enabled;
            set
            {
                _enabled = value;
                OnSave?.Invoke();
            }
        }

        /// <summary>
        ///     Determines if <see cref="Configuration" /> is a path to a file or the whole configuration itself.
        /// </summary>
        public bool ConfigIsFile
        {
            get => _configIsFile ?? Gamerun.Default.MangoHUD.ConfigIsFile;
            set
            {
                _configIsFile = value;
                OnSave?.Invoke();
            }
        }

        #endregion PUBLIC PROPERTIES

        #region OVERRIDES

        public override bool IsDefaults => _enabled == null && _configIsFile == null && _configuration == null;

        public override void ReadSettings(Stream stream)
        {
            var bufferByte = stream.ReadByte();
            var bufferRead = 0;
            byte[] buffer;
            if (bufferByte == -1) throw new GamerunEndOfStreamException(stream.Position);
            _enabled = Tools.IsBitSet(bufferByte, 0);
            _configIsFile = Tools.IsBitSet(bufferByte, 1);
            var configIsEmpty = Tools.IsBitSet(bufferByte, 2);
            var configIsVLE = Tools.IsBitSet(bufferByte, 2);

            if (configIsEmpty) return;

            var configLength = 0;

            if (configIsVLE)
            {
                configLength = (int)Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(int)];
                bufferRead = stream.Read(buffer, 0, buffer.Length);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                configLength = BitConverter.ToInt32(buffer, 0);
            }

            if (configLength == 0) return;
            buffer = new byte[configLength];
            bufferRead = stream.Read(buffer, 0, buffer.Length);
            if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
            _configuration = Encoding.UTF8.GetString(buffer, 0, configLength);
        }

        public override void WriteSettings(Stream stream)
        {
            var length = Encoding.UTF8.GetByteCount(Configuration);
            byte bufferByte = 0;
            byte[] buffer;
            bufferByte += (byte)(Enabled ? 1 : 0);
            bufferByte += (byte)(ConfigIsFile ? 2 : 0);
            bufferByte += (byte)(string.IsNullOrWhiteSpace(Configuration) ? 4 : 0);
            bufferByte += (byte)(length < Tools.VLEMaxSize ? 8 : 0);
            stream.WriteByte(bufferByte);

            if (length < Tools.VLEMaxSize)
            {
                Tools.WriteVarUInt(stream, (uint)length);
            }
            else
            {
                buffer = BitConverter.GetBytes(length);
                stream.Write(buffer, 0, buffer.Length);
            }

            using var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true);
            writer.Write(Configuration);
        }

        public override GamerunStartArguments GenerateArgs(GamerunStartArguments args)
        {
            if (Enabled) args.Environment["MANGOHUD"] = "1";
            if (ConfigIsFile) args.Environment["MANGOHUD_CONFIGFILE"] = Configuration;
            else args.Environment["MANGOHUD_CONFIG"] = Configuration;
            return args;
        }

        public override event GamerunSettingSaveDelegate? OnSave;

        #endregion OVERRIDES

        #region PRIVATE

        private bool? _configIsFile;
        private string? _configuration = string.Empty;
        private bool? _enabled;

        #endregion PRIVATE
    }
}