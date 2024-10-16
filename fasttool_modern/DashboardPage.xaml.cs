using Microsoft.Data.Sqlite;
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
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using static System.Net.Mime.MediaTypeNames;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace fasttool_modern
{
    //C:\Users\Kacper\AppData\Local\Packages\bf3de1fd-c129-47a6-9a79-f2c72e355dd6_a0tqctefsy7vm\LocalState
    public sealed partial class DashboardPage : Page
    {
        string dbFilePath = "myDatabase.db";
        int modifyButton = 1;

        string selectedDid = "1";
        string selectedPid = "";
        string selectedBid = "";
        string selectedAid = "";
        string selectedImage = "image";



        public DashboardPage()
        {
            this.InitializeComponent();
            loadProfiles();

            try
            {
                /*
                InsertDevice("1", "model1", 1.0, "port1");
                var localFolder = ApplicationData.Current.LocalFolder;
                var wdbFilePath = Path.Combine(localFolder.Path, "myDatabase.db");

                output.Text += $"Lokalizacja pliku bazy danych: {wdbFilePath}\n";
                output.Text += viewDevices(new SqliteConnection($"Data Source={dbFilePath}\n"));*/
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Błąd SQLite: {ex.Message}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}\n");
            }
            ComboBoxProfiles.SelectedIndex = 0;
            bt1.IsEnabled = false;
            modifyButton = 1;
        }



        public void InsertDevice(string did, string model, double version, string port)
        {
            using (var connection = new SqliteConnection($"Data Source={dbFilePath}"))
            {
                connection.Open();

                string insertQuery = "INSERT INTO devices (did, model, version, port) VALUES (@did, @model, @version, @port);";

                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@did", did);
                    command.Parameters.AddWithValue("@model", model);
                    command.Parameters.AddWithValue("@version", version);
                    command.Parameters.AddWithValue("@port", port);

                    command.ExecuteNonQuery();
                }
            }
        }

        static string viewDevices(SqliteConnection connection)
        {
            connection.Open();
            string selectQuery = "SELECT model, version, port FROM devices;";
            string view = "";
            using (var command = new SqliteCommand(selectQuery, connection))
            {
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        view += $"Model: {reader.GetString(0)}\n"; 
                        view += $"Wersja: {reader.GetDouble(1)}\n"; 
                        view += $"Port: {reader.GetString(2)}\n"; 
                    }
                }
            }
            return view;
        }
        private void SaveButton(object sender, RoutedEventArgs e)
        {
            getProfileID();
            var selectedItem1 = ComboBoxType.SelectedItem as ComboBoxItem;
            string comboBoxValue = selectedItem1?.Content.ToString() ?? "Nie wybrano opcji";

            output.Text = $"Profil ID: {selectedPid}, Przycisk:{modifyButton} , Typ: {comboBoxValue}, akcja: {TextAction.Text}";

            addAction();

            addButton(selectedDid, selectedPid, modifyButton.ToString(), selectedAid, selectedImage, "null");


        }

        //device , profile, button, action
        private void addButton(string did, string pid, string bid, string aid, string image, string color)
        {
            using (var connection = new SqliteConnection($"Data Source={dbFilePath}"))
            {
                connection.Open();
                string insertQuery = "INSERT INTO buttons (did, pid, bid, aid, image, color) VALUES (@did, @pid, @bid, @aid, @image, @color);";
                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@did", did);
                    command.Parameters.AddWithValue("@pid", pid);
                    command.Parameters.AddWithValue("@bid", bid);
                    command.Parameters.AddWithValue("@aid", aid);
                    command.Parameters.AddWithValue("@image", image);
                    command.Parameters.AddWithValue("@color", color);

                    command.ExecuteNonQuery();
                }
            }
        }

        static string getButton(string did, string pid, string bid, string option)
        {
            string voption = option;
            string action = "";
            string image = "";
            string color = "";
            using (var connection = new SqliteConnection($"Data Source=myDatabase.db"))
            {
                connection.Open();
                string selectQuery = "SELECT aid, image, color FROM buttons WHERE did = @did AND pid = @pid AND bid = @bid;";
                using (var command = new SqliteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@did", did);
                    command.Parameters.AddWithValue("@pid", pid);
                    command.Parameters.AddWithValue("@bid", bid);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            action = reader.GetString(0); 
                            image = reader.GetString(1); 
                            color = reader.GetString(2); 
                        }
                    }
                }
            }
            if (voption == "action")
            {
                return action;
            }
            else if (voption == "image")
            {
                return image;
            }
            else if (voption == "color")
            {
                return color;
            }
            else
            {
                return "error";
            }

        }

        private void loadProfiles()
        {
            using (var connection = new SqliteConnection($"Data Source=myDatabase.db"))
            {
                connection.Open();
                string selectQuery = "SELECT profile FROM profiles;";
                using (var command = new SqliteCommand(selectQuery, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ComboBoxProfiles.Items.Add(reader.GetString(0)); 
                        }
                    }
                }
            }
        }

        private void getProfileID()
        {

            string profile = ComboBoxProfiles.SelectedItem as string;
            string pid = "";
            using (var connection = new SqliteConnection($"Data Source=myDatabase.db"))
            {
                connection.Open();
                string selectQuery = "SELECT pid FROM profiles WHERE profile = @profile;";
                using (var command = new SqliteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@profile", profile);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pid = reader.GetString(0); 
                        }
                    }
                }
            }
            selectedPid = pid;

        }

        private void addAction()
        {
            using (var connection = new SqliteConnection($"Data Source={dbFilePath}"))
            {
                connection.Open();
                var selectedItem = ComboBoxType.SelectedItem as ComboBoxItem;
                //selectedItem.ToString();
                string comboBoxValue = selectedItem?.Content.ToString() ?? "None";
                selectedAid = GenerateRandomString(4);
                string insertQuery = "INSERT INTO actions (aid, type, doaction) VALUES (@aid, @type, @doaction);";
                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@aid", selectedAid);
                    command.Parameters.AddWithValue("@type", comboBoxValue);
                    command.Parameters.AddWithValue("@doaction", TextAction.Text);


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

        private void activebutton_Click(object sender, RoutedEventArgs e)
        {
            bt1.IsEnabled = true;
            bt2.IsEnabled = true;
            bt3.IsEnabled = true;
            bt4.IsEnabled = true;
            bt5.IsEnabled = true;
            bt6.IsEnabled = true;
            bt7.IsEnabled = true;
            bt8.IsEnabled = true;
            Button button = sender as Button;
            button.IsEnabled = false;
            modifyButton = Convert.ToInt32(button.Tag);
            activeText.Text = $"Button {button.Tag}";
        }

        private void image_Click(object sender, RoutedEventArgs e) {
            ChooseImage newWindow = new ChooseImage();
            newWindow.Activate();
        }
    }
}
