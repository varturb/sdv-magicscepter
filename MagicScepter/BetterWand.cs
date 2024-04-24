using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter
{
  public static class BetterWand
  {
    public static void Warp(string location, int x, int y)
    {
      BeforeWarpAnimation();
      WarpPlayerTo(location, x, y);
      AfterWarpAnimation();
    }

    private static void BeforeWarpAnimation()
    {
      var currentLocation = Game1.currentLocation;
      var player = Game1.player;

      if (player.bathingClothes.Value || !player.IsLocalPlayer || player.onBridge.Value)
        return;

      for (int index = 0; index < 12; ++index)
        Game1.Multiplayer.broadcastSprites(
          player.currentLocation,
          new TemporaryAnimatedSprite(
            354,
            Game1.random.Next(25, 75),
            6,
            1,
            new Vector2(Game1.random.Next((int)player.position.X - 256, (int)player.position.X + 192), (float)Game1.random.Next((int)player.position.Y - 256, (int)player.position.Y + 192)),
            false,
             Game1.random.NextDouble() < 0.5
          ));
      currentLocation.playSound("wand");
      Game1.displayFarmer = false;
      player.temporarilyInvincible = true;
      player.temporaryInvincibilityTimer = -2000;
      player.Halt();
      player.faceDirection(2);
      player.CanMove = false;
      player.freezePause = 2000;
      Game1.flashAlpha = 1f;
    }

    private static void AfterWarpAnimation()
    {
      var player = Game1.player;
      new Rectangle(player.GetBoundingBox().X, player.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
      int num = 0;
      for (int x1 = (int)player.position.X + 8; x1 >= (int)player.position.X - 8; --x1)
      {
        Game1.Multiplayer.broadcastSprites(
          player.currentLocation,
          new TemporaryAnimatedSprite(
            6,
            new Vector2(x1, (float)player.position.Y) * 64f, Color.White, animationInterval: 50f
          )
          {
            layerDepth = 1f,
            delayBeforeAnimationStart = num * 25,
            motion = new Vector2(-0.25f, 0.0f)
          });
        ++num;
      }
    }

    private static void WarpPlayerTo(string location, int x, int y)
    {
      DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(() =>
      {
        var player = Game1.player;
        Game1.warpFarmer(location, x, y, false);
        Game1.fadeToBlackAlpha = 0.99f;
        Game1.screenGlow = false;
        Game1.displayFarmer = true;
        player.temporarilyInvincible = false;
        player.temporaryInvincibilityTimer = 0;
        player.CanMove = true;
      }), 1000);
    }
  }
}