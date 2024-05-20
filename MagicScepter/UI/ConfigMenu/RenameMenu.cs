using MagicScepter.Helpers;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class RenameMenu : IClickableMenu
  {
    public ClickableTextureComponent saveButton;
    public ClickableTextureComponent resetButton;
    public ClickableTextureComponent resetButtonIcon;
    public ClickableTextureComponent cancelButton;

    private TextBox textBox;
    private ClickableComponent textBoxCC;
    private TextBoxEvent e;
    private readonly ConfigMenu parentMenu;
    private readonly TeleportScroll teleportScroll;
    private string hoverText = string.Empty;
    private const int minLength = 1;
    private const int textBoxID = 100;
    private const int resetButtonID = 101;
    private const int saveButtonID = 102;
    private const int cancelButtonID = 103;
    public RenameMenu(TeleportScroll teleportScroll, ConfigMenu parentMenu)
    {
      this.parentMenu = parentMenu;
      this.teleportScroll = teleportScroll;

      SetPosition();
      CreateComponents();

      if (Game1.options.SnappyMenus)
      {
        snapToDefaultClickableComponent();
      }
    }

    private void SetPosition()
    {
      xPositionOnScreen = 0;
      yPositionOnScreen = 0;
      width = (int)Utility.ModifyCoordinateForUIScale(Game1.viewport.Width);
      height = (int)Utility.ModifyCoordinateForUIScale(Game1.viewport.Height);
    }

    private void CreateComponents()
    {
      textBox = new TextBox(null, null, Game1.dialogueFont, Game1.textColor);
      textBox.X = width / 2 - 400 / 2;
      textBox.Y = height / 2;
      textBox.Width = 400;
      textBox.Height = 192;
      e = TextBoxEnter;
      textBox.OnEnterPressed += e;
      textBox.Text = teleportScroll.Text;
      Game1.keyboardDispatcher.Subscriber = textBox;
      textBox.Selected = true;

      textBoxCC = new ClickableComponent(
        new Rectangle(textBox.X, textBox.Y, textBox.Width, 64),
        string.Empty
      )
      {
        myID = textBoxID,
        rightNeighborID = saveButtonID,
        leftNeighborID = resetButtonID,
        fullyImmutable = true
      };
      saveButton = new ClickableTextureComponent(
        new Rectangle(textBox.X + textBox.Width + 32 + 4, Game1.uiViewport.Height / 2 - 8, 64, 64),
        Game1.mouseCursors,
        Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46),
        1f
      )
      {
        myID = saveButtonID,
        rightNeighborID = cancelButtonID,
        leftNeighborID = textBoxID,
        fullyImmutable = true
      };
      cancelButton = new ClickableTextureComponent(
        new Rectangle(textBox.X + textBox.Width + 32 + 8 + 64, Game1.uiViewport.Height / 2 - 8, 64, 64),
        Game1.mouseCursors,
        Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47),
        1f
      )
      {
        myID = cancelButtonID,
        leftNeighborID = saveButtonID,
        fullyImmutable = true
      };
      resetButton = new ClickableTextureComponent(
        new Rectangle(textBox.X - 32 + 12 - 64, Game1.uiViewport.Height / 2 - 8, 64, 64),
        Game1.mouseCursors2,
        new Rectangle(64, 208, 16, 16),
        4f
      )
      {
        myID = resetButtonID,
        rightNeighborID = textBoxID,
        fullyImmutable = true
      };
      resetButtonIcon = new ClickableTextureComponent(
        new Rectangle(textBox.X - 32 + 12 - 64, Game1.uiViewport.Height / 2 - 8, 64, 64),
        Game1.mouseCursors2,
        new Rectangle(64, 240, 16, 16),
        4f
      );

      populateClickableComponentList();
    }

    private void TextBoxEnter(TextBox sender)
    {
      if (sender.Text.Length >= minLength)
      {
        if (textBox.Selected == true)
        {
          textBox.Selected = false;
        }
        else
        {
          var entryToSave = teleportScroll.ConvertToSaveDataEntry();
          entryToSave.Name = textBox.Text;

          ModDataHelper.UpdateSaveData(entryToSave);

          if (teleportScroll.Text != entryToSave.Name)
          {
            GameHelper.ShowMessage(I18n.RenameMenu_Message_Success(entryToSave.Name), MessageType.Success);
          }

          exitThisMenu();
          parentMenu.RefreshTeleportScrolls();
          Game1.activeClickableMenu = parentMenu;
        }
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
      allClickableComponents.Add(textBoxCC);
      allClickableComponents.Add(resetButton);
      allClickableComponents.Add(saveButton);
      allClickableComponents.Add(cancelButton);
    }

    public override void snapToDefaultClickableComponent()
    {
      currentlySnappedComponent = getComponentWithID(textBoxID);
      snapCursorToCurrentSnappedComponent();
    }

    public override void receiveGamePadButton(Buttons b)
    {
      base.receiveGamePadButton(b);
      if (textBox.Selected)
      {
        switch (b)
        {
          case Buttons.DPadUp:
          case Buttons.DPadDown:
          case Buttons.DPadLeft:
          case Buttons.DPadRight:
          case Buttons.LeftThumbstickLeft:
          case Buttons.LeftThumbstickUp:
          case Buttons.LeftThumbstickDown:
          case Buttons.LeftThumbstickRight:
            textBox.Selected = false;
            break;
        }
      }
    }

    public override void receiveKeyPress(Keys key)
    {
      if (Game1.options.doesInputListContain(Game1.options.menuButton, key))
      {
        exitThisMenu();
        Game1.activeClickableMenu = parentMenu;
      }
      else if (!textBox.Selected && !Game1.options.doesInputListContain(Game1.options.menuButton, key))
      {
        base.receiveKeyPress(key);
      }
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      hoverText = string.Empty;

      saveButton?.tryHover(x, y);
      if (saveButton?.containsPoint(x, y) ?? false) hoverText = I18n.Common_Save();

      cancelButton?.tryHover(x, y);
      if (cancelButton?.containsPoint(x, y) ?? false) hoverText = I18n.Common_Cancel();

      resetButton?.tryHover(x, y, 0.4f);
      resetButtonIcon?.tryHover(x, y, 0.4f);
      if (resetButton?.containsPoint(x, y) ?? false) hoverText = I18n.RenameMenu_ResetToDefault();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      base.receiveLeftClick(x, y, playSound);
      textBox.Update();

      if (saveButton.containsPoint(x, y))
      {
        TextBoxEnter(textBox);
      }
      if (cancelButton.containsPoint(x, y))
      {
        exitThisMenu();
        Game1.activeClickableMenu = parentMenu;
      }
      if (resetButton.containsPoint(x, y))
      {
        Game1.playSound("smallSelect");
        textBox.Text = teleportScroll.DefaultText;
        GameHelper.ShowMessage(I18n.RenameMenu_Message_Default(), MessageType.Warn);
      }
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);

      GameHelper.DrawFadedBackground(b, 0.2f);
      
      SpriteText.drawStringWithScrollCenteredAt(
        b,
        I18n.RenameMenu_Title(),
        Game1.uiViewport.Width / 2,
        Game1.uiViewport.Height / 2 - 86,
        I18n.RenameMenu_Title()
      );
      textBox.Draw(b);

      saveButton.draw(b);
      cancelButton.draw(b);
      resetButton.draw(b);
      resetButtonIcon.draw(b);

      if (!hoverText.IsEmpty()) drawHoverText(b, hoverText, Game1.smallFont);

      drawMouse(b);
    }
  }
}