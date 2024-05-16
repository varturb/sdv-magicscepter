using MagicScepter.Constants;
using MagicScepter.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MagicScepter.UI
{
  public class VisibilityButton : ButtonBase
  {
    public new string HoverText => !Hovered ? string.Empty : isHidden ? TranslatedKeys.Hidden : TranslatedKeys.Visible;

    private readonly int index;
    private readonly ConfigMenu configMenu;
    private readonly bool skip;
    private bool isHidden;

    public VisibilityButton(int index, bool isHidden, ConfigMenu configMenu, bool skip = false)
      : base(64, 48, new Rectangle(46, 64, 64, 35), 0.8f)
    {
      this.index = index;
      this.isHidden = isHidden;
      this.configMenu = configMenu;
      this.skip = skip;
    }

    protected override void SetupTexture()
    {
      var texture = ModUtility.Helper.ModContent.Load<Texture2D>(AllConstants.SpritesheetTexturePath);
      SetTexture(texture);
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

      var teleportScrolls = configMenu.teleportScrolls;
      var entryToSave = teleportScrolls[index].ConvertToSaveDataEntry();
      entryToSave.Hidden = isHidden;

      ModDataHelper.UpdateSaveData(entryToSave);
      configMenu.RefreshTeleportScrolls();
      Game1.playSound("smallSelect");
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
        isHidden ? Color.DarkGray * 0.5f : Color.White, 
        GameHelper.CalculateDepth(ClickableComponent.bounds.Y)
      );
    }
  }
}