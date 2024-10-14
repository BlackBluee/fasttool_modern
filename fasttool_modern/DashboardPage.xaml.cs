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
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace fasttool_modern
{
    //C:\Users\Kacper\AppData\Local\Packages\bf3de1fd-c129-47a6-9a79-f2c72e355dd6_a0tqctefsy7vm\LocalState
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        string dbFilePath = "myDatabase.db";
        int modifyButton = 1;
        public DashboardPage()
        {
            this.InitializeComponent();
            
            
            try
            {
                //InsertDevice("1", "model1", 1.0, "port1");
                //var localFolder = ApplicationData.Current.LocalFolder;
                //var wdbFilePath = Path.Combine(localFolder.Path, "myDatabase.db");
                
                //OutputTextBlock.Text += $"Lokalizacja pliku bazy danych: {wdbFilePath}\n";
                //OutputTextBlock.Text += viewDevices(new SqliteConnection($"Data Source={dbFilePath}\n"));
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
                        // Odczytujemy model, wersję i port
                        view += $"Model: {reader.GetString(0)}\n"; // Indeks 0 - model
                        view += $"Wersja: {reader.GetDouble(1)}\n"; // Indeks 1 - version
                        view += $"Port: {reader.GetString(2)}\n"; // Indeks 2 - port
                    }
                }
            }
            return view;
        }
        private void SaveButton(object sender, RoutedEventArgs e)
        {
            var selectedItem = ComboBoxType.SelectedItem as ComboBoxItem;
            string comboBoxValue = selectedItem?.Content.ToString() ?? "Nie wybrano opcji";
            var selectedItem2 = ComboBoxProfiles.SelectedItem as ComboBoxItem;
            string comboBoxValue2 = selectedItem2?.Content.ToString() ?? "Nie wybrano opcji";
            output.Text = $"Profil: {comboBoxValue2}, Przycisk:{modifyButton} , Typ: {comboBoxValue}, akcja: {TextAction.Text}";
            
        }
    }
}
