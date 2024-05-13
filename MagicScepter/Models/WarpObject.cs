using MagicScepter.Helpers;
using Microsoft.Xna.Framework;

namespace MagicScepter.Models
{
  public class WarpObject
  {
    public string ID { get; protected set; }
    public int Order { get; protected set; }
    public string Text { get; protected set; }
    public bool CanWarp { get; protected set; }
    public Rectangle SpirteSource { get; }
    public WarpDoWhen WarpDoWhen { get; set; }

    public WarpObject(DataEntry data)
    {
      ID = data.ID;
      Order = data.Order;
      Text = ModUtility.Helper.Translation.Get(data.TranslationKey);
      SpirteSource = new(data.SpriteOffset, 0, 64, 64);
      WarpDoWhen = data.Warp.DeepCopy();

      if (GetType() == typeof(WarpObject))
      {
        CanWarp = WarpHelper.CanWarp(WarpDoWhen.When);
      }
    }

    public void Warp()
    {
      if (CanWarp)
      {
        WarpHelper.Warp(WarpDoWhen.Do);
      }
    }
  }
}