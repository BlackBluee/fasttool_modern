using System.Collections.Generic;

public class Profile
{
    public string ProfileID { get; set; }
    public string ProfileName { get; set; }
    public ICollection<ButtonData> ButtonDatas { get; set; }
}

