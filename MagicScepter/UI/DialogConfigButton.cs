using MagicScepter.Constants;
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

    public DialogConfigButton(): base(0, 0, 0, 0, true)
    {
      width = 64;
      height = 64;
      spritesheetTexture = ModUtility.Helper.ModContent.Load<Texture2D>(AllConstants.SpritesheetTexturePath);

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
      if (e.Button == SButton.MouseLeft || e.Button == SButton.ControllerA)
      {
        var x = (int)Utility.ModifyCoordinateForUIScale(e.Cursor.ScreenPixels.X);
        var y = (int)Utility.ModifyCoordinateForUIScale(e.Cursor.ScreenPixels.Y);
        if (isWithinBounds(x, y))
        {
          OpenConfigMenu();
          base.receiveLeftClick(x, y);
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
          drawHoverText(Game1.spriteBatch, TranslatedKeys.Configuration, Game1.smallFont);
        }

        drawMouse(b);
      }
    }
  }
}