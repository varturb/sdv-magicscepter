using System;
using System.Collections.Generic;
using System.Linq;
using MagicScepter.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class DropdownComponent : OptionsElement
  {
    public ClickableComponent ClickableComponent;

    private const int dropdownHeight = 44;
    private const int dropdownArrowButtonOffset = 40;
    private int selectedOption;
    private int startingSelected;
    public bool Clicked;
    private readonly List<string> options = new();
    private readonly List<string> optionsTexts = new();
    private readonly Action<string> action;
    private Rectangle dropDownBounds;
    private Rectangle clickableBounds;
    private static Rectangle dropDownBGSource = new(433, 451, 3, 3);
    private static Rectangle dropDownButtonSource = new(437, 450, 10, 11);

    public DropdownComponent(Rectangle bounds, string label, List<KeyValuePair<string, string>> options, string selectedOption, Action<string> action)
        : base(label, bounds.X, bounds.Y, 48, dropdownHeight, -1)
    {
      this.bounds = bounds;
      this.options = options.Select(o => o.Key).ToList();
      this.optionsTexts = options.Select(o => o.Value).ToList();
      this.action = action;
      this.selectedOption = this.options.FindIndex(o => o.Equals(selectedOption));
      startingSelected = this.selectedOption;

      RecalculateBounds();
    }

    public virtual void RecalculateBounds()
    {
      var width = 48;
      foreach (var option in optionsTexts)
      {
        var labelWidth = (int)Game1.smallFont.MeasureString(option).X;
        if (labelWidth > width)
        {
          width = labelWidth;
        }
      }
      width += 24;
      dropDownBounds = new Rectangle(
        bounds.X + bounds.Width - width - dropdownArrowButtonOffset,
        bounds.Y,
        width,
        dropdownHeight * options.Count
      );
      clickableBounds = new Rectangle(
        dropDownBounds.X,
        dropDownBounds.Y,
        dropDownBounds.Width + dropdownArrowButtonOffset,
        dropdownHeight
      );
      ClickableComponent = new ClickableComponent(new Rectangle(
        dropDownBounds.X,
        dropDownBounds.Y,
        dropDownBounds.Width,
        dropdownHeight
      ), label);
    }

    public void SetupIDs(int ID, int upID, int downID, int leftID, int rightID)
    {
      ClickableComponent.myID = ID;
      ClickableComponent.upNeighborID = upID;
      ClickableComponent.downNeighborID = downID;
      ClickableComponent.leftNeighborID = leftID;
      ClickableComponent.rightNeighborID = rightID;
      ClickableComponent.fullyImmutable = true;
    }

    public override void receiveLeftClick(int x, int y)
    {
      startingSelected = selectedOption;
      if (!Clicked)
      {
        Game1.playSound("shwip");
      }
      leftClickHeld(x, y);
    }

    public override void leftClickHeld(int x, int y)
    {
      if (clickableBounds.Contains(x, y))
      {
        Clicked = true;
      }

      if (Clicked)
      {
        dropDownBounds.Y = Math.Min(dropDownBounds.Y, Game1.uiViewport.Height - dropdownHeight);
        if (!Game1.options.SnappyMenus)
        {
          selectedOption = (int)Math.Max(Math.Min((float)(y - dropDownBounds.Y) / (float)dropdownHeight, options.Count - 1), 0f);
        }
      }
    }

    public override void leftClickReleased(int x, int y)
    {
      if (options.Count > 0)
      {
        if (Clicked)
        {
          Game1.playSound("drumkit6");
        }
        Clicked = false;
        if (dropDownBounds.Contains(x, y) || (Game1.options.gamepadControls && !Game1.lastCursorMotionWasMouse))
        {
          action.Invoke(options[selectedOption]);
        }
        else
        {
          selectedOption = startingSelected;
        }
      }
    }

    public override void receiveKeyPress(Keys key)
    {
      var mousePos = Game1.getMousePosition();
      if (!Game1.options.SnappyMenus || !dropDownBounds.Contains(mousePos))
      {
        return;
      }

      if (!Clicked)
      {
        if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
        {
          selectedOption++;
          if (selectedOption >= options.Count)
          {
            selectedOption = 0;
          }

          action.Invoke(options[selectedOption]);
        }
        else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
        {
          selectedOption--;
          if (selectedOption < 0)
          {
            selectedOption = options.Count - 1;
          }

          action.Invoke(options[selectedOption]);
        }
      }
      else if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
      {
        Game1.playSound("shiny4");
        selectedOption++;
        if (selectedOption >= options.Count)
        {
          selectedOption = 0;
        }

        action.Invoke(options[selectedOption]);
      }
      else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
      {
        Game1.playSound("shiny4");
        selectedOption--;
        if (selectedOption < 0)
        {
          selectedOption = options.Count - 1;
        }

        action.Invoke(options[selectedOption]);
      }
    }

    public void Draw(SpriteBatch b)
    {
      GameHelper.DrawSmallText(b, label, new Vector2(bounds.X, bounds.Y));
      float alpha = 1f;

      if (!Clicked)
      {
        IClickableMenu.drawTextureBox(
          b,
          Game1.mouseCursors,
          dropDownBGSource,
          dropDownBounds.X,
          dropDownBounds.Y,
          dropDownBounds.Width,
          dropdownHeight,
          Color.White * alpha,
          4f,
          drawShadow: false
        );
        b.DrawString(
          Game1.smallFont,
          (selectedOption < options.Count && selectedOption >= 0) ? optionsTexts[selectedOption] : "",
          new Vector2(dropDownBounds.X + 8, dropDownBounds.Y + 8),
          Game1.textColor * alpha,
          0f,
          Vector2.Zero,
          1f,
          SpriteEffects.None,
          0.88f
        );
        b.Draw(
          Game1.mouseCursors,
          new Vector2(dropDownBounds.X + dropDownBounds.Width, dropDownBounds.Y),
          dropDownButtonSource,
          Color.White * alpha,
          0f,
          Vector2.Zero,
          4f,
          SpriteEffects.None,
          0.88f
        );
        return;
      }

      IClickableMenu.drawTextureBox(
        b,
        Game1.mouseCursors,
        dropDownBGSource,
        dropDownBounds.X,
        dropDownBounds.Y,
        dropDownBounds.Width,
        dropDownBounds.Height,
        Color.White * alpha,
        4f,
        drawShadow: false,
        0.97f
      );

      for (int i = 0; i < options.Count; i++)
      {
        if (i == selectedOption)
        {
          b.Draw(
            Game1.staminaRect,
            new Rectangle(dropDownBounds.X, dropDownBounds.Y + i * dropdownHeight, dropDownBounds.Width, 44),
            new Rectangle(0, 0, 1, 1),
            Color.Wheat,
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            0.975f
          );
        }
        b.DrawString(
          Game1.smallFont,
          optionsTexts[i],
          new Vector2(dropDownBounds.X + 8, dropDownBounds.Y + 8 + dropdownHeight * i),
          Game1.textColor * alpha,
          0f,
          Vector2.Zero,
          1f,
          SpriteEffects.None,
          0.98f
        );
      }

      b.Draw(
        Game1.mouseCursors,
        new Vector2(dropDownBounds.X + dropDownBounds.Width, dropDownBounds.Y),
        dropDownButtonSource,
        Color.Wheat * alpha,
        0f,
        Vector2.Zero,
        4f,
        SpriteEffects.None,
        0.981f
      );

    }
  }
}
