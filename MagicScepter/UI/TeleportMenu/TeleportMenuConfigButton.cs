using MagicScepter.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class TeleportMenuConfigButton : IClickableMenu
  {
    private readonly Texture2D spritesheetTexture;
    private readonly ClickableTextureComponent button;

    public TeleportMenuConfigButton()
    {
      width = 36;
      height = 36;
      
      spritesheetTexture = ModUtility.Helper.ModContent.Load<Texture2D>(ModConstants.SpritesheetTexturePath);

      button = new ClickableTextureComponent(
        new Rectangle(0, 0, 36, 36),
        spritesheetTexture,
        new Rectangle(34, 64, 12, 12),
        3f
      );
    }

    public override void receiveLeftClick(int x, int y, bool playSound = false)
    {
      x = (int)Utility.ModifyCoordinateForUIScale(x);
      y = (int)Utility.ModifyCoordinateForUIScale(y);
      if (isWithinBounds(x, y))
      {
        OpenConfigMenu();
        base.receiveLeftClick(x, y);
      }
    }

    private static void OpenConfigMenu()
    {
      Game1.exitActiveMenu();
      Game1.activeClickableMenu = new ConfigMenu();
      Game1.playSound("smallSelect");
    }

    public override void draw(SpriteBatch b)
    {
      var hovered = isWithinBounds(Game1.getMouseX(), Game1.getMouseY());
      var alpha = hovered ? 1f : 0.3f;
      var white = Color.White * alpha;
      button.bounds.X = xPositionOnScreen;
      button.bounds.Y = yPositionOnScreen;
      button.draw(b, white, 1f);

      if (hovered) drawHoverText(Game1.spriteBatch, I18n.ConfigurationMenu_Title(), Game1.smallFont);

      base.draw(b);
    }
  }
}