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
            loadProfiles();
        }

        private void loadProfiles()
        {
            listProfiles.Children.Clear();
            listProfiles.RowDefinitions.Clear();

            using (var connection = new SqliteConnection($"Data Source=myDatabase.db"))
            {
                connection.Open();
                string selectQuery = "SELECT pid, profile FROM profiles;";
                using (var command = new SqliteCommand(selectQuery, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RowDefinition row = new RowDefinition();
                            listProfiles.RowDefinitions.Add(row);
                            


                            TextBlock textBlock = new TextBlock();
                            textBlock.Text = reader.GetString(1);
                            
                            Button button = new Button();
                            button.Content = "Remove";
                            button.Click += RemoveRowButton_Click;
                            button.Tag = reader.GetString(0);
                            if (reader.GetString(0) == "HHH1")
                            {
                                button.IsEnabled = false;
                            }

                            Grid.SetColumn(textBlock, 0);
                            Grid.SetRow(textBlock, listProfiles.RowDefinitions.Count - 1); // Ustawienie wiersza na ostatni dodany wiersz


                            Grid.SetColumn(button, 1);
                            Grid.SetRow(button, listProfiles.RowDefinitions.Count - 1);

                            listProfiles.Children.Add(textBlock);
                            listProfiles.Children.Add(button);
                        }
                    }
                }
            }
        }

        private void addProfile(object sender, RoutedEventArgs e)
        {
            using (var connection = new SqliteConnection($"Data Source=myDatabase.db"))
            {
                connection.Open();
                
                string pid = GenerateRandomString(4);
                string profile = ComboBoxProfiles.SelectedItem as string;
                string insertQuery = "INSERT INTO profiles (pid, profile) VALUES (@pid, @profile);";

                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@pid", pid);
                    command.Parameters.AddWithValue("@profile", profile);

                    command.ExecuteNonQuery();
                }
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
                using (var connection = new SqliteConnection($"Data Source=myDatabase.db"))
                {
                    connection.Open();

                    
                    string profile = ComboBoxProfiles.SelectedItem as string;
                    string removeQuery = "DELETE FROM profiles WHERE pid = @pid";

                    using (var command = new SqliteCommand(removeQuery, connection))
                    {
                        command.Parameters.AddWithValue("@pid", pid);
                        

                        command.ExecuteNonQuery();
                    }

                }
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
