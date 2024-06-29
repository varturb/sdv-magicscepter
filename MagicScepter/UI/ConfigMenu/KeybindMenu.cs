using MagicScepter.Constants;
using MagicScepter.Helpers;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class KeybindMenu : IClickableMenu
  {
    private readonly Texture2D spritesheetTexture;
    private readonly TeleportScroll teleportScroll;
    private const int setButtonID = 100;

    private KeybindListener keyListener;

    public KeybindMenu(TeleportScroll teleportScroll)
    {
      width = 640;
      height = 128;

      this.teleportScroll = teleportScroll;

      exitFunction = delegate
      {
        Game1.activeClickableMenu = new ConfigMenu();
      };

      spritesheetTexture = FileHelper.GetSpritesheetTexture();
      
      SetPosition();
      CreateComponents();

      if (Game1.options.SnappyMenus)
      {
        currentlySnappedComponent = getComponentWithID(keyListener.SetButton.myID);
        snapCursorToCurrentSnappedComponent();
      }
    }

    private void SetPosition()
    {
      var topLeft = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
      xPositionOnScreen = (int)topLeft.X;
      yPositionOnScreen = (int)topLeft.Y + 32;
    }

    private void CreateComponents()
    {
      keyListener = new KeybindListener(
        teleportScroll.ID,
        teleportScroll.Text,
        new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height),
        teleportScroll.Keybind,
        setValue: SetKeybind,
        clearToButton: SButton.None
      );
      keyListener.SetButton.myID = setButtonID;
      keyListener.SetButton.upNeighborID = upperRightCloseButton_ID;
      keyListener.SetButton.rightNeighborID = upperRightCloseButton_ID;
      keyListener.SetButton.fullyImmutable = true;

      initializeUpperRightCloseButton();
      upperRightCloseButton.myID = upperRightCloseButton_ID;
      upperRightCloseButton.downNeighborID = setButtonID;
      upperRightCloseButton.leftNeighborID = setButtonID;
      upperRightCloseButton.fullyImmutable = true;

      populateClickableComponentList();
    }

    private void SetKeybind(SButton key)
    {

      teleportScroll.Keybind = key;
      var entryToSave = teleportScroll.ConvertToSaveDataEntry();
      ModDataHelper.UpdateSaveData(entryToSave);

      if (key != SButton.None)
      {
        var message = I18n.KeybindMenu_Message_Success(entryToSave.Keybind, teleportScroll.Text);
        GameHelper.ShowMessage(message, MessageType.Success);

        exitThisMenu();
      }
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      SetPosition();
      CreateComponents();
    }

    public override void populateClickableComponentList()
    {

      allClickableComponents ??= new();
      allClickableComponents.Clear();
      allClickableComponents.Add(keyListener.SetButton);
      allClickableComponents.Add(upperRightCloseButton);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      base.receiveLeftClick(x, y, playSound);
      keyListener.receiveLeftClick(x, y);
    }

    public override void receiveKeyPress(Keys key)
    {
      keyListener.receiveKeyPress(key);
      if (!keyListener.IsListening)
      {
        base.receiveKeyPress(key);
      }
    }

    public override void draw(SpriteBatch b)
    {
      // draw faded background
      GameHelper.DrawFadedBackground(b, 0.2f);
      // draw menu title
      GameHelper.DrawTextInScroll(b, teleportScroll.Text, xPositionOnScreen + width / 2, yPositionOnScreen - 64);

      // draw menu box
      drawTextureBox(
        b,
        spritesheetTexture,
        new Rectangle(98, 64, 18, 18),
        xPositionOnScreen,
        yPositionOnScreen,
        width,
        height,
        Color.White,
        4f
      );

      upperRightCloseButton.visible = !keyListener.IsListening;

      keyListener.Draw(b);

      base.draw(b);
      drawMouse(b);
    }
  }
}