using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class WarpMenuConfigButton : IClickableMenu
  {

    private readonly Texture2D emoteMenuTexture;

    public WarpMenuConfigButton()
    {
      width = 64;
      height = 64;
      emoteMenuTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\EmoteMenu");
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

      b.Draw( // draw icon background
        Game1.mouseCursors2,
        new Vector2(xPositionOnScreen, yPositionOnScreen),
        new Rectangle(64, 208, 16, 16),
        white,
        0.0f,
        Vector2.Zero,
        Game1.pixelZoom,
        SpriteEffects.None,
        1f
      );
      b.Draw( // draw icon
        emoteMenuTexture,
        new Vector2(xPositionOnScreen + 12, yPositionOnScreen + 12),
        new Rectangle(64, 16, 16, 16),
        white,
        0.0f,
        Vector2.Zero,
        2.5f,
        SpriteEffects.None,
        1f
      );

      if (hovered)
      {
        drawHoverText(Game1.spriteBatch, "Settings", Game1.smallFont);
      }

      base.draw(b);
    }
  }
}