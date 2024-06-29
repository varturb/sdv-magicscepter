using System.Collections.Generic;
using MagicScepter.Helpers;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.UI
{
  public class MoveUpButton : ButtonBase
  {
    private readonly int index;
    private readonly List<TeleportScroll> teleportScrolls;
    private readonly bool skip;
    private readonly ConfigMenu parentMenu;

    public MoveUpButton(int index, List<TeleportScroll> teleportScrolls, ConfigMenu parentMenu, bool skip = false)
      : base(36, 36, new Rectangle(64, 18, 12, 12), 3f, I18n.ConfigurationMenu_ButtonHover_MoveUp())
    {
      this.index = index;
      this.teleportScrolls = teleportScrolls;
      this.skip = skip;
      this.parentMenu = parentMenu;
    }

    protected override void SetupTexture()
    {
      SetTexture(FileHelper.GetSpritesheetTexture());
    }

    public void UpdatePosition(int x, int y)
    {
      xPositionOnScreen = x;
      yPositionOnScreen = y;
      ClickableComponent.bounds = new Rectangle(x, y, width, height);
    }

    protected override void ButtonPressed()
    {
      if (skip)
        return;

      var prev = teleportScrolls[index - 1].ConvertToSaveDataEntry();
      var curr = teleportScrolls[index].ConvertToSaveDataEntry();
      (curr.Order, prev.Order) = (prev.Order, curr.Order);

      var entiresToSave = new List<SaveDataEntry> { prev, curr };

      ModDataHelper.UpdateSaveData(entiresToSave);

      parentMenu.RefreshTeleportScrolls();
      Game1.playSound("smallSelect");
      GameHelper.ShowMessage(I18n.ConfigurationMenu_MoveUp_Message(curr.Name), MessageType.Warn);
    }

    protected override void ButtonHovered(bool hovered)
    {
      if (skip)
        Hovered = false;
    }

    protected override void Draw()
    {
      if (skip)
        return;

      ClickableComponent.draw(
        Game1.spriteBatch,
        Color.White * 0.8f,
        GameHelper.CalculateDepth(ClickableComponent.bounds.Y)
      );
    }
  }
}