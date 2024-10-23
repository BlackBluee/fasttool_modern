public class Action
{
    public string ActionID { get; set; }
    public int Queue { get; set; }
    public string Type { get; set; }
    public string DoAction { get; set; }

    public string ButtonID { get; set; }
    public string ProfileID { get; set; }  // Opcjonalnie, jeśli chcesz mieć referencję do profilu

    public ButtonData ButtonData { get; set; }
}

