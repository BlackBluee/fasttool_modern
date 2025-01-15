using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel;


namespace fasttool_modern
{
    public sealed partial class ChooseImage : Window
    {
        public string SelectedImagePath { get; private set; }
        List<string> buttonListapp = new List<string>();
        List<string> buttonListstandart = new List<string>();

        public ChooseImage()
        {
            this.InitializeComponent();
            LoadImagesFromFolder("Assets/images/apps", buttonListapp);
            CreateButtonsDynamically(buttonListapp, "images/apps/");
            LoadImagesFromFolder("Assets/images/standarts", buttonListstandart);
            CreateButtonsDynamically(buttonListstandart, "images/standarts/");
        }

        private void LoadImagesFromFolder(string relativePath, List<string> buttonList)
        {
            try
            {
                string folderPath = Path.Combine(Package.Current.InstalledLocation.Path, relativePath);
                string[] imageFiles = Directory.GetFiles(folderPath, "*.png");

                foreach (string filePath in imageFiles)
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    buttonList.Add(fileNameWithoutExtension);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void CreateButtonsDynamically(List<string> buttonList, string path)
        {
            for (int i = 0; i < 20; i++)
            {
                RowDefinition row = new RowDefinition();
                sPanel.RowDefinitions.Add(row);
            }
            for (int i = 0; i < 20; i++)
            {
                RowDefinition row = new RowDefinition();
                aPanel.RowDefinitions.Add(row);
            }

            for (int i = 0; i < buttonList.Count; i++)
            {
                string imageName = buttonList[i];
                Button btn = new Button();
                Image image = new Image();
                btn.Height = 98;
                btn.Width = 98;

                btn.Margin = new Thickness(15);
                try
                {
                    BitmapImage bitmap = new BitmapImage(new Uri($"ms-appx:///Assets/{path}{imageName}.png"));
                    btn.Content = $"{path}{imageName}";
                    image.Source = bitmap;
                    btn.Content = image;
                }
                catch (Exception ex)
                {
                    outText.Text = ($"B³¹d ³adowania obrazu {imageName}: {ex.Message}");
                }

                Grid.SetColumn(btn, i % 6);
                Grid.SetRow(btn, i / 6);

                btn.Tag = imageName;
                btn.Click += Button_Click;
                if (path == "images/apps/")
                    aPanel.Children.Add(btn);
                else if (path == "images/standarts/")
                {
                    sPanel.Children.Add(btn);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var ipath = "";
            var selectedTab = ItabView.SelectedItem as TabViewItem;
            if (selectedTab != null)
            {
                ipath = selectedTab.Tag.ToString();
            }

            Button button = sender as Button;
            var ifile = button.Tag.ToString();

            SelectedImagePath = ipath + ifile;
            this.Close();
        }
    }
}
