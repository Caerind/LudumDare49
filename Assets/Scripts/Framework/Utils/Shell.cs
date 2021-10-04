using System.Diagnostics;
using System.Threading;

public static class Shell
{
#if UNITY_EDITOR_WIN
    private const string shellName = "powershell.exe";
#endif
#if UNITY_EDITOR_OSX
    private const string shellName = "/bin/bash";
#endif
#if UNITY_EDITOR_LINUX
    private const string shellName = "/bin/bash";
#endif
#if !UNITY_EDITOR
    private const string shellName = "";
#endif

    public static void ExecuteCommand(string command)
    {
        var thread = new Thread(delegate ()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo()
            {
                FileName = shellName,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                Arguments = command
            };

            Process process = Process.Start(processInfo);
            process.Start();
            process.WaitForExit();
            process.Close();
        });

        thread.Start();
    }
}
