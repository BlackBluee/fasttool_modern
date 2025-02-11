﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using Persistance;
using fasttool_modern.Helpers;
using fasttool_modern.Services;

namespace fasttool_modern
{
    public sealed partial class ProfilesPage : Page
    {
        string selectedDid = "1";
        ActiveWindowTracker _activeWindowTracker = ActiveWindowTracker.Instance;
        public ProfilesPage()
        {
            this.InitializeComponent();
            NewProfiles();
            RemoveAllProfiles();
            LoadProfiles();
        }

        private void NewProfiles()
        {
            using (var context = new AppDbContext())
            {
                var profiles = context.Profiles.Select(p => p.ProfileName).ToList();
                var app = Application.Current as App;
                if (app != null)
                {
                    foreach (var item in _activeWindowTracker.availableApps)
                    {
                        if (item != null && !ComboBoxProfiles.Items.Contains(item) && !profiles.Contains(item))
                        {
                            ComboBoxProfiles.Items.Add(item);
                        }
                    }

                }
            }
            
        }

        private void LoadProfiles()
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

        private void AddProfile(object sender, RoutedEventArgs e)
        {
            using (var context = new AppDbContext()) { 
                context.Database.EnsureCreated();
                string pid = StringGenerator.GenerateRandomString(4);
                string profile = ComboBoxProfiles.SelectedItem as string;
                context.Profiles.Add(new Profile { ProfileID = pid, ProfileName = profile });
                for (int i = 1; i <= 7; i++)
                {
                    context.ButtonDatas.Add(new ButtonData { DeviceID = selectedDid, ProfileID = pid, ButtonID = i.ToString() });
                }
                context.SaveChanges();
            }
            LoadProfiles();
        }
        private void RemoveRowButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            RemoveProfile(button.Tag.ToString());
            
            LoadProfiles();
        }
        private void RemoveProfile(string pid)
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
            LoadProfiles();
        }

    }
}
