using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ButtonData
{
    public string ButtonID { get; set; }
    public string DeviceID { get; set; }
    public string ProfileID { get; set; }
    public string ActionID { get; set; }
    public string Image { get; set; }
    public string Color { get; set; }

    public Device Device { get; set; }
    public Profile Profile { get; set; }
    public List<Action> Actions { get; set; }
}


