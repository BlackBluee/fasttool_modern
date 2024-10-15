using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Data.Sqlite;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace fasttool_modern
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        /// 

        string dbFilePath = "myDatabase.db";
        public App()
        {
            this.InitializeComponent();
            try
            {
                // Kod do połączenia z bazą danych lub wykonania zapytań
                if (!File.Exists(dbFilePath))
                {
                    // Tworzenie bazy danych
                    using (var connection = new SqliteConnection($"Data Source={dbFilePath}"))
                    {
                        connection.Open();
                        CreateDatabase(connection);
                    }

                    Console.WriteLine("Baza danych została utworzona. \n");
                }
                else
                {
                    Console.WriteLine("Baza danych już istnieje. \n");
                }

            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Błąd SQLite: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
            }
        }
        static void CreateDatabase(SqliteConnection connection)
        {
            string createDevicesTable = @"
                CREATE TABLE devices (
                    did TEXT PRIMARY KEY,   
                    model TEXT,           
                    version REAL,
                    port TEXT
                );";
            string createProfilesTable = @"
                CREATE TABLE profiles (
                    pid TEXT PRIMARY KEY,   
                    profile TEXT           
                );";
            string createButtonsTable = @"
                CREATE TABLE buttons (
                    did TEXT,
                    pid TEXT,
                    bid TEXT,
                    aid TEXT,
                    image TEXT,           
                    color TEXT,
                    FOREIGN KEY (did) REFERENCES devices(did),
                    FOREIGN KEY (pid) REFERENCES profiles(pid)
                );";
            string createActionsTable = @"
                CREATE TABLE actions (
                    aid TEXT PRIMARY KEY,   
                    type TEXT,          
                    doaction TEXT           
                );";
            using (var command = new SqliteCommand(createDevicesTable, connection)) { command.ExecuteNonQuery(); }
            using (var command = new SqliteCommand(createProfilesTable, connection)) { command.ExecuteNonQuery(); }
            using (var command = new SqliteCommand(createButtonsTable, connection)) { command.ExecuteNonQuery(); }
            using (var command = new SqliteCommand(createActionsTable, connection)) { command.ExecuteNonQuery(); }
            Console.WriteLine("Tabele zostały utworzone.");
        }


        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window m_window;
    }
}
