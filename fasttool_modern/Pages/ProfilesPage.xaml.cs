using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Data.Sqlite;
using System.Reflection;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace fasttool_modern
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfilesPage : Page
    {
        public ProfilesPage()
        {
            this.InitializeComponent();
            RemoveAllProfiles();
            loadProfiles();
        }

        private void loadProfiles()
        {
            listProfiles.Children.Clear();
            listProfiles.RowDefinitions.Clear();

            using (var context = new AppDbContext())
            {
                var profiles = context.Profiles.ToList();
                foreach (var profile in profiles)
                {
                    RowDefinition row = new RowDefinition();
                    listProfiles.RowDefinitions.Add(row);

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = profile.ProfileName;
                    Grid.SetColumn(textBlock, 0);
                    Grid.SetRow(textBlock, listProfiles.RowDefinitions.Count - 1); 

                    Button button = new Button();
                    button.Content = "Remove";
                    button.Click += RemoveRowButton_Click;
                    button.Tag = profile.ProfileID;
                    if (profile.ProfileID == "HOME")
                    {
                        button.IsEnabled = false;
                    }
                    Grid.SetColumn(button, 1);
                    Grid.SetRow(button, listProfiles.RowDefinitions.Count - 1);
                    listProfiles.Children.Add(textBlock);
                    listProfiles.Children.Add(button);
                }
            }  
        }

        private void addProfile(object sender, RoutedEventArgs e)
        {
            using (var context = new AppDbContext()) { 
                context.Database.EnsureCreated();
                string pid = GenerateRandomString(4);
                string profile = ComboBoxProfiles.SelectedItem as string;
                context.Profiles.Add(new Profile { ProfileID = pid, ProfileName = profile });
                context.SaveChanges();
            }
            loadProfiles();
        }
        private void RemoveRowButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            removeprofile(button.Tag.ToString());
            
            loadProfiles();
        }
        private void removeprofile(string pid)
        {
            using (var context = new AppDbContext())
            {
                var profileToDelete = context.Profiles.SingleOrDefault(a => a.ProfileID == pid);

                if (profileToDelete != null)
                {
                    context.Profiles.Remove(profileToDelete);
                    context.SaveChanges();
                }
            }
        }

        private void RemoveAllProfiles()
        {
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
                var profiles = context.Profiles.ToList();
                foreach (var profile in profiles)
                {
                    if (profile.ProfileID != "HOME")
                    {
                        context.Profiles.Remove(profile);
                    }
                }
            }
            loadProfiles();
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                                        .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
