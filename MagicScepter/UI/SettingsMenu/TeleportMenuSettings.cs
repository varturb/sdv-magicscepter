using MagicScepter.Constants;
using MagicScepter.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class TeleportMenuSettings : IClickableMenu
  {
    private SliderComponent radiusSlider;
    private SliderComponent scaleSlider;
    private SliderComponent selectedScaleSlider;
    private OptionComponent resetOption;
    private TeleportMenu teleportMenu;
    private readonly Texture2D spritesheetTexture;
    private const int radiusSliderID = 200;
    private const int scaleSliderID = 201;
    private const int selectedScaleSliderID = 202;
    private const int resetButtonID = 203;
    private static ModConfig ModConfig => ModUtility.Config;

    public TeleportMenuSettings()
    {
      width = 512;
      height = 352;

      spritesheetTexture = ModUtility.Helper.ModContent.Load<Texture2D>(ModConstants.SpritesheetTexturePath);
      exitFunction = delegate
      {
        Game1.activeClickableMenu = new SettingsMenu();
      };

      ResetLayout();
      CreateComponents();

      snapToDefaultClickableComponent();
    }

    private void ResetLayout()
    {
      var topLeft = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
      xPositionOnScreen = 32;
      yPositionOnScreen = (int)topLeft.Y - 64;
    }

    private void CreateComponents()
    {
      var slot = 0;
      radiusSlider = new SliderComponent(
        new(xPositionOnScreen + 24, yPositionOnScreen + 32 + 64 * slot++, width - 48, 64),
        min: ModConstants.ScrollsRadiusRange.Min,
        max: ModConstants.ScrollsRadiusRange.Max,
        value: ModConfig.Radius,
        SetScrollsRadius,
        I18n.TeleportMenuSettings_Radius(),
        interval: ModConstants.ScrollsRadiusInterval
      );

      scaleSlider = new SliderComponent(
        new(xPositionOnScreen + 24, yPositionOnScreen + 32 + 64 * slot++, width - 48, 64),
        min: ModConstants.ScrollsScaleRange.Min.ToInt(100),
        max: ModConstants.ScrollsScaleRange.Max.ToInt(100),
        value: ModConfig.Scale.ToInt(100),
        SetScrollsScale,
        I18n.TeleportMenuSettings_Scale(),
        interval: ModConstants.ScrollsScaleInterval.ToInt(100),
        isFloat: true
      );

      selectedScaleSlider = new SliderComponent(
        new(xPositionOnScreen + 24, yPositionOnScreen + 32 + 64 * slot++, width - 48, 64),
        min: ModConstants.SelectedScrollScaleRange.Min.ToInt(100),
        max: ModConstants.SelectedScrollScaleRange.Max.ToInt(100),
        value: ModConfig.SelectedScale.ToInt(100),
        SetSelectedScrollScale,
        I18n.TeleportMenuSettings_SelectedScale(),
        interval: ModConstants.SelectedScrollScaleInterval.ToInt(100),
        isFloat: true
      );

      slot++;
      resetOption = new OptionComponent(
        new(xPositionOnScreen + 24, yPositionOnScreen + 32 + 64 * slot++, width - 48, 64),
        ResetScrollsSettings,
        I18n.TeleportMenuSettings_Reset_Label(),
        string.Empty,
        isSmallFont: true
      );

      teleportMenu = new TeleportMenu(previewMode: true);

      radiusSlider.SetupIDs(
        ID: radiusSliderID,
        upID: upperRightCloseButton_ID,
        downID: scaleSliderID,
        leftID: -1,
        rightID: -1
      );
      scaleSlider.SetupIDs(
        ID: scaleSliderID,
        upID: radiusSliderID,
        downID: selectedScaleSliderID,
        leftID: -1,
        rightID: -1
      );
      selectedScaleSlider.SetupIDs(
        ID: selectedScaleSliderID,
        upID: scaleSliderID,
        downID: resetButtonID,
        leftID: -1,
        rightID: -1
      );
      resetOption.SetupIDs(
        ID: resetButtonID,
        upID: selectedScaleSliderID,
        downID: -1,
        leftID: -1,
        rightID: -1
      );

      initializeUpperRightCloseButton();
      upperRightCloseButton.myID = upperRightCloseButton_ID;
      upperRightCloseButton.downNeighborID = radiusSliderID;
      upperRightCloseButton.leftNeighborID = radiusSliderID;

      populateClickableComponentList();
    }

    private static void SetScrollsRadius(int radius)
    {
      ModDataHelper.SetRadius(radius);
    }

    private static void SetScrollsScale(int scale)
    {
      ModDataHelper.SetScale(scale.ToFloat(100));
    }

    private static void SetSelectedScrollScale(int scale)
    {
      ModDataHelper.SetSelectedScale(scale.ToFloat(100));
    }

    private void ResetScrollsSettings()
    {
      ModDataHelper.ResetScrollsSettings();
      Game1.playSound("shwip");
      GameHelper.ShowMessage(I18n.TeleportMenuSettings_Reset_Message(), Models.MessageType.Warn);
      CreateComponents();

      if (Game1.options.SnappyMenus)
      {
        currentlySnappedComponent = resetOption.ClickableComponent;
        snapCursorToCurrentSnappedComponent();
      }
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      ResetLayout();
      CreateComponents();
    }

    public override void snapToDefaultClickableComponent()
    {
      if (Game1.options.SnappyMenus)
      {
        currentlySnappedComponent = radiusSlider.ClickableComponent;
        base.snapCursorToCurrentSnappedComponent();
      }
    }

    public override void populateClickableComponentList()
    {
      allClickableComponents ??= new();
      allClickableComponents.Clear();
      allClickableComponents.Add(radiusSlider.ClickableComponent);
      allClickableComponents.Add(scaleSlider.ClickableComponent);
      allClickableComponents.Add(selectedScaleSlider.ClickableComponent);
      allClickableComponents.Add(resetOption.ClickableComponent);
      allClickableComponents.Add(upperRightCloseButton);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      radiusSlider.receiveLeftClick(x, y);
      scaleSlider.receiveLeftClick(x, y);
      selectedScaleSlider.receiveLeftClick(x, y);
      resetOption.receiveLeftClick(x, y);

      base.receiveLeftClick(x, y, playSound);
    }

    public override void releaseLeftClick(int x, int y)
    {
      radiusSlider.releaseLeftClick(x, y);
      scaleSlider.releaseLeftClick(x, y);
      selectedScaleSlider.releaseLeftClick(x, y);
      base.releaseLeftClick(x, y);
    }

    public override void leftClickHeld(int x, int y)
    {
      radiusSlider.leftClickHeld(x, y);
      scaleSlider.leftClickHeld(x, y);
      selectedScaleSlider.leftClickHeld(x, y);
    }

    public override void receiveKeyPress(Keys key)
    {
      base.receiveKeyPress(key);
      radiusSlider.receiveKeyPress(key);
      scaleSlider.receiveKeyPress(key);
      selectedScaleSlider.receiveKeyPress(key);
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);
      resetOption.performHoverAction(x, y);
    }

    public override void update(GameTime time)
    {
      teleportMenu.update(time);
    }

    public override void draw(SpriteBatch b)
    {
      // draw faded background
      GameHelper.DrawFadedBackground(b, 0.5f);

      // draw teleport menu
      teleportMenu.draw(b);

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

      // hoverText = string.Empty;

      // draw options
      radiusSlider.draw(b);
      scaleSlider.draw(b);
      selectedScaleSlider.draw(b);
      resetOption.draw(b);

      // draw hover text
      // if (hoverText.IsNotEmpty()) drawHoverText(b, hoverText, Game1.smallFont);
      base.draw(b);
      drawMouse(b);
    }
  }
}