using MagicScepter.Constants;
using MagicScepter.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class SettingsMenu : IClickableMenu
  {
    private ClickableTextureComponent configPageButon;
    private ClickableTextureComponent settingsPageButon;
    private OptionComponent adjustTeleportMenuOption;
    private OptionComponent resetConfigurationOption;
    // private OptionComponent resetSettingsOption;
    private OptionComponent resetKeybindsOption;
    private CheckboxComponent menuTypeCheckbox;
    private const int configPageButtonID = 100;
    private const int settingsPageButtonID = 101;
    private const int menuTypeCheckboxID = 102;
    private const int resetConfigurationOptionID = 103;
    private const int resetKeybindsOptionID = 104;
    // private const int resetSettingsOptionID = 105;
    private const int adjustTeleportMenuOptionID = 106;
    private readonly Texture2D spritesheetTexture;
    private string hoverText = string.Empty;

    public SettingsMenu()
    {
      width = 832;
      height = 576;

      spritesheetTexture = FileHelper.GetSpritesheetTexture();

      ResetLayout();
      CreateComponents();

      if (Game1.options.SnappyMenus)
      {
        currentlySnappedComponent = settingsPageButon;
        base.snapCursorToCurrentSnappedComponent();
      }
    }

    private void ResetLayout()
    {
      var topLeft = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
      xPositionOnScreen = (int)topLeft.X;
      yPositionOnScreen = (int)topLeft.Y + 32;
    }

    private void CreateComponents()
    {
      configPageButon = new ClickableTextureComponent(
        new(xPositionOnScreen - 64, yPositionOnScreen + 24, 64, 64),
        spritesheetTexture,
        new(66, 64, 32, 32),
        2f
      )
      {
        myID = configPageButtonID,
        downNeighborID = settingsPageButtonID,
        rightNeighborID = menuTypeCheckboxID,
      };
      settingsPageButon = new ClickableTextureComponent(
        new(xPositionOnScreen - 64 + 4, yPositionOnScreen + 24 + 64, 64, 64),
        spritesheetTexture,
        new(34, 64, 32, 32),
        2f
      )
      {
        myID = settingsPageButtonID,
        upNeighborID = configPageButtonID,
        rightNeighborID = menuTypeCheckboxID,
      };

      // top section
      var slot = 0;
      menuTypeCheckbox = new CheckboxComponent(
        new(xPositionOnScreen + 24, yPositionOnScreen + 32 + 64 * slot++, width - 48, 64),
        SetMenuType,
        I18n.SettingsMenu_Option_OldDialog_Label(),
        ModUtility.Config.UseOldDialogMenu
      );
      menuTypeCheckbox.SetupIDs(
        ID: menuTypeCheckboxID,
        upID: upperRightCloseButton_ID,
        downID: adjustTeleportMenuOptionID,
        leftID: configPageButtonID,
        rightID: upperRightCloseButton_ID
      );

      adjustTeleportMenuOption = new OptionComponent(
        new(xPositionOnScreen + 24, yPositionOnScreen + 32 + 64 * slot++, width - 48, 64),
        OpenTeleportMenuSettings,
        I18n.SettingsMenu_Option_AdjustTeleportMenu_Label()
      );
      adjustTeleportMenuOption.SetupIDs(
        ID: adjustTeleportMenuOptionID,
        upID: menuTypeCheckboxID,
        downID: resetConfigurationOptionID,
        leftID: configPageButtonID,
        rightID: -1
      );

      // bottom section
      slot = 2;
      resetConfigurationOption = new OptionComponent(
        new(xPositionOnScreen + 24, yPositionOnScreen + height - 16 - 64 * slot--, width - 48, 64),
        ResetConfiguration,
        I18n.SettingsMenu_Option_ResetConfiguration_Label()
      );
      resetConfigurationOption.SetupIDs(
        ID: resetConfigurationOptionID,
        upID: adjustTeleportMenuOptionID,
        downID: resetKeybindsOptionID,
        leftID: configPageButtonID,
        rightID: -1
      );
      resetKeybindsOption = new OptionComponent(
        new(xPositionOnScreen + 24, yPositionOnScreen + height - 16 - 64 * slot--, width - 48, 64),
        ResetKeybinds,
        I18n.SettingsMenu_Option_ResetKeybinds_Label()
      );
      resetKeybindsOption.SetupIDs(
        ID: resetKeybindsOptionID,
        upID: resetConfigurationOptionID,
        downID: -1,
        leftID: configPageButtonID,
        rightID: -1
      );

      // resetSettingsOption = new OptionComponent(
      //   new Rectangle(xPositionOnScreen + 24, yPositionOnScreen + 32 + 64 * slot++, width - 48, 64),
      //   ResetSettings,
      //   I18n.SettingsMenu_Option_ResetSettings_Label()
      // );
      // resetSettingsOption.SetupIDs(
      //   ID: restoreSettingsOptionID,
      //   upID: restoreConfigurationOptionID,
      //   downID: restoreKeybindsOptionID,
      //   leftID: configPageButtonID,
      //   rightID: -1
      // );

      initializeUpperRightCloseButton();
      upperRightCloseButton.myID = upperRightCloseButton_ID;
      upperRightCloseButton.downNeighborID = menuTypeCheckboxID;
      upperRightCloseButton.leftNeighborID = menuTypeCheckboxID;

      populateClickableComponentList();
    }

    private void SetMenuType(bool value)
    {
      ModDataHelper.SetMenuType(value);
    }

    private void OpenTeleportMenuSettings()
    {
      exitThisMenu();
      Game1.activeClickableMenu = new TeleportMenuSettings();
    }

    private void ResetConfiguration()
    {
      ModDataHelper.RestoreConfiguraiton();
      Game1.playSound("shwip");
      GameHelper.ShowMessage(I18n.SettingsMenu_Option_ResetConfiguration_Message(), Models.MessageType.Warn);
    }

    // private void ResetSettings()
    // {
    //   ModDataHelper.RestoreSettings();
    //   GameHelper.ShowMessage(I18n.SettingsMenu_Option_ResetSettings_Message(), Models.MessageType.Warn);
    // }

    private void ResetKeybinds()
    {
      ModDataHelper.RestoreKeybinds();
      Game1.playSound("shwip");
      GameHelper.ShowMessage(I18n.SettingsMenu_Option_ResetKeybinds_Message(), Models.MessageType.Warn);
    }

    private void ConfigPageButtonPressed()
    {
      exitThisMenu();
      Game1.activeClickableMenu = new ConfigMenu();
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      ResetLayout();
      CreateComponents();
    }

    public override void populateClickableComponentList()
    {
      allClickableComponents ??= new();
      allClickableComponents.Clear();
      allClickableComponents.Add(configPageButon);
      allClickableComponents.Add(settingsPageButon);
      allClickableComponents.Add(menuTypeCheckbox.ClickableComponent);
      allClickableComponents.Add(adjustTeleportMenuOption.ClickableComponent);
      allClickableComponents.Add(resetConfigurationOption.ClickableComponent);
      allClickableComponents.Add(resetKeybindsOption.ClickableComponent);
      // allClickableComponents.Add(resetSettingsOption.ClickableComponent);
      allClickableComponents.Add(upperRightCloseButton);
    }

    public override void snapToDefaultClickableComponent()
    {
      currentlySnappedComponent = menuTypeCheckbox.ClickableComponent;
      base.snapCursorToCurrentSnappedComponent();
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (configPageButon.containsPoint(x, y))
      {
        ConfigPageButtonPressed();
        return;
      }

      menuTypeCheckbox.receiveLeftClick(x, y);
      adjustTeleportMenuOption.receiveLeftClick(x, y);
      resetConfigurationOption.receiveLeftClick(x, y);
      resetKeybindsOption.receiveLeftClick(x, y);
      // resetSettingsOption.receiveLeftClick(x, y);

      base.receiveLeftClick(x, y, playSound);
    }

    public override void performHoverAction(int x, int y)
    {
      menuTypeCheckbox.performHoverAction(x, y);
      adjustTeleportMenuOption.performHoverAction(x, y);
      resetConfigurationOption.performHoverAction(x, y);
      resetKeybindsOption.performHoverAction(x, y);
      // resetSettingsOption.performHoverAction(x, y);
      base.performHoverAction(x, y);
    }

    public override void draw(SpriteBatch b)
    {
      // draw faded background
      GameHelper.DrawFadedBackground(b, 0.2f);
      // draw menu title
      GameHelper.DrawTextInScroll(b, I18n.SettingsMenu_Title(), xPositionOnScreen + width / 2, yPositionOnScreen - 64);
      
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

      hoverText = string.Empty;

      // draw page buttons
      var mouseX = Game1.getMouseX();
      var mouseY = Game1.getMouseY();

      if (configPageButon.containsPoint(mouseX, mouseY)) hoverText = I18n.ConfigurationMenu_Title();
      var color = hoverText == I18n.SettingsMenu_Title() ? Color.White : Color.White * 0.8f;
      configPageButon.draw(b, color, GameHelper.CalculateDepth(settingsPageButon.bounds.Y));

      if (settingsPageButon.containsPoint(mouseX, mouseY)) hoverText = I18n.SettingsMenu_Title();
      settingsPageButon.draw(b);

      // draw options
      menuTypeCheckbox.draw(b);
      adjustTeleportMenuOption.draw(b);
      resetConfigurationOption.draw(b);
      resetKeybindsOption.draw(b);
      // resetSettingsOption.draw(b);

      // draw hover text
      if (hoverText.IsNotEmpty()) drawHoverText(b, hoverText, Game1.smallFont);

      base.draw(b);
      drawMouse(b);
    }
  }
}