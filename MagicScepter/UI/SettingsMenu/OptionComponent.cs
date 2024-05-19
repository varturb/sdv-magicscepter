using System;
using MagicScepter.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class OptionComponent : IClickableMenu
  {
    public ClickableTextureComponent ClickableComponent => button;
    private readonly ClickableTextureComponent button;
    private readonly string label;
    private readonly Action action;
    private readonly string hoverText;
    private bool hovered;

    public OptionComponent(Rectangle bounds, Action action, string label, string hoverText = "")
    {
      width = bounds.Width;
      height = bounds.Height;
      xPositionOnScreen = bounds.X;
      yPositionOnScreen = bounds.Y;
      this.action = action;
      this.label = label;
      this.hoverText = hoverText;

      button = new ClickableTextureComponent(
        new Rectangle(bounds.X + bounds.Width - 21 * 4, bounds.Y + bounds.Height / 2 - 22 - 8, 21 * 4, 11 * 4),
        Game1.mouseCursors,
        new Rectangle(294, 428, 21, 11),
        4f,
        true
      );
    }

    public void SetupIDs(int ID, int upID, int downID, int leftID, int rightID)
    {
      button.myID = ID;
      button.upNeighborID = upID;
      button.downNeighborID = downID;
      button.leftNeighborID = leftID;
      button.rightNeighborID = rightID;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (button.containsPoint(x, y))
      {
        action();
        base.receiveLeftClick(x, y, playSound);
      }
    }

    public override void performHoverAction(int x, int y)
    {
      button.tryHover(x, y);
      hovered = button.containsPoint(x, y);
      base.performHoverAction(x, y);
    }

    public override void draw(SpriteBatch b)
    {
      GameHelper.DrawText(b, label, new Vector2(xPositionOnScreen, button.bounds.Y));
      button.draw(b);

      if (hoverText.IsNotEmpty() && hovered) drawHoverText(b, hoverText, Game1.smallFont);

      base.draw(b);
      drawMouse(b);
    }
  }
}