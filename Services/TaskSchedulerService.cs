using System;
using System.Diagnostics;
using System.IO;

namespace Subscription_Manager.Services
{
    public static class TaskSchedulerService
    {
        private const string TaskName = "SubTrack Daily Notification";

        public static void EnsureTaskExists()
        {
            if (TaskExists())
                return;

            CreateTask();
        }

        public static void RemoveTask()
        {
            if (!TaskExists())
                return;

            RunSchtasks($"/Delete /TN \"{TaskName}\" /F");
        }

        private static bool TaskExists()
        {
            var result = RunSchtasks($"/Query /TN \"{TaskName}\"");
            return result.ExitCode == 0;
        }

        private static void CreateTask()
        {
            string exePath = Process.GetCurrentProcess().MainModule!.FileName!;

            string arguments =
                $"/Create /SC DAILY /ST 09:00 " +
                $"/TN \"{TaskName}\" " +
                $"/TR \"\\\"{exePath}\\\" --notify\" " +
                "/RL LIMITED /F";

            RunSchtasks(arguments);
        }

        private static ProcessResult RunSchtasks(string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "schtasks",
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi)!;
            process.WaitForExit();

            return new ProcessResult
            {
                ExitCode = process.ExitCode
            };
        }

        private class ProcessResult
        {
            public int ExitCode { get; set; }
        }
    }
}
