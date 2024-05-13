using MagicScepter.Helpers;

namespace MagicScepter.Models
{
  public class MiniObeliskObject : WarpObject
  {
    public MiniObeliskObject(DataEntry data, int index, int x, int y) : base(data)
    {
      ID = $"{ID}#{index + 1}";
      Order += index;
      Text += $"# {index + 1}";
      WarpDoWhen.Do.Point = new WarpDoPoint(x, y);

      CanWarp = WarpHelper.CanWarp(WarpDoWhen.When);
    }
  }
}