using System;

namespace MagicScepter.Models
{
  public class WarpLocationSaveDataEntry
  {
    public string ID { get; set; }
    public int Order { get; set; }
    public bool Hidden { get; set; }
    public string HotKey { get; set; }

    public WarpLocationSaveDataEntry(string id, int order, bool hidden = false, string hotKey = null)
    {
      ID = id;
      Order = order;
      Hidden = hidden;
      HotKey = hotKey;
    }
  }
}