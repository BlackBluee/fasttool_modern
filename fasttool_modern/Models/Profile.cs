using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Profile
{
    public string ProfileID { get; set; }
    public string ProfileName { get; set; }
    public ICollection<ButtonData> ButtonDatas { get; set; }
}

