using System;
using System.IO;
using System.Text;

namespace Gamerun.Shared
{
    // TODO: Add XML documentation
    public class MangoHUDSettings : GamerunSettingsAbstract
    {
        private bool _configIsFile;
        private string _configuration = string.Empty;
        private bool _enabled;

        public string Configuration
        {
            get => _configuration;
            set
            {
                _configuration = value;
                OnSave?.Invoke();
            }
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                OnSave?.Invoke();
            }
        }

        public bool ConfigIsFile
        {
            get => _configIsFile;
            set
            {
                _configIsFile = value;
                OnSave?.Invoke();
            }
        }

        public override void ReadSettings(Stream stream)
        {
            var bufferByte = stream.ReadByte();
            var bufferRead = 0;
            byte[] buffer;
            if (bufferByte == -1) throw new Exception(); // TODO
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
                if (bufferRead != buffer.Length) throw new Exception(); // TODO
                configLength = BitConverter.ToInt32(buffer, 0);
            }

            if (configLength == 0) return;
            buffer = new byte[configLength];
            bufferRead = stream.Read(buffer, 0, buffer.Length);
            if (bufferRead != buffer.Length) throw new Exception(); // TODO
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
    }
}