using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace MagicScepter.Debug
{
  public static class ScreenDebug
  {
    public const int ButtonBorderWidth = 4 * Game1.pixelZoom;

    public static void DrawBox(int x, int y, int width, int height, int order = 0, Color? color = null)
    {
      DrawBox(new Rectangle(x, y, width, height), order, color);
    }

    public static void DrawBox(Rectangle bounds, int order = 0, Color? color = null)
    {
      var b = Game1.spriteBatch;
      DrawLineBetween(b, new Vector2(bounds.X, bounds.Y), new Vector2(bounds.X + bounds.Width, bounds.Y), color ??= Color.Red);
      DrawLineBetween(b, new Vector2(bounds.X + bounds.Width, bounds.Y), new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height), color ??= Color.Red);
      DrawLineBetween(b, new Vector2(bounds.X, bounds.Y), new Vector2(bounds.X, bounds.Y + bounds.Height), color ??= Color.Red);
      DrawLineBetween(b, new Vector2(bounds.X, bounds.Y + bounds.Height), new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height), color ??= Color.Red);
      DrawLineBetween(b, new Vector2(bounds.X, bounds.Y), new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height), color ??= Color.Red);
      DrawLineBetween(b, new Vector2(bounds.X + bounds.Width, bounds.Y), new Vector2(bounds.X, bounds.Y + bounds.Height), color ??= Color.Red);

      if (order == -1)
        return;

      var i = order;
      var x = 5;
      var y = 60;
      DrawTab($"X: {bounds.X} Y: {bounds.Y} W: {bounds.Width} H: {bounds.Height}", x, y + i++ * 60);
    }

    public static void DrawMouse()
    {
      var width = Game1.getMousePosition().X + 32;
      var height = Game1.getMousePosition().Y + 32;
      var x = Game1.getMouseX();
      var y = Game1.getMouseY();
      var vpWidth = (int)Utility.ModifyCoordinateForUIScale(Game1.viewport.Width);
      var vpHeight = (int)Utility.ModifyCoordinateForUIScale(Game1.viewport.Height);
      DrawTab($"X:{x} Y:{y}", width, height, drawShadow: false);
      DrawLineBetween(Game1.spriteBatch, new Vector2(0, y), new Vector2(vpWidth, y), Color.Yellow, 1);
      DrawLineBetween(Game1.spriteBatch, new Vector2(x, 0), new Vector2(x, vpHeight), Color.Yellow, 1);
      x = (int)Utility.ModifyCoordinateFromUIScale(x);
      y = (int)Utility.ModifyCoordinateFromUIScale(y);
      DrawTab($"X:{x} Y:{y} (from ui)", width, height + 48, drawShadow: false);
      x = (int)Utility.ModifyCoordinateForUIScale(Game1.getMouseX());
      y = (int)Utility.ModifyCoordinateForUIScale(Game1.getMouseY());
      DrawTab($"X:{x} Y:{y} (for ui)", width, height + 96, drawShadow: false);
    }

    public static void DrawClickables(List<ClickableComponent> list)
    {
      foreach (var item in list ?? new())
      {
        DrawBox(item.bounds, -1);
      }
    }

    public static void DrawInfo(IClickableMenu menu)
    {
      var vpWidth = (int)Utility.ModifyCoordinateForUIScale(Game1.viewport.Width);
      var vpHeight = (int)Utility.ModifyCoordinateForUIScale(Game1.viewport.Height);
      var xPositionOnScreen = menu.xPositionOnScreen;
      var yPositionOnScreen = menu.yPositionOnScreen;
      var width = menu.width;
      var height = menu.height;
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

      DrawTab($"Viewport: {Game1.viewport.Width}/{Game1.viewport.Height} - {vpWidth}/{vpHeight}", x, y + h * i++);
      DrawTab($"Viewport position: X:{Game1.viewport.X} Y:{Game1.viewport.Y}", x, y + h * i++);
      DrawTab($"Zoom: {Game1.options.zoomLevel} Scale: {Game1.options.uiScale}", x, y + h * i++);
      // DrawTab($"Zoom - UI: {Game1.options.zoomLevel - Game1.options.uiScale}", x, y + h * i++);
      // DrawTab($"Pixel zoom: {Game1.pixelZoom}", x, y + h * i++);

      DrawTab($"Player location: {Utility.ModifyCoordinatesForUIScale(pl)}", x, y + h * i++);
      // DrawDot(b, Utility.ModifyCoordinatesForUIScale(pl), Color.Pink);

      // var menuPosition = GetMenuPositionOnScreen(width, height);
      // DrawTab($"Menu position: X:{(int)menuPosition.X} Y:{(int)menuPosition.Y}", x, y + h * i++);
      DrawTab($"Menu position: X:{xPositionOnScreen} Y:{yPositionOnScreen}", x, y + h * i++);
      // DrawDot(b, new Vector2(menuPosition.X, menuPosition.Y), Color.Yellow);
      // DrawDot(b, new Vector2(xPositionOnScreen, yPositionOnScreen), Color.Orange);

      // DrawTab($"Mouse: X:{Game1.getMousePosition().X} Y:{Game1.getMousePosition().Y}", x, y + h * i++);
      // DrawDot(b, new Vector2(Game1.getMousePosition().X, Game1.getMousePosition().Y), Color.LightBlue);
      // DrawTab($"X:{Game1.getMouseX()} Y:{Game1.getMouseY()}", Game1.getMousePosition().X + 32, Game1.getMousePosition().Y - 32);
      // DrawMouse();

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
      if (distance == 0) return;
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

    private static Vector2 GetMenuPositionOnScreen(int width, int height)
    {
      var playerStandingPosition = Game1.player.getStandingPosition();
      var offset = new Vector2(-width / 2f, -height / 2f);
      var playerYOffset = Utility.ModifyCoordinateForUIScale(-48);
      var playerCenterPoint = new Vector2(playerStandingPosition.X - (float)Game1.viewport.X, playerStandingPosition.Y - (float)Game1.viewport.Y);
      var playerCenterPositionOnScreen = Utility.ModifyCoordinatesForUIScale(playerCenterPoint);
      playerCenterPositionOnScreen.Y += playerYOffset;

      return playerCenterPositionOnScreen + offset;
    }
  }
}