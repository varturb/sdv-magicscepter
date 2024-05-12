using MagicScepter.WarpLocations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace MagicScepter.UI
{
  public class WarpMenu : IClickableMenu
  {
    private readonly Texture2D menuBackgroundTexture;
    private readonly List<WarpLocationBase> warpLocations;
    private List<ClickableTextureComponent> warpLocationButtons;
    private float alpha;
    private int selectedWarpTargetIndex = -1;
    private int selectionTime;
    private int oldSelectedWarpTargetIndex;
    private int age;
    private int buttonRadius;
    private readonly int expandedButtonRadius = 42;
    private readonly int expandTime = 200;
    private readonly float buttonScale = 1.2f;
    private readonly float selectedButtonScale = 1.8f;
    private bool gamepadMode;

    private bool ignoreMouse = false;

    public WarpMenu(List<WarpLocationBase> warpLocations)
    {
      menuBackgroundTexture = ModUtility.Helper.ModContent.Load<Texture2D>("assets/spritesheet.png");
      width = 400;
      height = 400;

      alpha = 0f;

      this.warpLocations = warpLocations;

      SetMenuPositionOnScreen();
      CreateWarpTargetButtons();
      SnapToPlayerPosition();
    }

    private void CreateWarpTargetButtons()
    {
      warpLocationButtons = new List<ClickableTextureComponent>();

      var index = 0;
      foreach (var warpLocation in warpLocations)
      {
        warpLocationButtons.Add(new ClickableTextureComponent(
          new Rectangle(0, 0, 64, 64),
          menuBackgroundTexture,
          warpLocation.SpirteSource,
          buttonScale
        ));
        index++;
      }
    }

    private void RepositionWarpTargetButtons()
    {
      var index = 0;
      foreach (var button in warpLocationButtons)
      {
        var num = Utility.Lerp(0f, MathF.PI * 2f, (float)index / (float)warpLocationButtons.Count);
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
      RepositionWarpTargetButtons();
    }

    private void ConfirmSelection()
    {
      exitThisMenu(false);

      if (selectedWarpTargetIndex > -1)
      {
        var warpTargetKey = warpLocations[selectedWarpTargetIndex].DialogKey;
        ResponseManager.HandleResponse(warpTargetKey);
      }
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
      if (gamepadMode || ignoreMouse)
        return;

      for (int index = 0; index < warpLocationButtons.Count; ++index)
      {
        if (warpLocationButtons[index].containsPoint(x, y))
        {
          selectedWarpTargetIndex = index;
          if (selectedWarpTargetIndex == oldSelectedWarpTargetIndex)
            return;

          selectionTime = 0;
          return;
        }
      }
      selectedWarpTargetIndex = -1;
    }

    public override void receiveKeyPress(Keys key)
    {
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
      ConfirmSelection();
      base.receiveLeftClick(x, y, playSound);
    }

    public override void receiveScrollWheelAction(int direction)
    {
      HandleScrollWheel(direction);

      base.receiveScrollWheelAction(direction);
    }


    public override void update(GameTime time)
    {
      var mousePosition = Game1.input.GetMouseState();
      var oldMouseState = Game1.oldMouseState;
      if (mousePosition != oldMouseState)
      {
        ignoreMouse = false;
      }

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
          for (int index = 0; index < warpLocationButtons.Count; ++index)
          {
            var v2 = new Vector2(
              warpLocationButtons[index].bounds.Center.X - (xPositionOnScreen + width / 2f),
              warpLocationButtons[index].bounds.Center.Y - (yPositionOnScreen + height / 2f)
            );
            float num2 = Vector2.Dot(v1, v2);
            if ((double)num2 > (double)num1)
            {
              num1 = num2;
              selectedWarpTargetIndex = index;

              FixMousePosition();
            }
          }
        }
      }

      for (int index = 0; index < warpLocationButtons.Count; index++)
      {
        if (warpLocationButtons[index].scale > buttonScale)
        {
          warpLocationButtons[index].scale = Utility.MoveTowards(warpLocationButtons[index].scale, buttonScale, (float)(time.ElapsedGameTime.Milliseconds / 1000f * 10f));
        }
      }
      if (selectedWarpTargetIndex > -1)
      {
        warpLocationButtons[selectedWarpTargetIndex].scale = selectedButtonScale;
      }

      if (oldSelectedWarpTargetIndex != selectedWarpTargetIndex)
      {
        oldSelectedWarpTargetIndex = selectedWarpTargetIndex;
        selectionTime = 0;
      }

      selectionTime += time.ElapsedGameTime.Milliseconds;
      base.update(time);
    }

    public override void draw(SpriteBatch b)
    {
      SetMenuPositionOnScreen();
      RepositionWarpTargetButtons();

      var white = Color.White;
      white.A = (byte)Utility.Lerp(0f, 255f, alpha);
      var index = 0;
      foreach (var button in warpLocationButtons)
      {
        if (index++ != selectedWarpTargetIndex)
        {
          button.draw(b, white, 0.86f);
        }
      }

      if (selectedWarpTargetIndex > -1)
      {
        warpLocationButtons[selectedWarpTargetIndex].draw(b, white, 0.86f);
        SpriteText.drawStringWithScrollCenteredAt(b, warpLocations[selectedWarpTargetIndex].DialogText, xPositionOnScreen + width / 2, yPositionOnScreen + height + 40);
      }

      if (!ignoreMouse)
        base.drawMouse(b);
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

      var count = warpLocationButtons.Count;
      var firstIndex = 0;
      var lastIndex = warpLocationButtons.Count - 1;
      ref var index = ref selectedWarpTargetIndex;

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

      var count = warpLocationButtons.Count;
      var firstIndex = 0;
      var lastIndex = warpLocationButtons.Count - 1;
      ref var index = ref selectedWarpTargetIndex;

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

      var count = warpLocationButtons.Count;
      var firstIndex = 0;
      var lastIndex = warpLocationButtons.Count - 1;
      ref var index = ref selectedWarpTargetIndex;

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

      var count = warpLocationButtons.Count;
      var firstIndex = 0;
      var lastIndex = warpLocationButtons.Count - 1;
      ref var index = ref selectedWarpTargetIndex;

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

    private void HandleScrollWheel(int direction)
    {
      var lastItemIndex = warpLocationButtons.Count - 1;
      ref var index = ref selectedWarpTargetIndex;

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
      if (selectedWarpTargetIndex > -1)
      {
        var button = warpLocationButtons[selectedWarpTargetIndex].bounds;
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
        xPositionOnScreen -= xPositionOnScreen + width - Game1.viewport.Width + 5;

      if (xPositionOnScreen < 0)
        xPositionOnScreen -= xPositionOnScreen;

      if (yPositionOnScreen + height > Game1.viewport.Height)
        yPositionOnScreen -= yPositionOnScreen + height - Game1.viewport.Height + 5;

      if (yPositionOnScreen < 0)
        yPositionOnScreen -= yPositionOnScreen;
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
  }
}
