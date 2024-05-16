using System.Collections.Generic;

namespace MagicScepter.Models
{
  public class ActionDoWhen
  {
    public ActionDo Do { get; set; }
    public List<ActionWhen> When { get; set; }
  }

  public class ActionDo
  {
    public string Type { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public ActionDoPoint Point { get; set; }
  }

  public class ActionDoPoint
  {
    public int X { get; set; }
    public int Y { get; set; }

    public ActionDoPoint() { }
    public ActionDoPoint(int x, int y)
    {
      X = x;
      Y = y;
    }
  }

  public class ActionWhen
  {
    public string Type { get; set; }
    public string Is { get; set; }
    public string IsNot { get; set; }
  }
}