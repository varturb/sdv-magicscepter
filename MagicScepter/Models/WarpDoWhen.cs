using System.Collections.Generic;

namespace MagicScepter.Models
{
  public class WarpDoWhen
  {
    public WarpDo Do { get; set; }
    public List<WarpWhen> When { get; set; }
  }

  public class WarpDo
  {
    public string Type { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public WarpDoPoint Point { get; set; }
  }

  public class WarpDoPoint
  {
    public int X { get; set; }
    public int Y { get; set; }

    public WarpDoPoint() { }
    public WarpDoPoint(int x, int y)
    {
      X = x;
      Y = y;
    }
  }

  public class WarpWhen
  {
    public string Type { get; set; }
    public string Is { get; set; }
    public string IsNot { get; set; }
  }
}