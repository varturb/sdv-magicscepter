using MagicScepter.Constants;
using MagicScepter.Helpers;
using Microsoft.Xna.Framework;

namespace MagicScepter.Models
{
  public class WarpObject
  {
    public string ID { get; protected set; }
    public int Order { get; set; }
    public string Text { get; protected set; }
    public string DefaultText { get; protected set; }
    public bool CanWarp { get; protected set; }
    public bool Hidden { get; protected set; }
    public Rectangle SpirteSource { get; }
    public WarpDoWhen WarpDoWhen { get; set; }
    
    private const int orderOffset = 100;

    public WarpObject(DataEntry data)
    {
      ID = data.ID;
      SpirteSource = new(data.SpriteOffset, 0, 64, 64);
      WarpDoWhen = data.Warp.DeepCopy();

      if (GetType() == typeof(WarpObject))
      {
        var saveEntry = ModDataHelper.GetWarpLocationEntry(data.ID);
        Order = saveEntry?.Order ?? (data.Order + orderOffset);
        DefaultText = TranslationHelper.Get(data.TranslationKey);
        Text = saveEntry?.Name ?? DefaultText;
        Hidden = WarpDoWhen.Do.Type != WarpDoType.Farm && (saveEntry?.Hidden ?? false);
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