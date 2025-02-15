using System;
using MagicScepter.Constants;
using MagicScepter.Helpers;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public class ScrollComponent : ClickableTextureComponent
  {
    public string ID;
    public bool Hovered = false;
    private int buttonRadius = 0;
    private float ageMS = 0;
    private float alpha = 0f;
    private readonly Action<string> action;
    private readonly TeleportScroll teleportScroll;
    private readonly int count;
    private static int ScrollsRadius => ModUtility.Config.Radius;
    private static float ScrollScale => ModUtility.Config.Scale;
    private static float SelectedScrollScale => ModUtility.Config.SelectedScale;
    private static Texture2D scrollsTexture = FileHelper.GetScrollsTexture();
    private readonly bool previewMode = false;
    private const float expandTimeMS = 200f;

    public ScrollComponent(TeleportScroll teleportScroll, int count, int x, int y, Rectangle source, Action<string> action, bool previewMode = false)
      : base(new Rectangle(x, y, 64, 64), FileHelper.GetSpritesheetTexture(), source, 1f)
    {
      this.count = count;
      this.teleportScroll = teleportScroll;
      this.action = action;
      this.previewMode = previewMode;
      scale = ScrollScale;
      ID = teleportScroll.ID;
      myID = teleportScroll.Order;

      texture = FileHelper.GetSpritesheetTexture();
    }

    public void SetupIDs(int upID = -1, int downID = -1, int leftID = -1, int rightID = -1)
    {
      upNeighborID = upID;
      downNeighborID = downID;
      leftNeighborID = leftID;
      rightNeighborID = rightID;
    }

    public void Update(GameTime time, bool selected, Rectangle parentBounds)
    {
      ageMS += time.ElapsedGameTime.Milliseconds;

      if (ageMS > expandTimeMS)
      {
        ageMS = expandTimeMS;
      }

      alpha = !previewMode ? ageMS / expandTimeMS : 1f;
      buttonRadius = !previewMode ? (int)(ageMS / expandTimeMS * ScrollsRadius) : ScrollsRadius;

      if (scale > ScrollScale)
      {
        scale = Utility.MoveTowards(scale, ScrollScale, (float)(time.ElapsedGameTime.Milliseconds / 1000f * 10f));
      }
      else
      {
        scale = !previewMode ? ageMS / expandTimeMS * ScrollScale : ScrollScale;
      }

      if (selected)
      {
        scale = SelectedScrollScale;
      }

      var rotation = ModUtility.Config.Rotation == ModConstants.RotationClockwise ? -2f : 2f;
      var idx = teleportScroll.Order - 1;
      var num = Utility.Lerp(0f, MathF.PI * rotation, (float)idx / (float)count);
      bounds.X = (int)((float)(parentBounds.X + parentBounds.Width / 2f + (int)(-Math.Sin(num) * (double)buttonRadius) * 4) - (float)bounds.Width / 2f);
      bounds.Y = (int)((float)(parentBounds.Y + parentBounds.Height / 2f + (int)(-Math.Cos(num) * (double)buttonRadius) * 4) - (float)bounds.Height / 2f);
    }

    public void OnClick(int x, int y)
    {
      if (containsPoint(x, y))
      {
        action(teleportScroll.ID);
      }
    }

    public override bool containsPoint(int x, int y)
    {
      var contains = base.containsPoint(x, y);
      if (contains)
      {
        scale = SelectedScrollScale;
      }
      return contains;
    }

    public override void tryHover(int x, int y, float maxScaleIncrease = 0.1f)
    {
      if (alpha == 1f)
      {
        Hovered = containsPoint(x, y);
        base.tryHover(x, y, SelectedScrollScale);
      }
    }

    public override void draw(SpriteBatch b)
    {
      var color = Color.White * alpha;
      b.Draw(
        texture,
        new Vector2((float)(bounds.X) + (float)(sourceRect.Width / 2) * baseScale, (float)(bounds.Y) + (float)(sourceRect.Height / 2) * baseScale),
        new Rectangle(0, 0, 64, 64),
        color,
        0f,
        new Vector2(sourceRect.Width / 2, sourceRect.Height / 2),
        scale,
        SpriteEffects.None,
        ModConstants.DefaultLayerDepth
      );
      b.Draw(
        scrollsTexture,
        new Vector2((float)(bounds.X) + (float)(sourceRect.Width / 2) * baseScale, (float)(bounds.Y) + (float)(sourceRect.Height / 2) * baseScale),
        sourceRect,
        color,
        0f,
        new Vector2(sourceRect.Width / 2, sourceRect.Height / 2),
        scale,
        SpriteEffects.None,
        ModConstants.DefaultLayerDepth
      );
    }
  }
}