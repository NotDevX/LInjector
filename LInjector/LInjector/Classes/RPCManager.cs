﻿using DiscordRPC;
using System;
using System.Windows.Forms;
using Button = DiscordRPC.Button;

namespace LInjector.Classes
{
    public static class RPCManager
    {
        public static DiscordRpcClient client;
        public static bool isEnabled;

        public static void SetRPCDetails(string Details)
        {
            RichPresence baseRichPresence = new RichPresence
            {

                Details = Details,
                State = "",

                Assets = new Assets
                {
                    LargeImageKey = "https://lexploits.netlify.app/extra/cdn/LInjector%20ico.png",
                    LargeImageText = "by The LExploits Project."
                },
                Buttons = new[]
                {
                        new Button { Label = "GitHub", Url = "https://github.com/ItzzExcel/LInjector" }
                }
            };
            if (client.IsInitialized)
            {
                try
                {
                    client.SetPresence(baseRichPresence);
                }
                catch (Exception ex)
                {
                    ThreadBox.MsgThread(
                        "Couldn't update LInjector State (RPC)\nException:\n" + ex.Message, "[WARNING] LInjector",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // public void SetListeningStatus(string name, string artist, string albumFormatted)
        // {
        //     Game activity = new(
        //         artist + albumFormatted,
        //         ActivityType.Listening,
        //         ActivityProperties.Instance,
        //         name
        //     );
        //     this.client.SetActivityAsync(activity);
        //}

        public static void InitRPC()
        {
            client = new DiscordRpcClient("1104489169314660363");
            if (isEnabled)
            {
                client.Initialize();
            }
            SetRPCDetails("Using LInjector");
        }

        public static void SetRpcFile(string currentFile)
        {
            if (client.IsInitialized)
            {
                try
                {
                    SetRPCDetails("Editing File: " + currentFile);
                }
                catch (Exception ex)
                {
                    ThreadBox.MsgThread(
                        "Couldn't update LInjector State (RPC)\nException:\n" + ex.Message, "[WARNING] LInjector",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public static void SetBaseRichPresence()
        {
            if (client.IsInitialized)
            {
                try
                {
                    SetRPCDetails("Using LInjector");
                }
                catch (Exception ex)
                {
                    ThreadBox.MsgThread(
                        "Couldn't set base Rich Presence (RPC)\nException:\n" + ex.Message, "[WARNING] LInjector",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}