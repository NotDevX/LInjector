﻿using System;
using System.Windows.Forms;
using LInjector.Classes;

namespace LInjector
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        public const string currentVersion = "v11.06.2023a";
        // Put "f81fb0e34f313b6cf0d0fc345890a33f" for skipping isOutdated MessageBox. 
        // The versions are in format dd/MM/yyy. (adding the v), if it's December 31, 1969, the version is "v31.12.1969"


        [STAThread]
        public static void Main(string[] args)
        {
            if (GitHubVersionChecker.IsOutdatedVersion(currentVersion) == true)
            {
                DialogResult outDatedResult = MessageBox.Show("LInjector is outdated, please, re-run LInjector Updating System or download the latest release via GitHub.\n" +
                    "Go to LInjector GitHub?",
                    "LInjector | Outdated", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (outDatedResult == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://github.com/ItzzExcel/LInjector/releases");
                }
            }

            tempLog.CreateVersionFile(currentVersion);
            if (args.Length > 0 && args[0] == "--metalpipe")
            {
                doPipe.DownloadPipeAsync();
                Console.WriteLine("--metalpipe called.");
            }
            DiscordRPCManager.InitRPC();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SingleInstanceChecker.CheckInstance();
        }
    }
}
