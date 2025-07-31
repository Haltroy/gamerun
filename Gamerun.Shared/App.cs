namespace Gamerun.Shared
{
    public class App
    {

        public App(string commandLine, AppSettings? settings = null)
        {
            CommandLine = commandLine;
            Settings = settings;
        }

        public App(uint id, bool user, bool template = false)
        {
            // TODO
        }

        public string CommandLine { get; set; }
        public uint ID { get; set; }

        public AppSettings? Settings { get; set; }
    }
}