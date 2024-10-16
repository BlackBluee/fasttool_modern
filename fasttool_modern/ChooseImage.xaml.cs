using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace fasttool_modern
{
    public sealed partial class ChooseImage : Window
    {
        List<string> buttonListapp = new List<string>();
        List<string> buttonListstandart = new List<string>();
        public ChooseImage()
        {
            this.InitializeComponent();
            LoadImagesFromFolder("images/apps/", buttonListapp);
            CreateButtonsDynamically(buttonListapp, "images/apps/");
            LoadImagesFromFolder("images/standarts/", buttonListstandart);
            CreateButtonsDynamically(buttonListstandart, "images/standarts/");
        }
        private void LoadImagesFromFolder(string path, List<string> buttonList)
        {
            string folderPath = path; 
            try
            {
                string[] imageFiles = Directory.GetFiles(folderPath, "*.png"); 
                foreach (string filePath in imageFiles)
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    buttonList.Add(fileNameWithoutExtension); 
                }
            }
            catch (Exception ex)
            {
                outText.Text = ($"B³¹d ³adowania plików: {ex.Message}");
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
                    BitmapImage bitmap = new BitmapImage(new Uri($"C:/{path}{imageName}.png"));
                    image.Source = bitmap;
                    btn.Content = image;
                }
                catch (Exception ex)
                {
                    outText.Text= ($"B³¹d ³adowania obrazu {imageName}: {ex.Message}");
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
            Button button = sender as Button;

        }
    }
}
