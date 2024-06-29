using System.Collections.Generic;
using System.Linq;
using MagicScepter.Helpers;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class DialogConfigButton : IClickableMenu
  {
    private readonly Texture2D spritesheetTexture;
    private readonly ClickableTextureComponent button;
    private readonly List<TeleportScroll> teleportScrolls;

    public DialogConfigButton(List<TeleportScroll> teleportScrolls) : base(0, 0, 0, 0, true)
    {
      width = 64;
      height = 64;
      this.teleportScrolls = teleportScrolls;

      spritesheetTexture = FileHelper.GetSpritesheetTexture();

      if (Game1.activeClickableMenu is DialogueBox dialogueBox && dialogueBox.responseCC?.Count > 0)
      {
        xPositionOnScreen = dialogueBox.x + dialogueBox.width - 64 - 80;
        yPositionOnScreen = dialogueBox.y - 64 - 26;

        button = new ClickableTextureComponent(
          new Rectangle(xPositionOnScreen, yPositionOnScreen, 64, 78),
          spritesheetTexture,
          new Rectangle(0, 64, 34, 39),
          2f
        )
        {
          myID = 1000,
          downNeighborID = dialogueBox.responseCC[0].myID
        };
        dialogueBox.responseCC[0].upNeighborID = button.myID;
        dialogueBox.allClickableComponents.Add(button);
      }
      Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46);

      ModUtility.Helper.Events.Input.ButtonPressed += OnButtonPressed;
    }

    public void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
      if (e.Button == SButton.MouseLeft || e.Button == SButton.ControllerA || e.Button == SButton.ControllerX)
      {
        var x = (int)Utility.ModifyCoordinateForUIScale(e.Cursor.ScreenPixels.X);
        var y = (int)Utility.ModifyCoordinateForUIScale(e.Cursor.ScreenPixels.Y);
        if (isWithinBounds(x, y))
        {
          OpenConfigMenu();
          base.receiveLeftClick(x, y);
        }
      }
      else
      {
        HandleKeybind(e.Button);
      }
    }

    private void HandleKeybind(SButton sButton)
    {
      if (Context.IsMainPlayer && Game1.activeClickableMenu is DialogueBox dialogueBox && KeybindListener.IsKeyValid(sButton))
      {
        var tpScroll = teleportScrolls.FirstOrDefault(tp => tp.Keybind == sButton);
        if (tpScroll != null && tpScroll.CanTeleport && !tpScroll.Hidden)
        {
          dialogueBox.closeDialogue();
          tpScroll.Teleport();
        }
      }
    }

    private static void OpenConfigMenu()
    {
      if (Game1.activeClickableMenu is DialogueBox dialogueBox)
      {
        dialogueBox.closeDialogue();
        Game1.activeClickableMenu = new ConfigMenu();
        Game1.playSound("smallSelect");
      }
    }

    public override void draw(SpriteBatch b)
    {
      if (Game1.activeClickableMenu is DialogueBox dialogueBox && !dialogueBox.transitioning && button != null)
      {
        var hovered = isWithinBounds(Game1.getMouseX(), Game1.getMouseY());
        var alpha = hovered ? 1f : 0.3f;
        var white = Color.White * alpha;

        base.draw(b);
        button.draw(b, white, 1f);

        if (hovered)
        {
          drawHoverText(Game1.spriteBatch, I18n.ConfigurationMenu_Title(), Game1.smallFont);
        }

        drawMouse(b);
      }
    }
  }
}