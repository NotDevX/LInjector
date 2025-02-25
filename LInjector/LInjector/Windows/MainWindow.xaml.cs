﻿using LInjector.Classes;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace LInjector.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsSettingsShown = false;
        private bool IsScriptsShown = false;
        private bool IsInfoShown = false;

        private readonly HttpClient client = new HttpClient();
        private readonly WebClient webCl = new WebClient();


        Storyboard StoryBoard = new Storyboard();

        public MainWindow()
        {
            InitializeComponent();
            RunAutoAttachTimer();
        }

        // "ObjectShift" FUNCTION FROM COMET-UPDATING-SYSTEM (https://github.com/MarsQQ/Comet-Updating-System-)

        private IEasingFunction Smooth { get; set; }
        = new QuarticEase
        {
            EasingMode = EasingMode.EaseInOut
        };

        public void ObjectShift(Duration speed, DependencyObject Object, Thickness Get, Thickness Set)
        {
            ThicknessAnimation Animation = new ThicknessAnimation()
            {
                From = Get,
                To = Set,
                Duration = speed,
                EasingFunction = Smooth,
            };
            Storyboard.SetTarget(Animation, Object);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(MarginProperty));
            StoryBoard.Children.Add(Animation);
            StoryBoard.Begin();
            StoryBoard.Children.Remove(Animation);
        }


        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); } catch { }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            TabSystemz.Visibility = Visibility.Hidden;
            Storyboard fadeOutStoryboard = new Storyboard();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.25)
            };
            fadeOutStoryboard.Children.Add(fadeOutAnimation);
            Storyboard.SetTarget(fadeOutAnimation, this);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(Window.OpacityProperty));
            fadeOutStoryboard.Completed += OnCloseFadeoutCompleted;
            fadeOutStoryboard.Begin();

            if (FluxInterfacing.is_injected(FluxInterfacing.pid))
            {
                var processesByName = Process.GetProcessesByName("Windows10Universal");
                foreach (var Process in processesByName)
                {
                    var FilePath = Process.MainModule.FileName;

                    if (FilePath.Contains("ROBLOX"))
                    {
                        Process.Kill();
                    }
                }
            }
        }

        private void OnCloseFadeoutCompleted(object sender, EventArgs e)
        {
            Close();
            System.Windows.Application.Current.Shutdown();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void AttachButton_Click(object sender, RoutedEventArgs e)
        {
            Inject();
        }

        private void ScriptPage_Click(object sender, RoutedEventArgs e)
        {

            if (IsScriptsShown == false)
            {
                if (IsSettingsShown == true || IsInfoShown == true) { return; }
                IsScriptsShown = true;
                ObjectShift(TimeSpan.FromMilliseconds(1500), ScriptsGrid, ScriptsGrid.Margin, new Thickness(0, 0, 0, 0)); // SHOW
                TabSystemz.Visibility = Visibility.Collapsed;
            }
            else
            {
                IsScriptsShown = false;
                ObjectShift(TimeSpan.FromMilliseconds(1500), ScriptsGrid, ScriptsGrid.Margin, new Thickness(-5000, 0, 5000, 0));
                TabSystemz.Visibility = Visibility.Visible;
            }
        }

        private void ConsoleDebugButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConsoleManager.isConsoleVisible)
            { ConsoleManager.HideConsole(); }
            else
            { ConsoleManager.ShowConsole(); }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleControls();
        }

        public void ToggleControls()
        {
            if (IsSettingsShown == false)
            {
                if (IsInfoShown == true || IsScriptsShown == true) { return; }
                IsSettingsShown = true;
                ObjectShift(TimeSpan.FromMilliseconds(1500), SettingsGrid, SettingsGrid.Margin, new Thickness(0, 0, 0, 0)); // SHOW
                TabSystemz.Visibility = Visibility.Collapsed;
            }
            else
            {
                IsSettingsShown = false;
                ObjectShift(TimeSpan.FromMilliseconds(1500), SettingsGrid, SettingsGrid.Margin, new Thickness(-5000, 0, 5000, 0)); // HIDE
                Task.Delay(1500);
                TabSystemz.Visibility = Visibility.Visible;
            }
        }

        private async void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cm = TabSystemz.current_monaco();
                string scriptString = await cm.GetText();

                try
                {
                    var flag = !FluxInterfacing.is_injected(FluxInterfacing.pid);
                    if (!flag)
                    {
                        try
                        {
                            FluxInterfacing.run_script(FluxInterfacing.pid, scriptString);
                        }
                        catch (Exception ex)
                        {
                            CustomCw.Cw($"Fluxus couldn't run the script.\n{ex.Message}\nStack Trace:\n{ex.StackTrace}", false, "error");
                        }
                    }
                    else
                    {
                        _ = Notifications.Fire(StatusListBox, "Inject before running script", NotificationLabel);
                    }
                }
                catch (Exception ex)
                {
                    ThreadBox.MsgThread("Fluxus couldn't run the script.", "LInjector | Fluxus API",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CustomCw.Cw($"(Module) Exception thrown\n{ex.Message}\nStack Trace:\n{ex.StackTrace}", false, "error");
                }
            }
            catch
            {
                _ = Notifications.Fire(StatusListBox, "I don't know what happened.", NotificationLabel);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _ = Notifications.Fire(StatusListBox, "Welcome to LInjector [BETA]", NotificationLabel);

            try
            {
                FluxInterfacing.create_files(Path.GetFullPath(".\\Resources\\libs\\Module.dll"));
            }
            catch (Exception ex)
            {
                ThreadBox.MsgThread("Couldn't initialize Fluxus API\nException:\n"
                                                   + ex.Message
                                                   + "\nPlease, share it on Discord.",
                    "[ERROR] LInjector", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                _ = Notifications.Fire(StatusListBox, "Couldn't initialize Fluxus API.", NotificationLabel);
            }

            if (ConfigHandler.topmost)
            {
                this.Topmost = true;
            }

            TabSystemz.Visibility = Visibility.Visible;
            ParseConfig();
            RefreshScriptList();
            LogToConsole.Log("Welcome to LInjector.", ConsoleLogList);
            _ = VersionChecker.CheckVersionUWP();
        }

        public void Inject()
        {
            FluxInterfacing.create_files(Path.GetFullPath(".\\Resources\\libs\\Module.dll"));
            var flag = !FluxInterfacing.is_injected(FluxInterfacing.pid);
            if (flag)
            {
                try
                {
                    try
                    {

                        _ = Notifications.Fire(StatusListBox, "Called Injection API (Powered by Fluxteam)", NotificationLabel);
                        FluxInterfacing.inject();
                        InternalFunctions.RunInternalFunctions();
                        FunctionWatch.runFuncWatch();
                        if (FluxInterfacing.pid > 0)
                        {
                            LogToConsole.Log($"Injected to Roblox UWP with PID: {FluxInterfacing.pid}", ConsoleLogList);
                        }
                        else
                        {
                            LogToConsole.Log($"Roblox UWP not detected.", ConsoleLogList);
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = Notifications.Fire(StatusListBox, "Something went wrong...", NotificationLabel);
                        ThreadBox.MsgThread($"Exception Message: {ex.InnerException}\nStack Trace: {ex.StackTrace}");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    ThreadBox.MsgThread("Error on inject:\n" + ex.Message
                                                                            + "\nStack Trace:\n" + ex.StackTrace,
                        "LInjector | Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                _ = Notifications.Fire(StatusListBox, "Already injected", NotificationLabel);
            }
        }

        public void RunAutoAttachTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += AttachedDetectorTick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        private void AttachedDetectorTick(object sender, EventArgs e)
        {
            if (ConfigHandler.autoattach == false)
            {
                return;
            }

            var processesByName = Process.GetProcessesByName("Windows10Universal");
            foreach (var Process in processesByName)
            {
                var FilePath = Process.MainModule.FileName;

                if (FilePath.Contains("ROBLOX"))
                {
                    try
                    {
                        var flag = FluxInterfacing.is_injected(FluxInterfacing.pid);
                        if (flag)
                        {
                            return;
                        }

                        Inject();
                    }
                    catch { }
                }
            }
        }

        private void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/ItzzExcel/LInjector");
        }

        private void DiscordButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.gg/NQY28YSVAb");
        }

        private void SearchScriptsBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            RefreshScriptList();
        }

        private void ScriptListHolder_Loaded(object sender, RoutedEventArgs e)
        {
            DirectoryInfo ScriptsFolder = new DirectoryInfo(".\\scripts");
            FileInfo[] Files = ScriptsFolder.GetFiles("*.*");
            foreach (FileInfo Script in Files)
            {
                ScriptListHolder.Items.Add(Script.Name);
            }
        }

        public void RefreshScriptList()
        {
            ScriptListHolder.Items.Clear();

            string searchQuery = SearchScriptsBox.Text.ToLower();
            DirectoryInfo scriptsFolder = new DirectoryInfo(".\\scripts");

            foreach (FileInfo script in scriptsFolder.GetFiles())
            {
                string fileName = script.Name;
                if (fileName.ToLower().Contains(searchQuery))
                {
                    ScriptListHolder.Items.Add(fileName);
                }
            }
        }


        private void SearchScriptsBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.ScriptListHolder.SelectedIndex != -1)
                {
                    string scriptfolder = ".\\scripts\\";
                    object selectedItem = this.ScriptListHolder.SelectedItem;
                    if (this.ScriptListHolder.Items.Count != 0)
                    {
                        TabSystemz.current_monaco().SetText(File.ReadAllText(scriptfolder + (selectedItem != null ? selectedItem.ToString() : (string)null)));
                    }
                }
            }
            catch { }
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Open Script Files | LInjector",
                    Filter = "Script Files (*.txt;*.lua;*.luau)|*.txt;*.lua;*.luau|All files (*.*)|*.*",
                    Multiselect = false
                };
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string fileContent = File.ReadAllText(openFileDialog.FileName);

                    var dialogResult = System.Windows.Forms.MessageBox.Show(
                        "Open file in new tab?",
                        "LInjector",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2);
                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        TabSystemz.add_tab_with_text(fileContent, openFileDialog.SafeFileName);
                    }
                    else
                    {
                        TabSystemz.current_monaco().SetText(fileContent);
                        TabSystemz.ChangeCurrentTabTitle(openFileDialog.SafeFileName);
                    }
                }

            }
            catch
            {
                _ = Notifications.Fire(StatusListBox, "Error while opening the file.", NotificationLabel);
            }
        }

        private void ClearEditor(object sender, RoutedEventArgs e)
        {
            TabSystemz.current_monaco().SetText("");
        }

        private async void SaveToFileButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = await TabSystemz.GetCurrentTabTitle(),
                Title = "Save to File | LInjector",
                Filter = "Script Files (*.txt;*.lua;*.luau)|*.txt;*.lua;*.luau|All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    var cm = TabSystemz.current_monaco();
                    string text = await cm.GetText();
                    string result = text;

                    if (string.IsNullOrEmpty(result))
                    {
                        _ = Notifications.Fire(StatusListBox, "No content detected", NotificationLabel);
                    }
                    else
                    {
                        try
                        {
                            File.WriteAllText(filePath, result);
                            string savedFileName = Path.GetFileName(saveFileDialog.FileName);
                            _ = Notifications.Fire(StatusListBox, $"{savedFileName} saved", NotificationLabel);
                            TabSystemz.ChangeCurrentTabTitle(savedFileName);
                        }
                        catch (Exception)
                        {
                            _ = Notifications.Fire(StatusListBox, "Error saving the file", NotificationLabel);
                        }
                    }


                }
                catch (Exception)
                {
                    _ = Notifications.Fire(StatusListBox, "Error saving the file", NotificationLabel);
                }
            }
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleInfo();
        }

        public void ToggleInfo()
        {
            if (IsInfoShown == false)
            {
                if (IsSettingsShown == true || IsScriptsShown == true) { return; }
                IsInfoShown = true;
                ObjectShift(TimeSpan.FromMilliseconds(1500), InformationGrid, InformationGrid.Margin, new Thickness(0, 0, 0, 0)); // SHOW
                TabSystemz.Visibility = Visibility.Collapsed;
            }
            else
            {

                IsInfoShown = false;
                ObjectShift(TimeSpan.FromMilliseconds(1500), InformationGrid, InformationGrid.Margin, new Thickness(-5000, 0, 5000, 0)); // HIDE
                Task.Delay(1500);
                TabSystemz.Visibility = Visibility.Visible;
            }
        }

        // CONFIGURATION SHIFT

        public void ParseConfig()
        {
            if (ConfigHandler.autoattach)
            { AutoAttachToggle.IsChecked = true; }
            else { AutoAttachToggle.IsChecked = false; }

            if (ConfigHandler.splashscreen)
            { SplashToggle.IsChecked = true; }
            else { SplashToggle.IsChecked = false; }

            if (ConfigHandler.debug)
            { DebugModeToggle.IsChecked = true; }
            else { DebugModeToggle.IsChecked = false; }

            if (ConfigHandler.discord_rpc)
            { RPCToggle.IsChecked = true; }
            else { RPCToggle.IsChecked = false; }

            if (ConfigHandler.topmost)
            { TopmostToggle.IsChecked = true; }
            else { TopmostToggle.IsChecked = false; }
        }

        // AUTO ATTACH TOGGLE

        private void AutoAttachToggle_Checked(object sender, RoutedEventArgs e)
        {
            ConfigHandler.autoattach = true;
            ConfigHandler.SetConfigValue("autoattach", true);
        }

        private void AutoAttachToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ConfigHandler.autoattach = false;
            ConfigHandler.SetConfigValue("autoattach", false);
        }

        // SPLASH SCREEN TOGGLE 

        private void SplashToggle_Checked(object sender, RoutedEventArgs e)
        {
            ConfigHandler.splashscreen = true;
            ConfigHandler.SetConfigValue("splashscreen", true);
        }

        private void SplashToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ConfigHandler.splashscreen = false;
            ConfigHandler.SetConfigValue("splashscreen", false);
        }

        // DEBUG MODE TOGGLE

        private void DebugModeToggle_Checked(object sender, RoutedEventArgs e)
        {
            ConfigHandler.debug = true;
            ConsoleManager.ShowConsole();
            ConfigHandler.SetConfigValue("debug", true);
        }

        private void DebugModeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ConfigHandler.debug = false;
            ConsoleManager.HideConsole();
            ConfigHandler.SetConfigValue("debug", false);
        }

        // RPC Toggle

        private void RPCToggle_Checked(object sender, RoutedEventArgs e)
        {
            ConfigHandler.SetConfigValue("discord_rpc", true);
            ConfigHandler.discord_rpc = true;
            RPCManager.isEnabled = true;

            if (!RPCManager.client.IsInitialized)
            {

                RPCManager.InitRPC();
            }

        }

        private void RPCToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ConfigHandler.SetConfigValue("discord_rpc", false);
            ConfigHandler.discord_rpc = false;
            RPCManager.isEnabled = false;

            if (RPCManager.client.IsInitialized)
            {
                RPCManager.client.Dispose();
            }
        }

        // TOPMOST TOGGLE

        private void TopmostToggle_Checked(object sender, RoutedEventArgs e)
        {
            ConfigHandler.SetConfigValue("topmost", true);
            ConfigHandler.topmost = true;
            Topmost = true;
        }

        private void TopmostToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ConfigHandler.SetConfigValue("topmost", false);
            ConfigHandler.topmost = false;
            Topmost = false;
        }

        // RETURN TO NORMAL:

        private void ShiftToNormal(object sender, RoutedEventArgs e)
        {
            ToggleControls();
        }

        // SAFE MODE TOGGLE

        private void SafeModetoggle_Checked(object sender, RoutedEventArgs e)
        {
            ConfigHandler.SetConfigValue("safe_mode", true);
            ConfigHandler.safe_mode = true;
        }

        private void SafeModetoggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ConfigHandler.SetConfigValue("safe_mode", false);
            ConfigHandler.safe_mode = false;
        }
    }
}
