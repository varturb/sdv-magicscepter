using System;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

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

    public static void DrawText(SpriteBatch b, string text, Vector2 position)
    {
      Utility.drawTextWithShadow(b, text, Game1.dialogueFont, position, Game1.textColor, 1f, 0.15f);
    }

    public static Action<SpriteBatch, Action> DrawBackToFront => (SpriteBatch b, Action action) =>
    {
      try { b.End(); } catch { }
      b.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);

      action();

      b.End();
      b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
    }; 
  }
}