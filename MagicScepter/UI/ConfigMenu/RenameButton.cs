using MagicScepter.Constants;
using MagicScepter.Helpers;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MagicScepter.UI
{
  public class RenameButton : ButtonBase
  {
    private readonly TeleportScroll teleportScroll;

    public RenameButton(TeleportScroll teleportScroll)
      : base(40, 40, new Rectangle(64, 16, 16, 16), 2.5f, I18n.ConfigurationMenu_ButtonHover_Rename())
    {
      this.teleportScroll = teleportScroll;
    }

    protected override void SetupTexture()
    {
      var texture = Game1.temporaryContent.Load<Texture2D>(ModConstants.EmoteMenuTexturePath);
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
      Game1.playSound("smallSelect");
      Game1.activeClickableMenu = new RenameMenu(teleportScroll);
    }

    protected override void Draw()
    {
      ClickableComponent.draw(
        Game1.spriteBatch, 
        Color.White * 0.8f, 
        GameHelper.CalculateDepth(ClickableComponent.bounds.Y)
      );
    }
  }
}