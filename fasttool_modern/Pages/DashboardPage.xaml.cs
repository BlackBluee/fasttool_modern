using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;



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
        string selectedPid = "KLyN";
        string selectedBid = "";
        string selectedAid = "";
        string selectedImage = "image";



        public DashboardPage()
        {
            this.InitializeComponent();
            loadProfiles();
            loadButtons();

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

        private void loadButtons() 
        {
            using (var connection = new SqliteConnection($"Data Source=myDatabase.db"))
            {
                output.Text += selectedDid;
                output.Text += selectedPid;
                connection.Open();
                string selectQuery = "SELECT bid, image FROM buttons WHERE did = @did AND pid = @pid;";
                using (var command = new SqliteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@did", selectedDid);
                    command.Parameters.AddWithValue("@pid", selectedPid);
                    
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            output.Text += reader.GetString(0);
                            output.Text += reader.GetString(1);
                            var choosebt = int.Parse(reader.GetString(0)); 
                            string image = reader.GetString(1);
                            BitmapImage bitmap = new BitmapImage(new Uri($"C:/{image}.png"));
                            Microsoft.UI.Xaml.Controls.Image image1 = new Microsoft.UI.Xaml.Controls.Image();
                            switch (choosebt)
                            {
                                case 1:
                                    image1.Source = bitmap;
                                    bt1.Content = image1;
                                    break;
                                case 2:
                                    image1.Source = bitmap;
                                    bt2.Content = image1;
                                    break;
                                case 3:
                                    image1.Source = bitmap;
                                    bt3.Content = image1;
                                    break;
                                case 4:
                                    image1.Source = bitmap;
                                    bt3.Content = image1;
                                    break;
                            } 
                        }
                    }
                }
            }

        }

        private void loadButton() 
        {
            string aid = "";
            using (var connection = new SqliteConnection($"Data Source=myDatabase.db"))
            {
                
                connection.Open();
                string selectQuery = "SELECT aid FROM buttons WHERE did = @did AND pid = @pid AND bid = @bid;";
                using (var command = new SqliteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@did", selectedDid);
                    command.Parameters.AddWithValue("@pid", selectedPid);
                    command.Parameters.AddWithValue("@bid", modifyButton);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            aid = reader.GetString(0);
                        }
                    }
                }
            }
            using (var connection = new SqliteConnection($"Data Source=myDatabase.db"))
            {
                
                connection.Open();
                string selectQuery = "SELECT type, doaction FROM actions WHERE aid = @aid;";
                using (var command = new SqliteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@aid", aid);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foreach (ComboBoxItem item in ComboBoxType.Items)
                            {
                                if (item.Content.ToString() == reader.GetString(0))
                                {
                                    ComboBoxType.SelectedItem = item;
                                    break;
                                }
                            }
                            TextAction.Text = reader.GetString(1);
                        }
                    }
                }
            }
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
            loadButton();
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
            newWindow.Closed += NewWindow_Closed;

        }

        private void NewWindow_Closed(object sender, WindowEventArgs e)
        {
            ChooseImage chooseImage = sender as ChooseImage;

            if (chooseImage != null)
            {
                selectedImage = chooseImage.SelectedImagePath;
                BitmapImage bitmap = new BitmapImage(new Uri($"C:/{selectedImage}.png"));
                Microsoft.UI.Xaml.Controls.Image image = new Microsoft.UI.Xaml.Controls.Image();
                image.Source = bitmap;
                imgButton.Tag = selectedImage;
                switch (modifyButton)
                {
                    case 1:
                        bt1.Content = image;
                        break;
                    case 2:
                        bt2.Content = image;
                        break;
                    case 3:
                        bt3.Content = image;
                        break;
                    case 4:
                        bt4.Content = image;
                        break;
                    case 5:
                        bt5.Content = image;
                        break;
                    case 6:
                        bt6.Content = image;
                        break;
                    case 7:
                        bt7.Content = image;
                        break;
                    case 8:
                        bt8.Content = image;
                        break;
                }
            }
        }
    }
}
