using fasttool_modern.Services;
using Microsoft.UI.Xaml.Controls;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace fasttool_modern
{
    public sealed partial class ConnectionPage : Page
    {
        SerialPortManager serialPortManager = SerialPortManager.Instance;
        public ConnectionPage()
        {
            this.InitializeComponent();
            LoadDetectedPorts();
            LoadButton();
        }
        private void LoadDetectedPorts()
        {
            foreach (var port in System.IO.Ports.SerialPort.GetPortNames())
            {
                if (port != null && !comboBoxPorts.Items.Contains(port))
                {
                    comboBoxPorts.Items.Add(port);
                }
            }
            comboBoxPorts.SelectedItem = comboBoxPorts.Items.Contains(serialPortManager.getPortName()) ? serialPortManager.getPortName() : comboBoxPorts.Items[0];
        }
        private void LoadButton()
        {
            if (serialPortManager.askConnection())
            {
                connectionButton.Content = "Disconnect";
            }
            else
            {
                connectionButton.Content = "Connect";
            }
        }
    }
}
