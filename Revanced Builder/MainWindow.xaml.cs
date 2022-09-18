using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Net;
using System.Net.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.ComponentModel;

namespace Revanced_Builder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool downloadNewVersion = false;
        public MainWindow()
        {
            InitializeComponent();
            this.Visibility = Visibility.Hidden;
            CheckIfFoldersExist();
            GrabPatchesList();
            GC.Collect();
            AddURLInList();
            GrabNewestVersionAndComapre();
            GC.Collect();
            DownloadReVanced();
            GC.Collect();
            this.Visibility = Visibility.Visible;
            this.Closing += MainWindow_Closing;
        }

        List<Patch> patchesList = new List<Patch>();
        Dictionary<string, string> appDescription = new Dictionary<string, string>();
        Dictionary<string, Uri> ReVancedURL = new Dictionary<string, Uri>();
        List<string> ReVancedCurrentVersion = new List<string>();
        List<string> NewestReVancedVersion = new List<string>();
        List<string> FileNames = new List<string>();
        List<appVersion> CurrentAppVersions = new List<appVersion>();
        bool ReVancedVersionEmpty = false;

        //ReVanced stuff
        private async void GrabPatchesList()
        {
            HttpClient client = new HttpClient();
            Uri patchSource = new Uri("https://raw.githubusercontent.com/revanced/revanced-patches/main/patches.json");
            string JSONAsync = await client.GetStringAsync(patchSource);
            patchesList = JsonConvert.DeserializeObject<List<Patch>>(JSONAsync);
            foreach (Patch patch in patchesList)
            {
                if (!patch.deprecated)
                {
                    List<CompatiblePackage> temp = patch.compatiblePackages;
                    appDescription.Add(patch.name, patch.description);
                    foreach (CompatiblePackage item in temp)
                    {
                        switch (item.name)
                        {
                            default:
                                break;
                            case "com.google.android.youtube":
                                YoutubeExcludedFeatures.Items.Add(patch.name);
                                YoutubeExcludedFeaturesList.Add(patch.name);
                                break;
                            case "com.google.android.apps.youtube.music":
                                YoutubeMusicExcludedFeaturesList.Add(patch.name);
                                YoutubeMusicExcludedFeatures.Items.Add(patch.name);
                                break;
                            case "com.twitter.android":

                                break;
                            case "com.reddit.frontpage":

                                break;
                        }
                    }
                }
            }
        }

        private void GrabDefault()
        {
            WebClient client = new WebClient();
            Uri url = new Uri("https://raw.githubusercontent.com/shazzaam7/ReVanced-Builder/main/default.json");
            client.DownloadFile(url, Directory.GetCurrentDirectory() + @"\versions\appVersions.json");
        }

        private void CheckIfFoldersExist()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced");
            }
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced\Apks"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced\Apks");
            }
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced\RevancedApks"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced\RevancedApks");
            }
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\versions"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\versions");
            }
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\versions\ReVancedversion.txt"))
            {
                File.Create(Directory.GetCurrentDirectory() + @"\versions\ReVancedversion.txt");
            }
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\versions\appVersions.json"))
            {
                GrabDefault();
            }
            FileInfo info = new FileInfo(Directory.GetCurrentDirectory() + @"\versions\ReVancedversion.txt");
            if (info.Length == 0)
            {
                ReVancedVersionEmpty = true;
            }
            FileInfo appVersionInfo = new FileInfo(Directory.GetCurrentDirectory() + @"\versions\appVersions.json");
            if (appVersionInfo.Length == 0)
            {
                Console.WriteLine("Empty");
                
            }
        }

        private void AddURLInList()
        {
            Uri ReVancedCLI = new Uri("https://github.com/revanced/revanced-cli/releases/latest");
            Uri ReVancedPatches = new Uri("https://github.com/revanced/revanced-patches/releases/latest");
            Uri ReVancedIntegrations = new Uri("https://github.com/revanced/revanced-integrations/releases/latest");
            ReVancedURL.Add("revanced-cli", ReVancedCLI);
            ReVancedURL.Add("revanced-patches", ReVancedPatches);
            ReVancedURL.Add("revanced-integrations", ReVancedIntegrations);
        }

        private void GrabNewestVersionAndComapre()
        {
            if (ReVancedVersionEmpty)
            {
                using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + @"\versions\ReVancedversion.txt"))
                {
                    foreach (Uri item in ReVancedURL.Values)
                    {
                        WebClient client = new WebClient();
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(client.DownloadString(item));
                        var version = doc.DocumentNode.SelectSingleNode("//*[@id='repo-content-pjax-container']/div/nav/ol/li[2]/a").InnerText;
                        string temp = version.Replace(" ", "");
                        NewestReVancedVersion.Add(temp);
                    }
                    foreach (string item in NewestReVancedVersion)
                    {
                        sw.Write(item);
                    }
                    downloadNewVersion = true;
                }
            } else
            {
                using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + @"\versions\ReVancedversion.txt"))
                {
                    string line = sr.ReadLine();
                    List<string> temp = new List<string>();
                    while (line != null)
                    {
                        temp.Add(line);
                        line = sr.ReadLine();
                    }
                    for (int i = 0; i < temp.Count; i++)
                    {
                        ReVancedCurrentVersion.Add(temp[i]);
                    }
                }
                using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + @"\versions\ReVancedversion.txt"))
                {
                    foreach (Uri item in ReVancedURL.Values)
                    {
                        WebClient client = new WebClient();
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(client.DownloadString(item));
                        var version = doc.DocumentNode.SelectSingleNode("//*[@id='repo-content-pjax-container']/div/nav/ol/li[2]/a").InnerText;
                        string temp = version.Replace(" ", "");
                        temp = temp.Substring(0, temp.LastIndexOf('\n'));
                        NewestReVancedVersion.Add(temp);
                    }
                    for (int i = 0; i < NewestReVancedVersion.Count; i++)
                    {
                        if (ReVancedCurrentVersion[i] != NewestReVancedVersion[i])
                        {
                            ReVancedCurrentVersion[i] = NewestReVancedVersion[i];
                            downloadNewVersion = true;
                            sw.WriteLine(ReVancedCurrentVersion[i]);
                        } else
                        {
                            sw.WriteLine(ReVancedCurrentVersion[i]);
                        }
                    }
                }
            }
        }

        private void DownloadReVanced()
        {
            if (downloadNewVersion)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced");
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    if (file.Name.StartsWith("revanced-") || file.Name.StartsWith("app"))
                    {
                        file.Delete();
                    }
                }
                int i = 0;
                foreach (string link in ReVancedURL.Keys)
                {
                    WebClient client = new WebClient();
                    WebClient downloadClient = new WebClient();
                    HtmlDocument doc = new HtmlDocument();
                    Uri test = new Uri("https://github.com/revanced/" + link + "/releases/expanded_assets/" + NewestReVancedVersion[i]);
                    doc.LoadHtml(client.DownloadString(test));
                    var download = doc.DocumentNode.SelectNodes("//a");
                    foreach (var item in download)
                    {
                        if (item.Attributes["href"].Value.EndsWith(".jar") || item.Attributes["href"].Value.EndsWith(".apk"))
                        {
                            string fileName = item.Attributes["href"].Value.Substring(item.Attributes["href"].Value.LastIndexOf('/')).Replace("/", "");
                            FileNames.Add(fileName);
                            if (!File.Exists(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced\" + fileName))
                            {
                                Uri downloadLink = new Uri("https://github.com" + item.Attributes["href"].Value);
                                AppDownloadWindow downloads = new AppDownloadWindow(downloadLink, fileName);
                                downloads.ShowDialog();
                                break;
                            }
                        }
                    }
                    i++;
                }
            } else
            {
                int i = 0;
                foreach (string link in ReVancedURL.Keys)
                {
                    WebClient client = new WebClient();
                    WebClient downloadClient = new WebClient();
                    HtmlDocument doc = new HtmlDocument();
                    Uri test = new Uri("https://github.com/revanced/" + link + "/releases/expanded_assets/" + NewestReVancedVersion[i]);
                    doc.LoadHtml(client.DownloadString(test));
                    var download = doc.DocumentNode.SelectNodes("//a");
                    foreach (var item in download)
                    {
                        if (item.Attributes["href"].Value.EndsWith(".jar") || item.Attributes["href"].Value.EndsWith(".apk"))
                        {
                            string fileName = item.Attributes["href"].Value.Substring(item.Attributes["href"].Value.LastIndexOf('/')).Replace("/", "");
                            FileNames.Add(fileName);
                            break;
                        }
                    }
                    i++;
                }
            }
        }

        //APK Downloader
        private void DownloadAPK(string name)
        {
            using (StreamReader sr = File.OpenText(Directory.GetCurrentDirectory() + @"\versions\appVersions.json"))
            {
                string line = sr.ReadToEnd();
                CurrentAppVersions = JsonConvert.DeserializeObject<List<appVersion>>(line);
            }
            foreach (appVersion item in CurrentAppVersions)
            {
                Console.WriteLine(item.name + " " + item.version);
            }
            string version = "";
            string programVersion = "";
            foreach (Patch patch in patchesList)
            {
                List<CompatiblePackage> temp = patch.compatiblePackages;
                foreach (CompatiblePackage item in temp)
                {
                    switch (name)
                    {
                        default:
                            break;
                        case "com.google.android.youtube":
                            programVersion = "YouTube";
                            if (item.name == name)
                            {
                                List<string> versions = item.versions;
                                if (versions.Count > 0)
                                {
                                    int lastVersion = versions.Count() - 1;
                                    if (version.Length < 1)
                                    {
                                        version = versions[lastVersion];
                                    }
                                    else
                                    {
                                        string[] latestTest = versions[lastVersion].Split('.');
                                        string[] currentVersion = version.Split('.');
                                        if (int.Parse(currentVersion[0]) <= int.Parse(latestTest[0]))
                                        {
                                            if (int.Parse(currentVersion[1]) <= int.Parse(latestTest[1]))
                                            {
                                                version = versions[lastVersion];
                                            }
                                            else
                                            {
                                                if (int.Parse(currentVersion[2]) <= int.Parse(latestTest[2]))
                                                {
                                                    version = versions[lastVersion];
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case "com.google.android.apps.youtube.music":
                            if (item.name == name)
                            {
                                List<string> versions = item.versions;
                                if (versions.Count > 0)
                                {
                                    int lastVersion = versions.Count() - 1;
                                    if (version.Length < 1)
                                    {
                                        version = versions[lastVersion];
                                    }
                                    else
                                    {
                                        string[] latestTest = versions[lastVersion].Split('.');
                                        string[] currentVersion = version.Split('.');
                                        if (int.Parse(currentVersion[0]) <= int.Parse(latestTest[0]))
                                        {
                                            if (int.Parse(currentVersion[1]) <= int.Parse(latestTest[1]))
                                            {
                                                version = versions[lastVersion];
                                            }
                                            else
                                            {
                                                if (int.Parse(currentVersion[2]) <= int.Parse(latestTest[2]))
                                                {
                                                    version = versions[lastVersion];
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            programVersion = "YouTubeMusic";
                            break;
                    }
                }
            }
            appVersion tempVersion = new appVersion();
            bool downloadNewAPK = false;
            switch (programVersion)
            {
                default:
                    break;
                case "YouTube":
                    tempVersion.name = programVersion;
                    tempVersion.version = version;
                    foreach (appVersion item in CurrentAppVersions)
                    {
                        if (item.name == tempVersion.name)
                        {
                            if (item.version != tempVersion.version)
                            {
                                int i = CurrentAppVersions.IndexOf(item);
                                CurrentAppVersions.RemoveAt(i);
                                CurrentAppVersions.Insert(i, tempVersion);
                                downloadNewAPK = true;
                                break;
                            }
                        }
                    }
                    if (downloadNewAPK)
                    {
                        AppDownloadWindow YoutubeAPKDownload = new AppDownloadWindow(programVersion, version.Replace('.', '-'));
                        YoutubeAPKDownload.ShowDialog();
                    } else
                    {
                        Console.WriteLine("YouTube APK is up to date.");
                    }
                    break;
                case "YouTubeMusic":
                    tempVersion.name = programVersion;
                    tempVersion.version = version;
                    foreach (appVersion item in CurrentAppVersions)
                    {
                        if (item.name == tempVersion.name)
                        {
                            if (item.version != tempVersion.version)
                            {
                                int i = CurrentAppVersions.IndexOf(item);
                                CurrentAppVersions.RemoveAt(i);
                                CurrentAppVersions.Insert(i, tempVersion);
                                downloadNewAPK = true;
                                break;
                            }
                        }
                    }
                    if (downloadNewAPK)
                    {
                        AppDownloadWindow YoutubeMusicAPK = new AppDownloadWindow(programVersion, version.Replace('.','-'));
                        YoutubeMusicAPK.ShowDialog();
                    } else
                    {
                        Console.WriteLine("YouTube Music APK is up to date.");
                    }
                    break;
            }
            using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + @"\versions\appVersions.json"))
            {
                sw.Write(JsonConvert.SerializeObject(CurrentAppVersions));
            }
        }


        //Exit
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        //Youtube ReVanced Stuff

        List<string> YoutubeExcludedFeaturesList = new List<string>();
        List<string> YoutubeIncludedFeaturesList = new List<string>();

        private void YoutubeBuildModdedApp_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced\RevancedApks\YoutubeRevanced.apk"))
            {
                File.Delete(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced\RevancedApks\YoutubeRevanced.apk");
            }
            DownloadAPK("com.google.android.youtube");
            string zuluJDKPath = Directory.GetCurrentDirectory() + @"\zuluJDK\bin\";
            string arguments = @" -jar Revanced\" + FileNames[0] + @" -a Revanced\Apks\Youtube.apk -o Revanced\RevancedApks\YoutubeRevanced.apk -b Revanced\" + FileNames[1] + @" -m Revanced\" + FileNames[2];
            foreach (string item in YoutubeIncludedFeaturesList)
            {
                arguments = arguments + " -i " + item;
            }
            foreach (string item in YoutubeExcludedFeaturesList)
            {
                arguments = arguments + " -e " + item;
            }
            ProcessStartInfo builderInfo = new ProcessStartInfo("java.exe", arguments)
            {
                CreateNoWindow = false,
                UseShellExecute = true
            };
            builderInfo.WorkingDirectory = zuluJDKPath;
            Process builder = new Process();
            builder.StartInfo = builderInfo;
            builder.Start();
            builder.WaitForExit();
            Process.Start(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced\RevancedApks");
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\revanced-cache");
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    if (file.Name.StartsWith("revanced") || file.Name.EndsWith(".apk") || file.Name.StartsWith("aapt"))
                    {
                        file.Delete();
                    }
                }
            }
            catch
            {
            }
        }

        private void YoutubeExcludedFeatures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YoutubeExcludedFeatures.SelectedIndex < 0)
            {
                YoutubeExcludedFeatures.SelectedIndex = 0;
            }
            if (YoutubeExcludedFeatures.Items.Count < 1)
            {
                YoutubeFeatureDescription.Text = "";
                return;
            }
            string tempName;
            YoutubeFeatureDescription.Text = "";
            try
            {
                tempName = YoutubeExcludedFeatures.SelectedItem.ToString();
            }
            catch
            {
                return;
            }
            foreach (string key in appDescription.Keys)
            {
                if (key == tempName)
                {
                    YoutubeFeatureDescription.Text = "Feature Description:\n" + appDescription[key];
                }
            }
        }

        private void YoutubeIncludedFeatures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YoutubeExcludedFeatures.Items.Count < 0)
            {
                return;
            }
        }

        private void YoutubeIncludeFeatureButton_Click(object sender, RoutedEventArgs e)
        {
            if (YoutubeExcludedFeatures.SelectedIndex < 0)
            {
                return;
            }
            string selectedFeature;
            try
            {
                selectedFeature = YoutubeExcludedFeatures.SelectedItem.ToString();
            }
            catch
            {
                return;
            }
            YoutubeExcludedFeatures.Items.Remove(selectedFeature);
            YoutubeIncludedFeatures.Items.Add(selectedFeature);
            YoutubeExcludedFeaturesList.Remove(selectedFeature);
            YoutubeIncludedFeaturesList.Add(selectedFeature);
        }

        private void YoutubeExcludeFeatureButton_Click(object sender, RoutedEventArgs e)
        {
            if (YoutubeIncludedFeatures.SelectedIndex < 0)
            {
                return;
            }
            string selectedFeature;
            try
            {
                selectedFeature = YoutubeIncludedFeatures.SelectedItem.ToString();
            }
            catch
            {
                return;
            }
            YoutubeExcludedFeatures.Items.Add(selectedFeature);
            YoutubeIncludedFeatures.Items.Remove(selectedFeature);
            YoutubeExcludedFeaturesList.Add(selectedFeature);
            YoutubeIncludedFeaturesList.Remove(selectedFeature);
        }

        //Youtube Music ReVanced Stuff

        List<string> YoutubeMusicExcludedFeaturesList = new List<string>();
        List<string> YoutubeMusicIncludedFeaturesList = new List<string>();

        private void YoutubeMusicBuildButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced\RevancedApks\YoutubeMusicRevanced.apk"))
            {
                File.Delete(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced\RevancedApks\YoutubeMusicRevanced.apk");
            }
            DownloadAPK("com.google.android.apps.youtube.music");
            string zuluJDKPath = Directory.GetCurrentDirectory() + @"\zuluJDK\bin\";
            string arguments = @" -jar Revanced\" + FileNames[0] + @" -a Revanced\Apks\YoutubeMusic.apk -o Revanced\RevancedApks\YoutubeMusicRevanced.apk -b Revanced\" + FileNames[1] + @" -m Revanced\" + FileNames[2];
            foreach (string item in YoutubeMusicIncludedFeaturesList)
            {
                arguments = arguments + " -i " + item;
            }
            foreach (string item in YoutubeMusicExcludedFeaturesList)
            {
                arguments = arguments + " -e " + item;
            }
            ProcessStartInfo builderInfo = new ProcessStartInfo("java.exe", arguments)
            {
                CreateNoWindow = false,
                UseShellExecute = true
            };
            builderInfo.WorkingDirectory = zuluJDKPath;
            Process builder = new Process();
            builder.StartInfo = builderInfo;
            builder.Start();
            builder.WaitForExit();
            Process.Start(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced\RevancedApks");
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory() + @"\zuluJDK\bin\revanced-cache");
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    if (file.Name.StartsWith("revanced") || file.Name.EndsWith(".apk") || file.Name.StartsWith("aapt"))
                    {
                        file.Delete();
                    }
                }
            }
            catch
            {
            }
        }

        private void YoutubeMusicExcludedFeatures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YoutubeMusicExcludedFeatures.SelectedIndex < 0)
            {
                YoutubeMusicExcludedFeatures.SelectedIndex = 0;
            }
            if (YoutubeMusicExcludedFeatures.Items.Count < 1)
            {
                YoutubeMusicFeatureDescription.Text = "";
                return;
            }
            string temp;
            YoutubeMusicFeatureDescription.Text = "";
            try
            {
                temp = YoutubeMusicExcludedFeatures.SelectedItem.ToString();
            }
            catch
            {
                return;
            }
            foreach (string key in appDescription.Keys)
            {
                if (key == temp)
                {
                    YoutubeMusicFeatureDescription.Text = "Feature Description:\n" + appDescription[key];
                }
            }
        }

        private void YoutubeMusicIncludedFeatures_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YoutubeMusicIncludedFeatures.Items.Count < 0)
            {
                return;
            }
        }

        private void YoutubeMusicIncludeFeatureButton_Click(object sender, RoutedEventArgs e)
        {
            if (YoutubeMusicExcludedFeatures.SelectedIndex < 0)
            {
                return;
            }
            string selectedFeature;
            try
            {
                selectedFeature = YoutubeMusicExcludedFeatures.SelectedItem.ToString();
            }
            catch
            {
                return;
            }
            YoutubeMusicExcludedFeatures.Items.Remove(selectedFeature);
            YoutubeMusicIncludedFeatures.Items.Add(selectedFeature);
            YoutubeMusicExcludedFeaturesList.Remove(selectedFeature);
            YoutubeMusicIncludedFeaturesList.Add(selectedFeature);
        }

        private void YoutubeMusicExcludeFeatureButton_Click(object sender, RoutedEventArgs e)
        {
            if (YoutubeMusicIncludedFeatures.SelectedIndex < 0)
            {
                return;
            }
            string selectedFeature;
            try
            {
                selectedFeature = YoutubeMusicIncludedFeatures.SelectedItem.ToString();
            }
            catch
            {
                return;
            }
            YoutubeMusicExcludedFeatures.Items.Add(selectedFeature);
            YoutubeMusicIncludedFeatures.Items.Remove(selectedFeature);
            YoutubeMusicExcludedFeaturesList.Add(selectedFeature);
            YoutubeMusicIncludedFeaturesList.Remove(selectedFeature);
        }
    }
}
