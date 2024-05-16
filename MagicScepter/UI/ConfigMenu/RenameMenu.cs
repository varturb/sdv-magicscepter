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
  public class RenameMenu : IClickableMenu
  {
    public ClickableTextureComponent saveButton;
    public ClickableTextureComponent resetButton;
    public ClickableTextureComponent resetButtonIcon;
    public ClickableTextureComponent cancelButton;

    protected TextBox textBox;

    public ClickableComponent textBoxCC;

    private TextBoxEvent e;

    protected int minLength = 1;

    private ConfigMenu parentMenu;

    private WarpObject warpObject;

    private string hoverText = string.Empty;

    public RenameMenu(ConfigMenu parentMenu, WarpObject warpOption)
    {
      this.parentMenu = parentMenu;
      warpObject = warpOption;

      xPositionOnScreen = 0;
      yPositionOnScreen = 0;
      width = Game1.uiViewport.Width;
      height = Game1.uiViewport.Height;

      textBox = new TextBox(null, null, Game1.dialogueFont, Game1.textColor);
      textBox.X = width / 2 - 400 / 2;
      textBox.Y = height / 2;
      textBox.Width = 400;
      textBox.Height = 192;
      e = TextBoxEnter;
      textBox.OnEnterPressed += e;
      Game1.keyboardDispatcher.Subscriber = textBox;
      textBox.Text = warpOption.Text;
      textBox.Selected = true;

      textBoxCC = new ClickableComponent(
        new Rectangle(textBox.X, textBox.Y, textBox.Width, 64),
        string.Empty
      )
      {
        myID = 100,
        rightNeighborID = 101,
        leftNeighborID = 103
      };
      saveButton = new ClickableTextureComponent(
        new Rectangle(textBox.X + textBox.Width + 32 + 4, Game1.uiViewport.Height / 2 - 8, 64, 64),
        Game1.mouseCursors,
        Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46),
        1f
      )
      {
        myID = 101,
        rightNeighborID = 102,
        leftNeighborID = 100
      };
      cancelButton = new ClickableTextureComponent(
        new Rectangle(textBox.X + textBox.Width + 32 + 4 + 64, Game1.uiViewport.Height / 2 - 8, 64, 64),
        Game1.mouseCursors,
        Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47),
        1f
      )
      {
        myID = 102,
        leftNeighborID = 101
      };
      resetButton = new ClickableTextureComponent(
        new Rectangle(textBox.X - 32 + 8 - 64, Game1.uiViewport.Height / 2 - 8, 64, 64),
        Game1.mouseCursors2,
        new Rectangle(64, 208, 16, 16),
        4f
      )
      {
        myID = 103,
        rightNeighborID = 100
      };
      resetButtonIcon = new ClickableTextureComponent(
        new Rectangle(textBox.X - 32 + 8 - 64, Game1.uiViewport.Height / 2 - 8, 64, 64),
        Game1.mouseCursors2,
        new Rectangle(64, 240, 16, 16),
        4f
      );

      if (Game1.options.SnappyMenus)
      {
        base.populateClickableComponentList();
        snapToDefaultClickableComponent();
      }
    }

    public override void snapToDefaultClickableComponent()
    {
      currentlySnappedComponent = getComponentWithID(100);
      snapCursorToCurrentSnappedComponent();
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
          if (Context.IsMainPlayer)
          {
            var entryToSave = warpObject.ConvertToSaveDataEntry();
            entryToSave.Name = textBox.Text;
            ModDataHelper.UpdateSaveData(entryToSave);
          }
          else
          {
            // Notify the MasterPlayer of name change
            // var updateMessage = new ObeliskUpdateMessage(this.obelisk);
            // ModEntry.helper.Multiplayer.SendMessage(updateMessage, nameof(ObeliskUpdateMessage), modIDs: new[] { ModEntry.manifest.UniqueID });
          }
          exitThisMenu();

          parentMenu.RefreshWarpObjects();
          Game1.activeClickableMenu = parentMenu;
        }
      }
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
      if (saveButton?.containsPoint(x, y) ?? false) hoverText = TranslatedKeys.Save;

      cancelButton?.tryHover(x, y);
      if (cancelButton?.containsPoint(x, y) ?? false) hoverText = TranslatedKeys.Cancel;

      resetButton?.tryHover(x, y, 0.2f);
      resetButtonIcon?.tryHover(x, y, 0.2f);
      if (resetButton?.containsPoint(x, y) ?? false) hoverText = TranslatedKeys.ResetToDefault;
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
        textBox.Text = warpObject.DefaultText;
      }
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);
      b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
      SpriteText.drawStringWithScrollCenteredAt(b, TranslatedKeys.Rename, Game1.uiViewport.Width / 2, Game1.uiViewport.Height / 2 - 128, TranslatedKeys.Rename);
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