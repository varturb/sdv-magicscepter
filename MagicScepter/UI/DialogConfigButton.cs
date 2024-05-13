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
    private readonly Texture2D emoteMenuTexture;

    public DialogConfigButton()
    {
      width = 64;
      height = 64;
      emoteMenuTexture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\EmoteMenu");

      if (Game1.activeClickableMenu is DialogueBox dialogueBox)
      {
        xPositionOnScreen = dialogueBox.x + dialogueBox.width - 64 - 80;
        yPositionOnScreen = dialogueBox.y - 64 - 22;
      }


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
      if (Game1.activeClickableMenu is DialogueBox dialogueBox && !dialogueBox.transitioning)
      {
        var hovered = isWithinBounds(Game1.getMouseX(), Game1.getMouseY());
        var alpha = hovered ? 1f : 0.3f;
        var white = Color.White * alpha;

        base.draw(b);
        b.Draw( // draw icon background
          Game1.mouseCursors,
          new Vector2(xPositionOnScreen, yPositionOnScreen),
          new Rectangle(16, 368, 16, 16),
          white,
          0.0f,
          Vector2.Zero,
          Game1.pixelZoom,
          SpriteEffects.None,
          1f
        );
        b.Draw( // draw icon
          emoteMenuTexture,
          new Vector2(xPositionOnScreen + 8, yPositionOnScreen + 16),
          new Rectangle(64, 16, 16, 16),
          white,
          0.0f,
          Vector2.Zero,
          2.7f,
          SpriteEffects.None,
          1f
        );

        if (hovered)
        {
          drawHoverText(Game1.spriteBatch, "Settings", Game1.smallFont);
        }

        drawMouse(b);
      }
    }
  }
}