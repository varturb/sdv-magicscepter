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
    private string selectedWarpTarget;
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

    public WarpMenu(List<WarpLocationBase> warpLocations)
    {
      menuBackgroundTexture = ModUtility.Helper.ModContent.Load<Texture2D>("assets/spritesheet.png");
      width = 384;
      height = 384;
      xPositionOnScreen = (int)((float)(Game1.viewport.Width / 2) - (float)width / 2f);
      yPositionOnScreen = (int)((float)(Game1.viewport.Height / 2) - (float)height / 2f);
      alpha = 0f;

      this.warpLocations = warpLocations;

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
      RepositionWarpTargetButtons();
    }

    private void RepositionWarpTargetButtons()
    {
      var index = 0;
      foreach (var button in warpLocationButtons)
      {
        var num = Utility.Lerp(0f, MathF.PI * 2f, (float)index / (float)warpLocationButtons.Count);
        button.bounds.X = (int)((float)(xPositionOnScreen + width / 2 + (int)(-Math.Sin(num) * (double)buttonRadius) * 4) - (float)button.bounds.Width / 2f);
        button.bounds.Y = (int)((float)(yPositionOnScreen + height / 2 + (int)(-Math.Cos(num) * (double)buttonRadius) * 4) - (float)button.bounds.Height / 2f);
        index++;
      }
    }

    private void SnapToPlayerPosition()
    {
      if (Game1.player == null)
        return;

      var vector2 = Game1.player.getLocalPosition(Game1.viewport) + new Vector2((float)-width / 2f, (float)-height / 2f);
      xPositionOnScreen = (int)vector2.X + 24;
      yPositionOnScreen = (int)vector2.Y - 32;

      if (xPositionOnScreen + width > Game1.viewport.Width)
        xPositionOnScreen -= xPositionOnScreen + width - Game1.viewport.Width;

      if (xPositionOnScreen < 0)
        xPositionOnScreen -= xPositionOnScreen;

      if (yPositionOnScreen + height > Game1.viewport.Height)
        yPositionOnScreen -= yPositionOnScreen + height - Game1.viewport.Height;

      if (yPositionOnScreen < 0)
        yPositionOnScreen -= yPositionOnScreen;

      RepositionWarpTargetButtons();
    }

    private void ConfirmSelection()
    {
      exitThisMenu(false);

      if (selectedWarpTarget != null)
      {
        ResponseManager.HandleResponse(selectedWarpTarget);
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
      x = (int)Utility.ModifyCoordinateFromUIScale(x);
      y = (int)Utility.ModifyCoordinateFromUIScale(y);

      if (gamepadMode)
        return;

      for (int index = 0; index < warpLocationButtons.Count; ++index)
      {
        if (warpLocationButtons[index].containsPoint(x, y))
        {
          selectedWarpTarget = warpLocations[index].DialogKey;
          selectedWarpTargetIndex = index;
          if (selectedWarpTargetIndex == oldSelectedWarpTargetIndex)
            return;

          selectionTime = 0;
          return;
        }
      }
      selectedWarpTarget = null;
      selectedWarpTargetIndex = -1;
    }

    public override void update(GameTime time)
    {
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
              selectedWarpTarget = warpLocations[index].DialogKey;
              selectedWarpTargetIndex = index;

              Game1.setMousePosition(
                warpLocationButtons[selectedWarpTargetIndex].bounds.Right - warpLocationButtons[selectedWarpTargetIndex].bounds.Width / 8,
                warpLocationButtons[selectedWarpTargetIndex].bounds.Bottom - warpLocationButtons[selectedWarpTargetIndex].bounds.Height / 8
              );
            }
          }
        }
      }
      for (int index = 0; index < warpLocationButtons.Count; ++index)
      {
        if (warpLocationButtons[index].scale > buttonScale)
        {
          warpLocationButtons[index].scale = Utility.MoveTowards(warpLocationButtons[index].scale, buttonScale, (float)(time.ElapsedGameTime.Milliseconds / 1000f * 10f));
        }
      }
      if (selectedWarpTarget != null && selectedWarpTargetIndex > -1)
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

    public override void receiveGamePadButton(Buttons b)
    {
      if (b == Buttons.DPadUp || b == Buttons.DPadRight)
      {
        if (selectedWarpTargetIndex >= 0 && selectedWarpTargetIndex < warpLocationButtons.Count - 1)
        {
          selectedWarpTargetIndex++;
        }
        else
        {
          selectedWarpTargetIndex = 0;
        }
      }
      if (b == Buttons.DPadDown || b == Buttons.DPadLeft)
      {
        if (selectedWarpTargetIndex <= warpLocationButtons.Count && selectedWarpTargetIndex > 0)
        {
          selectedWarpTargetIndex--;
        }
        else if (selectedWarpTargetIndex == 0)
        {
          selectedWarpTargetIndex = warpLocationButtons.Count - 1;
        }
        else
        {
          selectedWarpTargetIndex = 0;
        }
      }

      Game1.setMousePosition(
        warpLocationButtons[selectedWarpTargetIndex].bounds.Right - warpLocationButtons[selectedWarpTargetIndex].bounds.Width / 8,
        warpLocationButtons[selectedWarpTargetIndex].bounds.Bottom - warpLocationButtons[selectedWarpTargetIndex].bounds.Height / 8
      );
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      x = (int)Utility.ModifyCoordinateFromUIScale(x);
      y = (int)Utility.ModifyCoordinateFromUIScale(y);
      ConfirmSelection();
      base.receiveLeftClick(x, y, playSound);
    }

    public override void draw(SpriteBatch b)
    {
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

      if (selectedWarpTarget != null)
      {
        warpLocationButtons[selectedWarpTargetIndex].draw(b, white, 0.86f);

        foreach (var warpLocation in warpLocations)
        {
          if (warpLocation.DialogKey == selectedWarpTarget)
          {
            SpriteText.drawStringWithScrollCenteredAt(b, warpLocation.DialogText, xPositionOnScreen + width / 2, yPositionOnScreen + height + 40);
            break;
          }
        }
      }

      base.draw(b);
      base.drawMouse(b);
    }
  }
}
