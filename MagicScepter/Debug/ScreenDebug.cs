using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace MagicScepter.Debug
{
  public class ScreenDebug
  {
    private readonly int width;
    private readonly int height;
    private readonly int xPositionOnScreen;
    private readonly int yPositionOnScreen;
    public const int ButtonBorderWidth = 4 * Game1.pixelZoom;

    public ScreenDebug(IClickableMenu menu)
    {
      width = menu.width;
      height = menu.height;
      xPositionOnScreen = menu.xPositionOnScreen;
      yPositionOnScreen = menu.yPositionOnScreen;
    }

    public void DrawDebug()
    {
      var b = Game1.spriteBatch;
      var x = 5;
      var y = 60;
      var h = 60;
      var i = 0;

      var pl = Game1.player.getLocalPosition(Game1.viewport);

      DrawLineBetween(b, new Vector2(xPositionOnScreen, yPositionOnScreen), new Vector2(xPositionOnScreen + width, yPositionOnScreen), Color.Red);
      DrawLineBetween(b, new Vector2(xPositionOnScreen + width, yPositionOnScreen), new Vector2(xPositionOnScreen + width, yPositionOnScreen + height), Color.Red);
      DrawLineBetween(b, new Vector2(xPositionOnScreen, yPositionOnScreen), new Vector2(xPositionOnScreen, yPositionOnScreen + height), Color.Red);
      DrawLineBetween(b, new Vector2(xPositionOnScreen, yPositionOnScreen + height), new Vector2(xPositionOnScreen + width, yPositionOnScreen + height), Color.Red);
      DrawLineBetween(b, new Vector2(xPositionOnScreen, yPositionOnScreen), new Vector2(xPositionOnScreen + width, yPositionOnScreen + height), Color.Red);
      DrawLineBetween(b, new Vector2(xPositionOnScreen + width, yPositionOnScreen), new Vector2(xPositionOnScreen, yPositionOnScreen + height), Color.Red);

      DrawTab($"Viewport: {Game1.viewport.Width}/{Game1.viewport.Height}", x, y + h * i++);
      DrawTab($"Viewport position: X:{Game1.viewport.X} Y:{Game1.viewport.Y}", x, y + h * i++);
      DrawTab($"Zoom level: {Game1.options.zoomLevel}", x, y + h * i++);
      DrawTab($"UI scale: {Game1.options.uiScale}", x, y + h * i++);
      DrawTab($"Zoom - UI: {Game1.options.zoomLevel - Game1.options.uiScale}", x, y + h * i++);

      DrawTab($"Player location: {Utility.ModifyCoordinatesForUIScale(pl)}", x, y + h * i++);
      // DrawDot(b, Utility.ModifyCoordinatesForUIScale(pl), Color.Pink);

      var menuPosition = GetMenuPositionOnScreen();
      DrawTab($"Menu position: X:{menuPosition.X} Y:{menuPosition.Y}", x, y + h * i++);
      DrawTab($"Menu position raw: X:{xPositionOnScreen} Y:{yPositionOnScreen}", x, y + h * i++);
      // DrawDot(b, new Vector2(menuPosition.X, menuPosition.Y), Color.Yellow);
      // DrawDot(b, new Vector2(xPositionOnScreen, yPositionOnScreen), Color.Orange);

      DrawTab($"Mouse: X:{Game1.getMousePosition().X} Y:{Game1.getMousePosition().Y}", x, y + h * i++);
      // DrawDot(b, new Vector2(Game1.getMousePosition().X, Game1.getMousePosition().Y), Color.LightBlue);
      DrawTab($"X:{Game1.getMouseX()} Y:{Game1.getMouseY()}", Game1.getMousePosition().X + 32, Game1.getMousePosition().Y - 32);

      var playerStandingPosition = Game1.player.getStandingPosition();
      var playerYOffset = Utility.ModifyCoordinateForUIScale(-48);
      var playerCenterPoint = new Vector2(playerStandingPosition.X - (float)Game1.viewport.X, playerStandingPosition.Y - (float)Game1.viewport.Y);
      var playerCenterPositionOnScreen = Utility.ModifyCoordinatesForUIScale(playerCenterPoint);
      playerCenterPositionOnScreen.Y += playerYOffset;
      DrawTab($"Player standing position: {Utility.ModifyCoordinatesForUIScale(playerCenterPoint)}", x, y + h * i++);
      // DrawDot(b, Utility.ModifyCoordinatesForUIScale(playerCenterPoint), Color.LightCyan);

      DrawTab($"Player center position: {playerCenterPositionOnScreen}", x, y + h * i++);
      // DrawDot(b, playerCenterPositionOnScreen, Color.Red);

    }

    public static void DrawTab(string text, int x, int y, int align = 0, float alpha = 1, bool drawShadow = true)
    {
      var font = Game1.smallFont;
      SpriteBatch spriteBatch = Game1.spriteBatch;
      Vector2 bounds = font.MeasureString(text);

      DrawTab(x, y, (int)bounds.X, (int)bounds.Y, out Vector2 drawPos, align, alpha, drawShadow: drawShadow);
      Utility.drawTextWithShadow(spriteBatch, text, font, drawPos, Game1.textColor);
    }

    public static void DrawTab(int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition, int align = 0, float alpha = 1, bool drawShadow = true)
    {
      SpriteBatch spriteBatch = Game1.spriteBatch;

      // calculate outer coordinates
      int outerWidth = innerWidth + ButtonBorderWidth * 2;
      int outerHeight = innerHeight + Game1.tileSize / 3;
      int offsetX = align switch
      {
        1 => -outerWidth / 2,
        2 => -outerWidth,
        _ => 0
      };

      // draw texture
      IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x + offsetX, y, outerWidth, outerHeight + Game1.tileSize / 16, Color.White * alpha, drawShadow: drawShadow);
      innerDrawPosition = new Vector2(x + ButtonBorderWidth + offsetX, y + ButtonBorderWidth);
    }

    private static void DrawDot(SpriteBatch spriteBatch, Vector2 pos, Color color = default(Color))
    {
      var circle = CreateCircle(spriteBatch, 4);
      spriteBatch.Draw(circle, new Vector2(pos.X - 2, pos.Y - 2), color);
    }

    private static void DrawLineBetween(
      SpriteBatch spriteBatch,
      Vector2 startPos,
      Vector2 endPos,
      Color color = default(Color),
      int thickness = 2)
    {
      // Create a texture as wide as the distance between two points and as high as
      // the desired thickness of the line.
      var distance = (int)Vector2.Distance(startPos, endPos);
      var texture = new Texture2D(spriteBatch.GraphicsDevice, distance, thickness);

      // Fill texture with given color.
      var data = new Color[distance * thickness];
      for (int i = 0; i < data.Length; i++)
      {
        data[i] = color;
      }
      texture.SetData(data);

      // Rotate about the beginning middle of the line.
      var rotation = (float)Math.Atan2(endPos.Y - startPos.Y, endPos.X - startPos.X);
      var origin = new Vector2(0, thickness / 2);

      spriteBatch.Draw(
          texture,
          startPos,
          null,
          Color.White,
          rotation,
          origin,
          1.0f,
          SpriteEffects.None,
          1.0f);
    }

    public static Texture2D CreateCircle(SpriteBatch spriteBatch, int radius)
    {
      int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
      Texture2D texture = new Texture2D(spriteBatch.GraphicsDevice, outerRadius, outerRadius);

      Color[] data = new Color[outerRadius * outerRadius];

      // Colour the entire texture transparent first.
      for (int i = 0; i < data.Length; i++)
        data[i] = Color.Transparent;

      // Work out the minimum step necessary using trigonometry + sine approximation.
      double angleStep = 1f / radius;

      for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
      {
        // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
        int x = (int)Math.Round(radius + radius * Math.Cos(angle));
        int y = (int)Math.Round(radius + radius * Math.Sin(angle));

        data[y * outerRadius + x + 1] = Color.White;
      }

      //width
      for (int i = 0; i < outerRadius; i++)
      {
        int yStart = -1;
        int yEnd = -1;


        //loop through height to find start and end to fill
        for (int j = 0; j < outerRadius; j++)
        {

          if (yStart == -1)
          {
            if (j == outerRadius - 1)
            {
              //last row so there is no row below to compare to
              break;
            }

            //start is indicated by Color followed by Transparent
            if (data[i + (j * outerRadius)] == Color.White && data[i + ((j + 1) * outerRadius)] == Color.Transparent)
            {
              yStart = j + 1;
              continue;
            }
          }
          else if (data[i + (j * outerRadius)] == Color.White)
          {
            yEnd = j;
            break;
          }
        }

        //if we found a valid start and end position
        if (yStart != -1 && yEnd != -1)
        {
          //height
          for (int j = yStart; j < yEnd; j++)
          {
            data[i + (j * outerRadius)] = new Color(10, 10, 10, 10);
          }
        }
      }

      texture.SetData(data);
      return texture;
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


    /*public override void receiveGamePadButton(Buttons b)
    {
      if (b == Buttons.DPadUp || b == Buttons.DPadRight)
      {
        if (selectedWarpTargetIndex >= 0 && selectedWarpTargetIndex < (warpLocationButtons.Count - 1))
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
    }*/
  }
}