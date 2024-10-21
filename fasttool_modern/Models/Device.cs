using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Device
{
    public string DeviceID { get; set; }
    public string Model { get; set; }
    public float Version { get; set; }
    public string Port { get; set; }
    public List<ButtonData> ButtonDatas { get; set; }
}
