using System;
using MagicScepter.Constants;
using MagicScepter.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class TeleportMenuConfigButton : ClickableTextureComponent
  {
    private readonly Action<SpriteBatch, string> drawAction;

    public TeleportMenuConfigButton(int x, int y, Action<SpriteBatch, string> drawAction)
      : base(new Rectangle(x, y, 36, 36), FileHelper.GetSpritesheetTexture(), new Rectangle(116, 64, 12, 12), 3f)
    {
      this.drawAction = drawAction;
    }

    public void OnClick(int x, int y)
    {
      if (base.containsPoint(x, y))
      {
        OpenConfigMenu();
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
      var hovered = containsPoint(Game1.getMouseX(), Game1.getMouseY());
      var alpha = hovered ? 1f : 0.3f;
      var color = Color.White * alpha;

      base.draw(b, color, ModConstants.DefaultLayerDepth);
      
      if (hovered)
      {
        drawAction(b, I18n.ConfigurationMenu_Title());
      }
    }
  }
}