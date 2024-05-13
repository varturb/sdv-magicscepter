using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class ConfigMenu : IClickableMenu
  {
    public ConfigMenu()
    {
      width = 832;
      height = 576;

      var topLeft = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
      xPositionOnScreen = (int)topLeft.X;
      yPositionOnScreen = (int)topLeft.Y + 32;
    }

    public override void draw(SpriteBatch b)
    {
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
      SpriteText.drawStringWithScrollCenteredAt(b, "Settings", xPositionOnScreen + width / 2, yPositionOnScreen - 64);
      drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen, width, height, Color.White, 4f);

      base.draw(b);
      drawMouse(b);
    }
  }
}