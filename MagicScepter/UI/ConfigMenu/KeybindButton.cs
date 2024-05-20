using MagicScepter.Constants;
using MagicScepter.Helpers;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MagicScepter.UI
{
  public class KeybindButton : ButtonBase
  {
    private readonly ConfigMenu parentMenu;
    private readonly TeleportScroll teleportScroll;

    public KeybindButton(TeleportScroll teleportScroll, ConfigMenu parentMenu)
      : base(48, 48, new Rectangle(116, 76, 12, 12), 4f, I18n.ConfigurationMenu_ButtonHover_Keybind())
    {
      this.parentMenu = parentMenu;
      this.teleportScroll = teleportScroll;
    }

    protected override void SetupTexture()
    {
      var texture = ModUtility.Helper.ModContent.Load<Texture2D>(ModConstants.SpritesheetTexturePath);
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
      Game1.activeClickableMenu = new KeybindMenu(parentMenu, teleportScroll);
    }

    protected override void Draw()
    {
      ClickableComponent.draw(
        Game1.spriteBatch, 
        Color.White, 
        GameHelper.CalculateDepth(ClickableComponent.bounds.Y)
      );
    }
  }
}