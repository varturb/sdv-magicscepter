using MagicScepter.Models;
using MagicScepter.Handlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using MagicScepter.Constants;
using StardewModdingAPI;
using System.Linq;
using MagicScepter.Helpers;

namespace MagicScepter.UI
{
  public class TeleportMenu : IClickableMenu
  {
    private readonly TeleportMenuConfigButton configButton;
    private readonly Texture2D spritesheetTexture;
    private readonly List<TeleportScroll> teleportScrolls;
    private List<ClickableTextureComponent> teleportScrollButtons;
    private float alpha;
    private int selectedTeleportScrollIndex = -1;
    private int oldSelectedTeleportScrollIndex;
    private int age;
    private int buttonRadius;
    private readonly int expandedButtonRadius = 42;
    private readonly int expandTime = 200;
    private readonly float buttonScale = 1.2f;
    private readonly float selectedButtonScale = 1.8f;
    private bool gamepadMode;
    private bool ignoreMouse = false;
    private bool positionHoverTextOnTop = false;

    public TeleportMenu(List<TeleportScroll> teleportScrolls) : base(0, 0, 0, 0, false)
    {
      width = 400;
      height = 400;
      alpha = 0f;

      spritesheetTexture = ModUtility.Helper.ModContent.Load<Texture2D>(ModConstants.SpritesheetTexturePath);
      configButton = new TeleportMenuConfigButton();

      this.teleportScrolls = teleportScrolls;

      SetMenuPositionOnScreen();
      CreateTeleportScrollButtons();
      SnapToPlayerPosition();
    }

    private void CreateTeleportScrollButtons()
    {
      teleportScrollButtons = new List<ClickableTextureComponent>();

      var index = 0;
      foreach (var tp in teleportScrolls)
      {
        teleportScrollButtons.Add(new ClickableTextureComponent(
          new Rectangle(0, 0, 64, 64),
          spritesheetTexture,
          tp.SpirteSource,
          buttonScale
        ));
        index++;
      }
    }

    private void RepositionTeleportScrollButtons()
    {
      var index = 0;
      foreach (var button in teleportScrollButtons)
      {
        var num = Utility.Lerp(0f, MathF.PI * 2f, (float)index / (float)teleportScrollButtons.Count);
        button.bounds.X = (int)((float)(xPositionOnScreen + width / 2f + (int)(-Math.Sin(num) * (double)buttonRadius) * 4) - (float)button.bounds.Width / 2f);
        button.bounds.Y = (int)((float)(yPositionOnScreen + height / 2f + (int)(-Math.Cos(num) * (double)buttonRadius) * 4) - (float)button.bounds.Height / 2f);
        index++;
      }
    }

    private void SnapToPlayerPosition()
    {
      if (Game1.player == null)
        return;

      SetMenuPositionOnScreen();
      RepositionTeleportScrollButtons();
    }

    private void ConfirmSelection()
    {
      exitThisMenu(false);

      if (selectedTeleportScrollIndex > -1)
      {
        var id = teleportScrolls[selectedTeleportScrollIndex].ID;
        ScrollHandler.HandleResponse(id);
      }
    }

    private void HandleUseToolButton(Keys key)
    {
      if (!Game1.options.doesInputListContain(Game1.options.useToolButton, key))
        return;

      ConfirmSelection();
    }

    private void HandleUpButton(Keys key)
    {
      if (!Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
        return;

      var count = teleportScrollButtons.Count;
      var firstIndex = 0;
      var lastIndex = teleportScrollButtons.Count - 1;
      ref var index = ref selectedTeleportScrollIndex;

      // var leftThreshold = (int)Math.Round((float)count / 4 - 0.01);
      var downThreshold = (int)Math.Round((float)count / 4 * 2 - 0.01);
      // var rightThreshold = (int)Math.Round((float)count / 4 * 3);

      if (index == -1)
      {
        index = firstIndex;
      }
      else
      {
        if (index == lastIndex)
        {
          index = firstIndex;
        }
        else if (firstIndex < index && index <= downThreshold)
        {
          index--;
        }
        else if (downThreshold < index && index < lastIndex)
        {
          index++;
        }
      }

      FixMousePosition();
    }

    private void HandleRightButton(Keys key)
    {
      if (!Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
        return;

      var count = teleportScrollButtons.Count;
      var firstIndex = 0;
      var lastIndex = teleportScrollButtons.Count - 1;
      ref var index = ref selectedTeleportScrollIndex;

      var leftThreshold = (int)Math.Round((float)count / 4 - 0.01);
      // var downThreshold = (int)Math.Round((float)count / 4 * 2 - 0.01);
      var rightThreshold = count > 2 ? (int)Math.Round((float)count / 4 * 3) : 1;

      if (index == -1)
      {
        index = rightThreshold;
      }
      else
      {
        if (index == firstIndex)
        {
          index = lastIndex;
        }
        else if ((firstIndex < index && index <= leftThreshold) || (lastIndex >= index && index > rightThreshold))
        {
          index--;
        }
        else if (leftThreshold < index && index < rightThreshold)
        {
          index++;
        }
      }

      FixMousePosition();
    }

    private void HandleDownButton(Keys key)
    {
      if (!Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
        return;

      var count = teleportScrollButtons.Count;
      var firstIndex = 0;
      var lastIndex = teleportScrollButtons.Count - 1;
      ref var index = ref selectedTeleportScrollIndex;

      // var leftThreshold = (int)Math.Round((float)count / 4 - 0.01);
      var downThreshold = (int)Math.Round((float)count / 4 * 2 - 0.01);
      // var rightThreshold = count > 2 ? (int)Math.Round((float)count / 4 * 3) : 1;

      if (index == -1)
      {
        index = downThreshold;
      }
      else
      {
        if (index == firstIndex)
        {
          index = lastIndex;
        }
        else if (lastIndex >= index && index > downThreshold)
        {
          index--;
        }
        else if (firstIndex < index && index < downThreshold)
        {
          index++;
        }
      }

      FixMousePosition();
    }

    private void HandleLeftButton(Keys key)
    {
      if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
        return;

      var count = teleportScrollButtons.Count;
      var firstIndex = 0;
      var lastIndex = teleportScrollButtons.Count - 1;
      ref var index = ref selectedTeleportScrollIndex;

      var leftThreshold = (int)Math.Round((float)count / 4 - 0.01);
      // var downThreshold = (int)Math.Round((float)count / 4 * 2 - 0.01);
      var rightThreshold = count > 2 ? (int)Math.Round((float)count / 4 * 3) : 1;

      if (index == -1)
      {
        index = leftThreshold;
      }
      else
      {
        if (index == lastIndex)
        {
          index = firstIndex;
        }
        else if (leftThreshold < index && index <= rightThreshold)
        {
          index--;
        }
        else if ((firstIndex <= index && index < leftThreshold) || (lastIndex > index && index > rightThreshold))
        {
          index++;
        }
      }

      FixMousePosition();
    }

    private void HandleKeybind(Keys key)
    {
      if (!Context.IsMainPlayer)
        return;

      var sButton = key.ToSButton();
      var tpScroll = teleportScrolls.FirstOrDefault(tp => tp.Keybind == sButton);
      if (tpScroll != null && tpScroll.CanTeleport && !tpScroll.Hidden)
      {
        selectedTeleportScrollIndex = teleportScrolls.IndexOf(tpScroll);
        exitThisMenu(false);
        tpScroll.Teleport();
      }
    }

    private void HandleScrollWheel(int direction)
    {
      var lastItemIndex = teleportScrollButtons.Count - 1;
      ref var index = ref selectedTeleportScrollIndex;

      if (index == -1)
      {
        index = 0;
      }
      else if (direction > 0)
      {
        if (-1 < index && index < lastItemIndex)
        {
          index++;
        }
        else
        {
          index = 0;
        }
      }
      else if (direction < 0)
      {
        if (0 < index && index <= lastItemIndex)
        {
          index--;
        }
        else
        {
          index = lastItemIndex;
        }
      }

      FixMousePosition();
    }

    private void FixMousePosition()
    {
      return;
      if (selectedTeleportScrollIndex > -1)
      {
        var button = teleportScrollButtons[selectedTeleportScrollIndex].bounds;
        var x = button.Right - button.Width / 8;
        var y = button.Bottom - button.Height / 8;

        ignoreMouse = ShouldIgnoreMouseUI();
        if (!ignoreMouse)
          Game1.setMousePosition(x, y);
      }
    }

    private void SetMenuPositionOnScreen()
    {
      var menuPositiononScreen = GetMenuPositionOnScreen();
      xPositionOnScreen = (int)menuPositiononScreen.X;
      yPositionOnScreen = (int)menuPositiononScreen.Y;

      if (xPositionOnScreen + width > Game1.viewport.Width)
      { // right off screen 
        xPositionOnScreen -= xPositionOnScreen + width - Game1.viewport.Width + 5;
      }

      if (xPositionOnScreen < 0)
      { // left off screen
        xPositionOnScreen -= xPositionOnScreen;
      }

      if (yPositionOnScreen + height > Game1.viewport.Height)
      { // bottom off screen
        yPositionOnScreen -= yPositionOnScreen + height - Game1.viewport.Height + 5;
      }

      if (yPositionOnScreen < 0)
      { // top off screen
        yPositionOnScreen -= yPositionOnScreen;
      }

      positionHoverTextOnTop = yPositionOnScreen >= Game1.viewport.Height - height - 5 - 64;
    }

    private Vector2 GetMenuPositionOnScreen()
    {
      var playerStandingPosition = Game1.player.getStandingPosition();
      var offset = new Vector2(-width / 2f, -height / 2f);
      var playerYOffset = Utility.ModifyCoordinateForUIScale(-48);
      var playerCenterPoint = new Vector2(playerStandingPosition.X - (float)Game1.viewport.X, playerStandingPosition.Y - (float)Game1.viewport.Y);
      var playerCenterPositionOnScreen = Utility.ModifyCoordinatesForUIScale(playerCenterPoint);
      playerCenterPositionOnScreen.Y += playerYOffset;

      return playerCenterPositionOnScreen + offset;
    }

    private static bool ShouldIgnoreMouseUI()
    {
      var uiScale = Game1.options.uiScale;
      var zoomLevel = Game1.options.zoomLevel;
      var diff = zoomLevel - uiScale;

      if (zoomLevel < 1.2 && diff > 0.2)
        return true;

      if (zoomLevel >= 1.2 && diff > 0.4)
        return true;

      return false;
    }

    public override void applyMovementKey(int direction)
    {
    }

    protected override void cleanupBeforeExit()
    {
      Game1.exitActiveMenu();
      base.cleanupBeforeExit();
    }

    public override void performHoverAction(int x, int y)
    {
      if (gamepadMode || ignoreMouse || alpha != 1f)
        return;

      for (int index = 0; index < teleportScrollButtons.Count; ++index)
      {
        if (teleportScrollButtons[index].containsPoint(x, y))
        {
          selectedTeleportScrollIndex = index;
          if (selectedTeleportScrollIndex == oldSelectedTeleportScrollIndex)
            return;
        }
      }
    }

    public override void receiveKeyPress(Keys key)
    {
      HandleKeybind(key);

      HandleUseToolButton(key);
      SetMenuPositionOnScreen();
      HandleUpButton(key);
      HandleRightButton(key);
      HandleDownButton(key);
      HandleLeftButton(key);

      base.receiveKeyPress(key);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      x = (int)Utility.ModifyCoordinateFromUIScale(x);
      y = (int)Utility.ModifyCoordinateFromUIScale(y);

      if (Context.IsMainPlayer)
        configButton.receiveLeftClick(x, y, playSound);

      base.receiveLeftClick(x, y, playSound);

      if (selectedTeleportScrollIndex > -1)
      {
        var wo = teleportScrollButtons[selectedTeleportScrollIndex];
        if (wo.containsPoint(x, y))
        {
          ConfirmSelection();
        }
      }
    }

    public override void receiveScrollWheelAction(int direction)
    {
      HandleScrollWheel(direction);

      base.receiveScrollWheelAction(direction);
    }

    public override void update(GameTime time)
    {
      // var mousePosition = Game1.input.GetMouseState();
      // var oldMouseState = Game1.oldMouseState;
      // if (mousePosition != oldMouseState)
      // {
      //   ignoreMouse = false;
      // }

      age += time.ElapsedGameTime.Milliseconds;

      if (age > expandTime)
      {
        age = expandTime;
      }

      var rightThumbStickUsed = (double)Math.Abs(Game1.input.GetGamePadState().ThumbSticks.Right.X) > 0.5 || (double)Math.Abs(Game1.input.GetGamePadState().ThumbSticks.Right.Y) > 0.5;
      var leftThumbStickUsed = (double)Math.Abs(Game1.input.GetGamePadState().ThumbSticks.Left.X) > 0.5 || (double)Math.Abs(Game1.input.GetGamePadState().ThumbSticks.Left.Y) > 0.5;

      if (!gamepadMode && Game1.options.gamepadControls && (rightThumbStickUsed || leftThumbStickUsed))
      {
        gamepadMode = true;
      }

      alpha = (float)age / (float)expandTime;
      buttonRadius = (int)((float)age / (float)expandTime * (float)expandedButtonRadius);
      SnapToPlayerPosition();

      var v1 = new Vector2();
      if (gamepadMode)
      {
        if (rightThumbStickUsed || leftThumbStickUsed)
        {
          v1 = rightThumbStickUsed
            ? new Vector2(Game1.input.GetGamePadState().ThumbSticks.Right.X, Game1.input.GetGamePadState().ThumbSticks.Right.Y)
            : new Vector2(Game1.input.GetGamePadState().ThumbSticks.Left.X, Game1.input.GetGamePadState().ThumbSticks.Left.Y);
          v1.Y *= -1f;
          v1.Normalize();
          float num1 = -1f;
          for (int index = 0; index < teleportScrollButtons.Count; ++index)
          {
            var v2 = new Vector2(
              teleportScrollButtons[index].bounds.Center.X - (xPositionOnScreen + width / 2f),
              teleportScrollButtons[index].bounds.Center.Y - (yPositionOnScreen + height / 2f)
            );
            float num2 = Vector2.Dot(v1, v2);
            if ((double)num2 > (double)num1)
            {
              num1 = num2;
              selectedTeleportScrollIndex = index;

              FixMousePosition();
            }
          }
        }
      }

      for (int index = 0; index < teleportScrollButtons.Count; index++)
      {
        if (teleportScrollButtons[index].scale > buttonScale)
        {
          teleportScrollButtons[index].scale = Utility.MoveTowards(teleportScrollButtons[index].scale, buttonScale, (float)(time.ElapsedGameTime.Milliseconds / 1000f * 10f));
        }
      }
      if (selectedTeleportScrollIndex > -1)
      {
        teleportScrollButtons[selectedTeleportScrollIndex].scale = selectedButtonScale;
      }

      if (oldSelectedTeleportScrollIndex != selectedTeleportScrollIndex)
      {
        oldSelectedTeleportScrollIndex = selectedTeleportScrollIndex;
      }

      // selectionTime += time.ElapsedGameTime.Milliseconds;
      base.update(time);
    }

    public override void draw(SpriteBatch b)
    {
      SetMenuPositionOnScreen();
      RepositionTeleportScrollButtons();

      GameHelper.DrawFadedBackground(b, 0.2f);

      var white = Color.White;
      white.A = (byte)Utility.Lerp(0f, 255f, alpha);
      var index = 0;
      foreach (var button in teleportScrollButtons)
      {
        if (index++ != selectedTeleportScrollIndex)
        {
          button.draw(b, white, 0.86f);
        }
      }

      if (selectedTeleportScrollIndex > -1)
      {
        teleportScrollButtons[selectedTeleportScrollIndex].draw(b, white, 0.86f);

        var text = teleportScrolls[selectedTeleportScrollIndex].Text;
        var y = positionHoverTextOnTop ? yPositionOnScreen - 100 : yPositionOnScreen + height + 40;
        var x = xPositionOnScreen + width / 2;

        if (Context.IsMainPlayer)
        {
          var textWidth = SpriteText.getWidthOfString(text);
          configButton.xPositionOnScreen = x + textWidth / 2 - 4;
          configButton.yPositionOnScreen = y + 8;
          var textOffset = "  ";
          SpriteText.drawStringWithScrollCenteredAt(b, text + textOffset, x, y);
          configButton.draw(b);
        }
        else
        {
          SpriteText.drawStringWithScrollCenteredAt(b, text, x, y);
        }
      }

      // if (!ignoreMouse)
      // if (!gamepadMode)
      drawMouse(b);

      // var debug = new ScreenDebug(this);
      // debug.DrawDebug();
    }
  }
}
