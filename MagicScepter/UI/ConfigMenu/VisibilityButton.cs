using MagicScepter.Constants;
using MagicScepter.Helpers;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.UI
{
  public class VisibilityButton : ButtonBase
  {
    public new string HoverText => !Hovered
      ? string.Empty
      : isHidden
        ? I18n.ConfigurationMenu_ButtonHover_Hidden()
        : I18n.ConfigurationMenu_ButtonHover_Visible();

    private readonly TeleportScroll teleportScroll;
    private readonly ConfigMenu parentMenu;
    private readonly bool skip;
    private bool isHidden;

    public VisibilityButton(TeleportScroll teleportScroll, ConfigMenu parentMenu, bool skip = false)
      : base(38, 38, new Rectangle(227, 425, 9, 9), 4f)
    {
      this.teleportScroll = teleportScroll;
      this.parentMenu = parentMenu;
      this.skip = skip;
      isHidden = teleportScroll.Hidden;
      ClickableComponent.visible = !skip;
    }

    protected override void SetupTexture()
    {
      SetTexture(Game1.mouseCursors);
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

      isHidden = !isHidden;

      var entryToSave = teleportScroll.ConvertToSaveDataEntry();
      entryToSave.Hidden = isHidden;

      ModDataHelper.UpdateSaveData(entryToSave);

      parentMenu.RefreshTeleportScrolls();

      Game1.playSound("drumkit6");
      var message = entryToSave.Hidden
        ? I18n.ConfigurationMenu_Hidden_Message(entryToSave.Name)
        : I18n.ConfigurationMenu_Visible_Message(entryToSave.Name);
      GameHelper.ShowMessage(message, MessageType.Warn);
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

      ClickableComponent.sourceRect.X = isHidden ? 227 : 236;
      ClickableComponent.draw(Game1.spriteBatch);
    }
  }
}