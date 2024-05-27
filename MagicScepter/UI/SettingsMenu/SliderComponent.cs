using System;
using MagicScepter.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class SliderComponent : IClickableMenu
  {
    public ClickableComponent ClickableComponent => sliderComponent;
    private readonly ClickableComponent sliderComponent;
    private int actionValue;
    private readonly Action<int> action;
    private readonly string label;
    private readonly int min;
    private readonly int max;
    private readonly int interval;
    private readonly bool isFloat;
    private bool isDragging = false;
    private readonly Rectangle sliderRect;

    public SliderComponent(Rectangle bounds, int min, int max, int value, Action<int> action, string label, int interval, bool isFloat = false)
    {
      xPositionOnScreen = bounds.X;
      yPositionOnScreen = bounds.Y;
      width = bounds.Width;
      height = bounds.Height;
      this.action = action;
      this.label = label;
      this.min = min;
      this.max = max;
      this.interval = interval;
      this.isFloat = isFloat;
      actionValue = value;
      sliderRect = new(xPositionOnScreen + width - width / 3, yPositionOnScreen + 8, width / 3, 24);

      sliderComponent = new ClickableComponent(
        sliderRect,
        label
      );
    }

    public void SetupIDs(int ID, int upID, int downID, int leftID, int rightID)
    {
      sliderComponent.myID = ID;
      sliderComponent.upNeighborID = upID;
      sliderComponent.downNeighborID = downID;
      sliderComponent.leftNeighborID = leftID;
      sliderComponent.rightNeighborID = rightID;
    }

    private void EmitValue()
    {
      action(actionValue);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (sliderComponent.bounds.Contains(new Point(x, y)))
      {
        isDragging = true;
      }
      base.receiveLeftClick(x, y);
      leftClickHeld(x, y);
    }

    public override void releaseLeftClick(int x, int y)
    {
      isDragging = false;
      base.releaseLeftClick(x, y);
    }

    public override void leftClickHeld(int x, int y)
    {
      base.leftClickHeld(x, y);

      if (isDragging)
      {
        var perc = (x - sliderRect.X) / (float)sliderRect.Width;
        actionValue = (int)(perc * (max - min) + min).Clamp(min, max).Adjust(interval);

        EmitValue();
      }
    }

    public override void receiveKeyPress(Keys key)
    {
      base.receiveKeyPress(key);

      var mousePos = Game1.getMousePosition();
      if (sliderComponent.bounds.Contains(mousePos))
      {
        if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
        {
          actionValue = actionValue.Adjust(interval);
          actionValue = (actionValue + interval).Clamp(min, max);
          EmitValue();
        }
        else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
        {
          actionValue = actionValue.Adjust(interval);
          actionValue = (actionValue - interval).Clamp(min, max);
          EmitValue();
        }
      }
    }

    public override void draw(SpriteBatch b)
    {
      var valueLabel = (isFloat ? actionValue.ToFloat(100) : actionValue).ToString().PadLeft(5, ' ');
      GameHelper.DrawSmallText(b, label, new Vector2(xPositionOnScreen, yPositionOnScreen + 4));
      GameHelper.DrawSmallText(b, valueLabel, new Vector2(sliderRect.X - 64, yPositionOnScreen + 4));

      drawTextureBox(
        b,
        Game1.mouseCursors,
        new(403, 383, 6, 6),
        sliderRect.X,
        sliderRect.Y,
        sliderRect.Width,
        sliderRect.Height,
        Color.White,
        4f,
        false
      );

      var runnerPosition = new Vector2(
        sliderRect.X + (sliderRect.Width - 40) * actionValue.ToPercentage(min, max),
        sliderRect.Y
      );
      b.Draw(
        Game1.mouseCursors,
        runnerPosition,
        new(420, 441, 10, 6),
        Color.White,
        0.0f,
        Vector2.Zero,
        4f,
        SpriteEffects.None,
        0.9f
      );

      base.draw(b);
    }
  }
}