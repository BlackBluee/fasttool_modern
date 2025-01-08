using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Linq;
using Persistance;
using fasttool_modern.Helpers;
using fasttool_modern.Services;
using System.Diagnostics;
using Windows.Storage.Pickers;
using Windows.Storage;


namespace fasttool_modern
{
    public sealed partial class DashboardPage : Page
    {
        int modifyButton = 1;
        SerialPortManager serialPortManager = SerialPortManager.Instance;
        KeyShortcutRecorder _shortcutRecorder = KeyShortcutRecorder.Instance;
        private bool _isRecording = false;

        string selectedDid = "1";
        string selectedPid = "HOME";
        string selectedAid = string.Empty;
        string selectedImage = "image";

        public DashboardPage()
        {
            this.InitializeComponent();
            //poczatkowe dane bazy danych
            InsertDevice("1", "Model1", 1.0f, "COM1");
            InsertProfile("HOME", "Home");
            LoadProfiles();
            LoadPanel();
            //getDecive();
            LoadButton();
            ComboBoxProfiles.SelectedIndex = 0;
            bt1.IsEnabled = false;
            modifyButton = 1;
            _shortcutRecorder.SetTextBox(TextAction);
        }

        public void InsertDevice(string did, string model, float version, string port)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    context.Database.EnsureCreated();
                    context.Devices.Add(new Device { DeviceID = did, Model = model, Version = version, Port = port });
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                output.Text = e.Message;
            }
        }

        public void InsertProfile(string pid, string pname)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    context.Database.EnsureCreated();
                    context.Profiles.Add(new Profile { ProfileID = pid, ProfileName = pname });
                    for (int i = 1; i <= 6; i++)
                    {
                        context.ButtonDatas.Add(new ButtonData { DeviceID = selectedDid, ProfileID = pid, ButtonID = i.ToString() });
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                output.Text = e.Message;
            }

        }

        private void AddAction()
        {

            var selectedItem = ComboBoxType.SelectedItem as ComboBoxItem;
            string comboBoxValue = selectedItem?.Content.ToString() ?? "None";
            selectedAid = StringGenerator.GenerateRandomString(4);
            using (var context = new AppDbContext())
            {
                var stringAction = TextAction.Text;
                if(comboBoxValue == "multimedia")
                {
                    var selectedItemMultimedia = ComboBoxMultimedia.SelectedItem as ComboBoxItem;
                    string ComboBoxMultimediaValue = selectedItemMultimedia?.Content.ToString() ?? "None";
                    stringAction = ComboBoxMultimediaValue;
                }
                context.Database.EnsureCreated();
                context.Actions.Add(new Action { ActionID = selectedAid, Type = comboBoxValue, DoAction = stringAction, ButtonID = modifyButton.ToString(), ProfileID = selectedPid, Queue = 0});
                context.SaveChanges();
            }
        }
        

        //device , profile, button, action
        private void AddButton(string did, string pid, string bid, string aid, string image, string color)
        {
            using (var context = new AppDbContext())
            {
                var buttonData = context.ButtonDatas.SingleOrDefault(b => b.ButtonID == bid && b.ProfileID == pid);
                if (buttonData != null)
                {
                    buttonData.ActionID = aid;
                    buttonData.Image = image;
                    buttonData.Color = color;
                }
                else
                    context.ButtonDatas.Add(new ButtonData { DeviceID = did, ProfileID = pid, ButtonID = bid, ActionID = aid, Image = image, Color = color });
                context.SaveChanges();
            }
        }

        public void GetDecive()
        {
            using (var context = new AppDbContext())
            {
                var devices = context.Devices.ToList();
                foreach (var device in devices)
                {
                    output.Text = $"Device ID: {device.DeviceID}, Model: {device.Model}, Version: {device.Version}, Port: {device.Port}";
                }
            }
        }

        private void GetProfileID()
        {
            string choseenProfile = ComboBoxProfiles.SelectedItem as string;
            using (var context = new AppDbContext())
            {
                var profile = context.Profiles.Where(p => p.ProfileName == choseenProfile).SingleOrDefault();
                selectedPid = profile.ProfileID;
            }
        }

        private void LoadProfiles()
        {
            using (var context = new AppDbContext())
            {
                var Profiles = context.Profiles.ToList();
                foreach (var profile in Profiles)
                {
                    ComboBoxProfiles.Items.Add(profile.ProfileName);
                }
            }
        }
        private void LoadPanel() 
        {
            if(selectedPid == "HOME")
            {
                bt7.IsEnabled = false;
            }
            else
            {
                bt7.IsEnabled = true;
            }
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
                var buttons = context.ButtonDatas.Where(b => b.DeviceID == selectedDid && b.ProfileID == selectedPid).ToList();
                
                foreach (var button in buttons)
                {
                    BitmapImage bitmap = new BitmapImage(new Uri($"ms-appx:///Assets/{button.Image}.png"));
                    Microsoft.UI.Xaml.Controls.Image image1 = new Microsoft.UI.Xaml.Controls.Image();
                    if (selectedPid == "HOME")
                    {
                        serialPortManager.Send("Type:Image,Button:" + button.ButtonID.ToString() + ", location: /" + button.Image + ".bin");
                    }
                    //wzorzec factory
                    var buttonControl = GetButtonControl(button.ButtonID);
                    if (buttonControl != null)
                    {
                        image1.Source = bitmap;
                        buttonControl.Content = image1;
                    }
                }
            }
        }
        private Button GetButtonControl(string buttonId)
        {
            return buttonId switch
            {
                "1" => bt1,
                "2" => bt2,
                "3" => bt3,
                "4" => bt4,
                "5" => bt5,
                "6" => bt6,
                "7" => bt7,
                "8" => bt8,
                _ => null,
            };
        }

        private void ComboBoxProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            modifyButton = 1;
            for (int i = 1; i <= 7; i++)
            {
                var buttonControl = GetButtonControl(i.ToString());
                if (buttonControl != null)
                {
                    buttonControl.IsEnabled = true;
                }
            }
            if (selectedPid == "HOME")
            {
                bt7.IsEnabled = false;
            }
            activeText.Text = $"Button {modifyButton}";
            bt1.IsEnabled = false;
            GetProfileID();
            LoadButton();
            LoadPanel();
        }
            private void LoadButton() 
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    context.Database.EnsureCreated();
                    var button = context.ButtonDatas.Where(b => b.ButtonID == modifyButton.ToString() && b.DeviceID == selectedDid && b.ProfileID == selectedPid).SingleOrDefault();

                    if (button != null)
                    {
                        output.Text = $"Device ID: {button.DeviceID}, Profile ID: {button.ProfileID}, Button ID: {button.ButtonID}, Action ID: {button.ActionID}, Image: {button.Image}, Color: {button.Color}";
                        selectedImage = button.Image;
                        var action = context.Actions.Where(a => a.ActionID == button.ActionID).SingleOrDefault();
                        if (action != null)
                        {
                            TextAction.Text = action.DoAction ?? "Brak akcji"; // Ustaw wartość domyślną, jeśli DoAction jest NULL

                            // Przeglądanie elementów w ComboBoxType i wybieranie odpowiedniego, jeśli Type nie jest NULL
                            foreach (ComboBoxItem item in ComboBoxType.Items)
                            {
                                if (action.Type != null && item.Content.ToString() == action.Type)
                                {
                                    ComboBoxType.SelectedItem = item;
                                    break;
                                }
                            }
                            foreach (ComboBoxItem item in ComboBoxMultimedia.Items)
                            {
                                if (action.DoAction != null && item.Content.ToString() == action.DoAction)
                                {
                                    ComboBoxMultimedia.SelectedItem = item;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            TextAction.Text = "Akcja nie znaleziona";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                output.Text = e.Message;
            }
        }

        private void ComboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ComboBoxType.SelectedItem as ComboBoxItem;
            string comboBoxValue = selectedItem?.Content.ToString() ?? "Nie wybrano opcji";
            if (comboBoxValue == "open app" || comboBoxValue == "hotkey")
            {
                ComboBoxMultimedia.Visibility = Visibility.Collapsed;
                TextAction.Visibility = Visibility.Visible;
                ActionButton.Visibility = Visibility.Visible;
                if (comboBoxValue == "open app")
                {
                    ActionButton.Content = "Choose";
                }
                else if (comboBoxValue == "hotkey")
                {
                    ActionButton.Content = "Record";
                }
            }
            else if (comboBoxValue == "multimedia")
            {
                ComboBoxMultimedia.Visibility = Visibility.Visible;
                TextAction.Visibility = Visibility.Collapsed;
                ActionButton.Visibility = Visibility.Collapsed;
            }
        }

        private async void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = ComboBoxType.SelectedItem as ComboBoxItem;
            string comboBoxValue = selectedItem?.Content.ToString() ?? "Nie wybrano opcji";
            if (comboBoxValue == "open app")
            {
                var picker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.List,
                    SuggestedStartLocation = PickerLocationId.Desktop
                };

                picker.FileTypeFilter.Add("*");

                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                StorageFile file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    TextAction.Text = file.Path;
                }
                else
                {
                    TextAction.Text = "Nie wybrano pliku.";
                }
            }
            else if (comboBoxValue == "hotkey")
            {
                if (_isRecording)
            {
                _shortcutRecorder.StopRecording();
                this.KeyDown -= _shortcutRecorder.OnKeyDown;
                ActionButton.Content = "Record"; 
            }
            else
            {
                _shortcutRecorder.StartRecording();
                this.KeyDown += _shortcutRecorder.OnKeyDown;
                ActionButton.Content = "Stop"; 
            }

            _isRecording = !_isRecording; 
            }
        }


        private void SaveButton(object sender, RoutedEventArgs e)
        {
            GetProfileID();
            var selectedItem1 = ComboBoxType.SelectedItem as ComboBoxItem;
            string comboBoxValue = selectedItem1?.Content.ToString() ?? "Nie wybrano opcji";
            AddAction();
            output.Text = $"Profil ID: {selectedPid}, Przycisk:{modifyButton} , Typ: {comboBoxValue}, akcja: {TextAction.Text}, aid: {selectedAid}";
            AddButton(selectedDid, selectedPid, modifyButton.ToString(), selectedAid, selectedImage, "black");
            serialPortManager.Send("Type:Image,Button:" + modifyButton.ToString() + ", location: /"+ selectedImage + ".bin");
        }
      
        private void Activebutton_Click(object sender, RoutedEventArgs e)
        {   
            for(int i = 1; i <= 7; i++)
            {
                var buttonControl = GetButtonControl(i.ToString());
                if (buttonControl != null)
                {
                    buttonControl.IsEnabled = true;
                }
            }
            if (selectedPid == "HOME")
            {
                bt7.IsEnabled = false;
            }
            Button button = sender as Button;
            button.IsEnabled = false;
            modifyButton = Convert.ToInt32(button.Tag);
            activeText.Text = $"Button {button.Tag}";
            GetProfileID();
            LoadButton();
            LoadPanel();
        }

        private void Image_Click(object sender, RoutedEventArgs e) {
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
                BitmapImage bitmap = new BitmapImage(new Uri($"ms-appx:///Assets/{selectedImage}.png"));
                Microsoft.UI.Xaml.Controls.Image image = new Microsoft.UI.Xaml.Controls.Image();
                image.Source = bitmap;
                imgButton.Tag = selectedImage;
                var buttonControl = GetButtonControl(modifyButton.ToString());
                if (buttonControl != null)
                {
                    buttonControl.Content = image;
                }
            }
        }
    }
}
