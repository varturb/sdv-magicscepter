using System;
using MagicScepter.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class CheckboxComponent : IClickableMenu
  {
    public ClickableTextureComponent ClickableComponent => checkbox;
    private readonly ClickableTextureComponent checkbox;
    private readonly string label;
    private readonly Action<bool> action;
    private readonly string hoverText;
    private readonly bool smallFont;
    private readonly bool soundOnSelect;
    private bool hovered;
    private bool isChecked;

    public CheckboxComponent(Rectangle bounds, Action<bool> action, string label, bool isChecked, string hoverText = "", bool smallFont = false, bool soundOnSelect = true)
    {
      width = bounds.Width;
      height = bounds.Height;
      xPositionOnScreen = bounds.X;
      yPositionOnScreen = bounds.Y;
      this.action = action;
      this.label = label;
      this.isChecked = isChecked;
      this.hoverText = hoverText;
      this.smallFont = smallFont;
      this.soundOnSelect = soundOnSelect;

      checkbox = new ClickableTextureComponent(
        new Rectangle(bounds.X + bounds.Width - 9 * 4 - 24, bounds.Y + bounds.Height / 2 - 18 - 8, 9 * 4, 9 * 4),
        Game1.mouseCursors,
        new Rectangle(227, 425, 9, 9),
        4f,
        true
      );
    }

    public void SetupIDs(int ID, int upID, int downID, int leftID, int rightID)
    {
      checkbox.myID = ID;
      checkbox.upNeighborID = upID;
      checkbox.downNeighborID = downID;
      checkbox.leftNeighborID = leftID;
      checkbox.rightNeighborID = rightID;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (Context.IsMainPlayer && checkbox.containsPoint(x, y))
      {
        isChecked = !isChecked;
        action(isChecked);
        if (soundOnSelect || !isChecked)
        {
          Game1.playSound("drumkit6");
        }
        base.receiveLeftClick(x, y, playSound);
      }
    }

    public override void performHoverAction(int x, int y)
    {
      checkbox.tryHover(x, y);
      hovered = checkbox.containsPoint(x, y);
      base.performHoverAction(x, y);
    }

    public override void draw(SpriteBatch b)
    {
      if (smallFont)
      {
        GameHelper.DrawSmallText(b, label, new Vector2(xPositionOnScreen, checkbox.bounds.Y));
      }
      else
      {
        GameHelper.DrawText(b, label, new Vector2(xPositionOnScreen, checkbox.bounds.Y));
      }
      checkbox.sourceRect.X = isChecked ? 236 : 227;
      checkbox.draw(b);

      if (hoverText.IsNotEmpty() && hovered) drawHoverText(b, hoverText, Game1.smallFont);

      base.draw(b);
      drawMouse(b);
    }
  }
}