using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Linq;
using Persistance;
using fasttool_modern.Helpers;
using fasttool_modern.Services;
using static System.Net.Mime.MediaTypeNames;

namespace fasttool_modern
{
    public sealed partial class DashboardPage : Page
    {
        int modifyButton = 1;
        SerialPortManager serialPortManager = SerialPortManager.Instance;

        string selectedDid = "1";
        string selectedPid = "HOME";
        string selectedAid = string.Empty;
        string selectedImage = "image";
        

        public DashboardPage()
        {
            this.InitializeComponent();
            //poczatkowe dane bazy danych
            //InsertDevice("1", "Model1", 1.0f, "COM1");
            //InsertProfile("HOME", "Home");
            LoadProfiles();
            LoadPanel();
            //getDecive();
            LoadButton();
            ComboBoxProfiles.SelectedIndex = 0;
            bt1.IsEnabled = false;
            modifyButton = 1;
          

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
                context.Database.EnsureCreated();
                context.Actions.Add(new Action { ActionID = selectedAid, Type = comboBoxValue, DoAction = TextAction.Text, ButtonID = modifyButton.ToString(), ProfileID = selectedPid });
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
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
                var buttons = context.ButtonDatas.Where(b => b.DeviceID == selectedDid && b.ProfileID == selectedPid).ToList();
                
                foreach (var button in buttons)
                {
                    BitmapImage bitmap = new BitmapImage(new Uri($"ms-appx:///Assets/{button.Image}.png"));
                    Microsoft.UI.Xaml.Controls.Image image1 = new Microsoft.UI.Xaml.Controls.Image();
                    //wzorzec factory
                    switch (button.ButtonID)
                    {
                        case "1":
                            image1.Source = bitmap;
                            bt1.Content = image1;
                            break;
                        case "2":
                            image1.Source = bitmap;
                            bt2.Content = image1;
                            break;
                        case "3":
                            image1.Source = bitmap;
                            bt3.Content = image1;
                            break;
                        case "4":
                            image1.Source = bitmap;
                            bt4.Content = image1;
                            break;
                        case "5":
                            image1.Source = bitmap;
                            bt5.Content = image1;
                            break;
                        case "6":
                            image1.Source = bitmap;
                            bt6.Content = image1;
                            break;
                        case "7":
                            image1.Source = bitmap;
                            bt7.Content = image1;
                            break;
                        case "8":
                            image1.Source = bitmap;
                            bt8.Content = image1;
                            break;
                    }

                }
            }

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
            GetProfileID();
            LoadButton();
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
