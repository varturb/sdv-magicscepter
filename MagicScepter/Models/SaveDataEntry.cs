namespace MagicScepter.Models
{
  public class SaveDataEntry
  {
    public string ID { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public bool Hidden { get; set; }
    public string HotKey { get; set; }

    public SaveDataEntry(string id, string name, int order, bool hidden = false, string hotKey = null)
    {
      ID = id;
      Name = name;
      Order = order;
      Hidden = hidden;
      HotKey = hotKey;
    }
  }
}