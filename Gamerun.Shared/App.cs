namespace Gamerun.Shared
{
    public class App
    {
        public App(string commandLine, AppSettings? settings)
        {
            CommandLine = commandLine;
            Settings = settings ?? Gamerun.Default;
        }

        public string CommandLine { get; set; }

        public AppSettings Settings
        {
            get; // TODO: Check if something wants to write here and if it does then copy the default value here 
            private set;
        }
    }
}