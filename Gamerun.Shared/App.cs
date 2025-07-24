namespace Gamerun.Shared
{
    public class App
    {
        private AppSettings? _settings;

        public App(string commandLine, AppSettings? settings = null)
        {
            CommandLine = commandLine;
            _settings = settings;
        }

        public App(uint id, bool user, bool template = false)
        {
            // TODO
        }

        public string CommandLine { get; set; }
        public uint ID { get; set; }

        public AppSettings Settings
        {
            get
            {
                if (_settings != null || !(Gamerun.Default.Clone() is AppSettings settings)) return _settings;
                _settings = settings.CreateShadowCopy(this);
                return settings;
            }
            set => _settings = value;
        }
    }
}