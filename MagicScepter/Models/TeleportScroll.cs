using MagicScepter.Constants;
using MagicScepter.Helpers;
using Microsoft.Xna.Framework;

namespace MagicScepter.Models
{
  public class TeleportScroll
  {
    public string ID { get; protected set; }
    public int Order { get; set; }
    public string Text { get; protected set; }
    public string DefaultText { get; protected set; }
    public bool CanTeleport { get; protected set; }
    public bool Hidden { get; protected set; }
    public Rectangle SpirteSource { get; }
    public ActionDoWhen ActionDoWhen { get; set; }
    
    private const int orderOffset = 100;

    public TeleportScroll(DataEntry data)
    {
      ID = data.ID;
      SpirteSource = new(data.SpriteOffset, 0, 64, 64);
      ActionDoWhen = data.Action.DeepCopy();

      if (GetType() == typeof(TeleportScroll))
      {
        var saveEntry = ModDataHelper.GetSaveDataEntry(data.ID);
        Order = saveEntry?.Order ?? (data.Order + orderOffset);
        DefaultText = TranslationHelper.Get(data.TranslationKey);
        Text = saveEntry?.Name ?? DefaultText;
        Hidden = ActionDoWhen.Do.Type != ActionDoType.Farm && (saveEntry?.Hidden ?? false);
        CanTeleport = TeleportHelper.CanTeleport(ActionDoWhen.When);
      }
    }

    public void Teleport()
    {
      if (CanTeleport)
      {
        TeleportHelper.Teleport(ActionDoWhen.Do);
      }
    }
  }
}