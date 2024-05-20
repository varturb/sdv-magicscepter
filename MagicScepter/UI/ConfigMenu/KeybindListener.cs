using System;
using System.Collections.Generic;
using MagicScepter.Helpers;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  internal class KeybindListener : OptionsElement
  {
    private SButton value;

    private readonly Action<SButton> setValue;

    public readonly ClickableTextureComponent SetButton;

    private readonly SButton clearToButton;

    private static readonly HashSet<SButton> invalidButtons = new()
    {
        // invalid
        SButton.None,

        // buttons that would break navigation
        SButton.MouseLeft,
        SButton.MouseRight,
        SButton.LeftThumbstickDown,
        SButton.LeftThumbstickLeft,
        SButton.LeftThumbstickRight,
        SButton.LeftThumbstickUp,
        SButton.RightThumbstickDown,
        SButton.RightThumbstickLeft,
        SButton.RightThumbstickRight,
        SButton.RightThumbstickUp,
        // SButton.LeftShoulder,
        // SButton.RightShoulder
    };

    private static readonly HashSet<SButton> clearButtons = new()
    {
        // buttons that would exit menu or conflict
        SButton.Escape,
        SButton.ControllerB,
    };

    public bool IsListening { get; private set; }

    public KeybindListener(string label, Rectangle bounds, SButton value, Action<SButton> setValue, SButton clearToButton = SButton.None)
      : base(label, -1, -1, bounds.Width, bounds.Height)
    {
      base.bounds = bounds;
      SetButton = new ClickableTextureComponent(
        new Rectangle(bounds.X + bounds.Width - 21 * Game1.pixelZoom - 24, bounds.Y + bounds.Height / 3, 21 * Game1.pixelZoom, 11 * Game1.pixelZoom),
        Game1.mouseCursors,
        new Rectangle(294, 428, 21, 11),
        Game1.pixelZoom,
        true
      );

      this.value = value;
      this.setValue = setValue;
      this.clearToButton = clearToButton;
    }

    public static bool IsKeyValid(SButton sButton)
    {
      return !clearButtons.Contains(sButton) && !invalidButtons.Contains(sButton);
    }

    public override void receiveLeftClick(int x, int y)
    {
      if (greyedOut || IsListening || !SetButton.bounds.Contains(x, y))
        return;

      IsListening = true;
      Game1.playSound("breathin");
      GameMenu.forcePreventClose = true;
    }

    public override void receiveKeyPress(Keys key)
    {
      var button = key.ToSButton();

      if (greyedOut || !IsListening)
        return;

      if (Game1.options.doesInputListContain(Game1.options.actionButton, key)
          || Game1.options.doesInputListContain(Game1.options.useToolButton, key)
          || invalidButtons.Contains(button))
      {
        GameHelper.ShowMessage(I18n.KeybindMenu_Message_InvalidKey(), MessageType.Error);
        IsListening = false;
        return;
      }

      if (clearButtons.Contains(button)
        || Game1.options.doesInputListContain(Game1.options.cancelButton, key)
        || Game1.options.doesInputListContain(Game1.options.menuButton, key))
      {
        GameHelper.ShowMessage(I18n.KeybindMenu_Message_Clear(), MessageType.Warn);
        value = clearToButton;
        Game1.playSound("bigDeSelect");
      }
      else
      {
        value = button;
        Game1.playSound("coin");
      }

      setValue(value);
      IsListening = false;
      GameMenu.forcePreventClose = false;
    }

    public void Draw(SpriteBatch b)
    {
      Utility.drawTextWithShadow(
        b,
        I18n.KeybindMenu_CurrentKey(value.MapToString(includeNone: true)),
        Game1.dialogueFont,
        new Vector2(bounds.X + 28, bounds.Y + bounds.Height / 3),
        greyedOut ? Game1.textColor * 0.33f : Game1.textColor,
        1f,
        0.15f
      );
      SetButton.draw(b);

      if (IsListening)
      {
        b.Draw(
          Game1.staminaRect,
          new Rectangle(0, 0, Game1.graphics.GraphicsDevice.Viewport.Width, Game1.graphics.GraphicsDevice.Viewport.Height),
          new Rectangle(0, 0, 1, 1),
          Color.Black * 0.75f,
          0.0f,
          Vector2.Zero,
          SpriteEffects.None,
          0.999f
        );
        b.DrawString(
          Game1.dialogueFont,
          I18n.KeybindMenu_Instruction(),
          Utility.getTopLeftPositionForCenteringOnScreen(Game1.tileSize * 3, Game1.tileSize),
          Color.White,
          0.0f,
          Vector2.Zero,
          1f,
          SpriteEffects.None,
          0.9999f
        );
      }
    }
  }
}
