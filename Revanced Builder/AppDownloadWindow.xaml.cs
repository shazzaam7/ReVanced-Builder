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
using System.Windows.Shapes;

using System.Net;
using System.IO;
using HtmlAgilityPack;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading;

namespace Revanced_Builder
{
    /// <summary>
    /// Interaction logic for AppDownloadWindow.xaml
    /// </summary>
    public partial class AppDownloadWindow : Window
    {
        public AppDownloadWindow()
        {
            InitializeComponent();
            GetZuluJDK();
        }

        public AppDownloadWindow(Uri downloadLink, string fileName)
        {
            InitializeComponent();
            ReVancedDownloader(downloadLink, fileName);
        }
        public AppDownloadWindow(string appName, string version)
        {
            InitializeComponent();
            AppDownloader(appName,version);
        }

        private void GetZuluJDK()
        {
            if (!File.Exists("zulu.zip"))
            {
                if (!Directory.Exists("zuluJDK"))
                {
                    WebClient client = new WebClient();
                    DownloadingLabel.Content = "Downloading ZuluJDK17";
                    Uri JDKlink = new Uri("https://cdn.azul.com/zulu/bin/zulu17.36.17-ca-jdk17.0.4.1-win_x64.zip");
                    client.DownloadProgressChanged += (send, argument) =>
                    {
                        DownloadProgress.Value = argument.ProgressPercentage;
                        ProgressPercentageLabel.Content = argument.ProgressPercentage.ToString() + @"%";
                    };
                    client.DownloadFileCompleted += (send, argument) =>
                    {
                        try
                        {
                            FastZip zip = new FastZip();
                            zip.ExtractZip("zulu.zip", Directory.GetCurrentDirectory(), null);
                            Directory.Move("zulu17.36.17-ca-jdk17.0.4.1-win_x64", "zuluJDK");
                            File.Delete("zulu.zip");
                        }
                        catch
                        {
                        }
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Visibility = Visibility.Hidden;
                    };
                    client.DownloadFileAsync(JDKlink, "zulu.zip");
                } else
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Visibility = Visibility.Hidden;
                }
            } else
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Visibility = Visibility.Hidden;
            }
            GC.Collect();
        }

        private void ReVancedDownloader(Uri downloadLink, string fileName)
        {
            WebClient client = new WebClient();
            DownloadingLabel.Content = "Downloading " + fileName;
            client.DownloadProgressChanged += (send, argument) =>
            {
                DownloadProgress.Value = argument.ProgressPercentage;
                ProgressPercentageLabel.Content = argument.ProgressPercentage.ToString() + @"%";
            };
            client.DownloadFileCompleted += (send, argument) =>
            {
                GC.Collect();
                this.Visibility = Visibility.Hidden;
            };
            client.DownloadFileAsync(downloadLink, Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced\" + fileName);
        }

        private void AppDownloader(string appName, string version)
        {
            switch (appName)
            {
                default:
                    GC.Collect();
                    this.Visibility = Visibility.Hidden;
                    break;
                case "YouTube":
                    try
                    {
                        WebClient YTClient = new WebClient();
                        HtmlDocument YTDoc = new HtmlDocument();
                        DownloadingLabel.Content = "Downloading Youtube v" + version.Replace('-','.');
                        Uri downloadYT = new Uri($"https://www.apkmirror.com/apk/google-inc/youtube/youtube-{version}-release/youtube-{version}-2-android-apk-download");
                        YTDoc.LoadHtml(YTClient.DownloadString(downloadYT));
                        var YTfindVersion = YTDoc.DocumentNode.SelectSingleNode("/html/body/div[2]/div/div[1]/article/div[2]/div[3]/div[1]/div[2]/div[2]/div/a");
                        Uri YTdownloadApp = new Uri("https://www.apkmirror.com" + YTfindVersion.Attributes["href"].Value);
                        YTDoc.LoadHtml(YTClient.DownloadString(YTdownloadApp));
                        var YTfindDownload = YTDoc.DocumentNode.SelectSingleNode("/html/body/div[2]/div/div[1]/article/div[2]/div/div/div[1]/p[2]/span/a");
                        Uri YTdownloadLink = new Uri("https://www.apkmirror.com" + YTfindDownload.Attributes["href"].Value);
                        YTClient.DownloadProgressChanged += (send, argument) =>
                        {
                            DownloadProgress.Value = argument.ProgressPercentage;
                            ProgressPercentageLabel.Content = argument.ProgressPercentage.ToString() + @"%";
                        };
                        YTClient.DownloadFileCompleted += (send, argument) =>
                        {
                            GC.Collect();
                            this.Visibility = Visibility.Hidden;
                        };
                        YTClient.DownloadFileAsync(YTdownloadLink, Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced\Apks\Youtube.apk");
                    }
                    catch
                    {
                    }
                    break;
                case "YouTubeMusic":
                    try
                    {
                        WebClient YTMClient = new WebClient();
                        HtmlDocument YTMDoc = new HtmlDocument();
                        DownloadingLabel.Content = "Downloading Youtube Music v" + version.Replace('-', '.');
                        Uri downloadYTM = new Uri($"https://www.apkmirror.com/apk/google-inc/youtube-music/youtube-music-{version}-release/youtube-music-{version}-2-android-apk-download");
                        YTMDoc.LoadHtml(YTMClient.DownloadString(downloadYTM));
                        var YTMfindVersion = YTMDoc.DocumentNode.SelectSingleNode("/html/body/div[2]/div/div[1]/article/div[2]/div[3]/div[1]/div[1]/div[2]/div/a");
                        Console.WriteLine(YTMfindVersion.Attributes["href"].Value);
                        Uri YTMdownloadApp = new Uri("https://www.apkmirror.com" + YTMfindVersion.Attributes["href"].Value);
                        Console.WriteLine(YTMdownloadApp);
                        YTMDoc.LoadHtml(YTMClient.DownloadString(YTMdownloadApp));
                        var YTMfindDownload = YTMDoc.DocumentNode.SelectSingleNode("/html/body/div[2]/div/div[1]/article/div[2]/div/div/div[1]/p[2]/span/a");
                        Console.WriteLine(YTMfindDownload.Attributes["href"].Value);
                        Uri YTMdownloadLink = new Uri("https://www.apkmirror.com" + YTMfindDownload.Attributes["href"].Value);
                        string test = "";
                        YTMClient.DownloadProgressChanged += (send, argument) =>
                        {
                            DownloadProgress.Value = argument.ProgressPercentage;
                            DownloadProgress.Value = argument.ProgressPercentage;
                            ProgressPercentageLabel.Content = argument.ProgressPercentage.ToString() + @"%";
                        };
                        YTMClient.DownloadFileCompleted += (send, argument) =>
                        {
                            GC.Collect();
                            this.Visibility = Visibility.Hidden;
                        };
                        YTMClient.DownloadFileAsync(YTMdownloadLink, Directory.GetCurrentDirectory() + @"\zuluJDK\bin\Revanced\Apks\YoutubeMusic.apk");

                    }
                    catch
                    {
                    }
                    break;
            }
        }
    }
}
