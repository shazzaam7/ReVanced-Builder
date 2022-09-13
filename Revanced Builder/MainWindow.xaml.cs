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

namespace Revanced_Builder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GrabPatchesList();
        }


        List<Patch> patchesList = new List<Patch>();
        Dictionary<string, string> appDescription = new Dictionary<string, string>();
        List<string> ExcludedFeatures = new List<string>();
        //Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Azul Systems\Zulu\zulu-17 - Location of registry for ZULU

        //Events
        private void Youtube_Features_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YoutubeFeatures.SelectedIndex < 0)
            {
                YoutubeFeatures.SelectedIndex = 0;
            }
            if (YoutubeFeatures.Items.Count < 1)
            {
                YoutubeFeatureDescription.Text = "";
                return;
            }
            string tempName;
            YoutubeFeatureDescription.Text = "";
            try
            {
                tempName = YoutubeFeatures.SelectedItem.ToString();
            }
            catch 
            {
                return;
            }
            foreach (string key in appDescription.Keys)
            {
                if (key == tempName)
                {
                    YoutubeFeatureDescription.Text = "Feature Description\n" + appDescription[key];
                }
            }
        }

        private void YoutubeExcludedFeaturesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YoutubeExcludedFeaturesList.Items.Count < 0)
            {
                return;
            }
        }

        private void YoutubeExcludeFeatureButton_Click(object sender, RoutedEventArgs e)
        {
            string SelectedFeature;
            if (YoutubeFeatures.Items.Count < 1)
            {
                return;
            }
            try
            {
                SelectedFeature = YoutubeFeatures.SelectedItem.ToString();
            }
            catch 
            {
                return;
            }
            YoutubeExcludedFeaturesList.Items.Add(SelectedFeature);
            YoutubeFeatures.Items.Remove(SelectedFeature);
        }

        private void YoutubeIncludeFeatureButton_Click(object sender, RoutedEventArgs e)
        {
            string SelectedFeature;
            if (YoutubeExcludedFeaturesList.Items.Count < 1 || YoutubeExcludedFeaturesList.SelectedIndex < 0)
            {
                return;
            }
            try
            {
                SelectedFeature = YoutubeExcludedFeaturesList.SelectedItem.ToString();
            }
            catch
            {
                return;
            }
            YoutubeFeatures.Items.Add(SelectedFeature);
            YoutubeExcludedFeaturesList.Items.Remove(SelectedFeature);
        }


        //Methods
        private async void GrabPatchesList()
        {
            HttpClient client = new HttpClient();
            Uri patchSource = new Uri("https://raw.githubusercontent.com/revanced/revanced-patches/main/patches.json");
            string JSONAsync = await client.GetStringAsync(patchSource);
            patchesList = JsonConvert.DeserializeObject<List<Patch>>(JSONAsync);
            foreach (Patch patch in patchesList)
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
                            YoutubeFeatures.Items.Add(patch.name);
                            break;
                        case "com.google.android.apps.youtube.music":

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
}
