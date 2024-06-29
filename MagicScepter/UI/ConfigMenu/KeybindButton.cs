using System;
using MagicScepter.Constants;
using MagicScepter.Helpers;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace MagicScepter.UI
{
  public class KeybindButton : ButtonBase
  {
    private readonly TeleportScroll teleportScroll;

    public KeybindButton(TeleportScroll teleportScroll)
      : base(48, 48, new Rectangle(116, 76, 12, 12), 4f, I18n.ConfigurationMenu_ButtonHover_Keybind())
    {
      this.teleportScroll = teleportScroll;

      if (teleportScroll.Keybind != SButton.None)
      {
        var currentKey = teleportScroll.Keybind.MapToString(false);
        if (currentKey.IsNotEmpty())
        {
          HoverText = I18n.ConfigurationMenu_ButtonHover_Keybind()
                  + Environment.NewLine
                  + I18n.ConfigurationMenu_ButtonHover_KeybindCurrent(currentKey);
        }
      }
    }

    protected override void SetupTexture()
    {
      SetTexture(FileHelper.GetSpritesheetTexture());
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
      Game1.activeClickableMenu = new KeybindMenu(teleportScroll);
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