using System;
using System.Text;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace MagicScepter.Helpers
{
  public static class GameHelper
  {
    public static float CalculateDepth(int offset = 0)
    {
      return 0.86f + offset / 20000f;
    }

    public static void ShowMessage(string message, MessageType type = MessageType.Global)
    {
      if (type == MessageType.Global)
      {
        Game1.showGlobalMessage(message);
      }
      else
      {
        var hudMessage = new HUDMessage(message, (int)type);
        Game1.addHUDMessage(hudMessage);
      }
    }

    public static void DrawFadedBackground(SpriteBatch b, float alpha = 0.75f)
    {
      if (!Game1.options.showClearBackgrounds)
      {
        var bounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
        b.Draw(Game1.fadeToBlackRect, new Vector2(bounds.X, bounds.Y), new Rectangle?(bounds), Color.Black * alpha, 0.0f, default, 1f, SpriteEffects.None, 0.9f);
      }
    }

    public static void DrawText(SpriteBatch b, string text, Vector2 position, float scale = 1f)
    {
      Utility.drawTextWithShadow(b, text, Game1.dialogueFont, position, Game1.textColor, scale, 0.15f);
    }

    public static void DrawSmallText(SpriteBatch b, string text, Vector2 position, float scale = 1f)
    {
      Utility.drawTextWithShadow(b, text, Game1.smallFont, position, Game1.textColor, scale, 0.15f);
    }

    public static Action<SpriteBatch, Action> DrawBackToFront => (SpriteBatch b, Action action) =>
    {
      try { b.End(); } catch { }
      b.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);

      action();

      b.End();
      b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
    };

    public static void DrawTextInScroll(SpriteBatch b, string text, int x, int y)
    {
      var texture = FileHelper.GetSpritesheetTexture();
      var textWidth = SpriteText.getWidthOfString(text);
      var position = new Vector2(x - textWidth / 2f, y);
      var alpha = 1f;
      var layerDepth = 0.88f;

      b.Draw(texture, position + new Vector2(-12f, -3f) * 4f, new Rectangle(64, 0, 12, 18), Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 0.001f);
      b.Draw(texture, position + new Vector2(0f, -3f) * 4f, new Rectangle(76, 0, 1, 18), Color.White * alpha, 0f, Vector2.Zero, new Vector2(textWidth, 4f), SpriteEffects.None, layerDepth - 0.001f);
      b.Draw(texture, position + new Vector2(textWidth, -12f), new Rectangle(77, 0, 12, 18), Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 0.001f);

      SpriteText.drawStringWithScrollCenteredAt(b, text, x, y, scrollType: -1);
    }

    public static float GetTextScale(string text, SpriteFont font, float maxWidth)
    {
      var width = font.MeasureString(text).X;
      var scale = 1f;
      while (width > maxWidth && scale > 0.25f)
      {
        scale -= 0.05f;        
        width *= scale;
      }

      return scale;
    }
  }
}