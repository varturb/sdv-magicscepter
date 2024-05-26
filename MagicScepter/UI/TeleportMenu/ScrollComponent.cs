using System;
using MagicScepter.Constants;
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
    private static readonly Texture2D spritesheetTexture = ModUtility.Helper.ModContent.Load<Texture2D>(ModConstants.SpritesheetTexturePath);
    private int buttonRadius = 0;
    private float ageMS = 0;
    private float alpha = 0f;
    private readonly Action<string> action;
    private readonly TeleportScroll teleportScroll;
    private readonly int count;
    private const int endButtonRadius = 42;
    private const float endScale = 1f;
    private const float selectedScale = 1.5f;
    private const float expandTimeMS = 200f;

    public ScrollComponent(TeleportScroll teleportScroll, int count, int x, int y, Rectangle source, Action<string> action)
      : base(new Rectangle(x, y, 64, 64), spritesheetTexture, source, endScale)
    {
      this.count = count;
      this.teleportScroll = teleportScroll;
      this.action = action;
      ID = teleportScroll.ID;
      myID = teleportScroll.Order;
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

      alpha = ageMS / expandTimeMS;
      buttonRadius = (int)(ageMS / expandTimeMS * endButtonRadius);

      if (scale > endScale)
      {
        scale = Utility.MoveTowards(scale, endScale, (float)(time.ElapsedGameTime.Milliseconds / 1000f * 10f));
      }
      else
      {
        scale = ageMS / expandTimeMS * endScale;
      }

      if (selected)
      {
        scale = selectedScale;
      }

      var idx = teleportScroll.Order - 1;
      var num = Utility.Lerp(0f, MathF.PI * 2f, (float)idx / (float)count);
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
        scale = selectedScale;
      }
      return contains;
    }

    public override void tryHover(int x, int y, float maxScaleIncrease = 0.1f)
    {
      if (alpha == 1f)
      {
        Hovered = containsPoint(x, y);
        base.tryHover(x, y, selectedScale);
      }
    }


    public override void draw(SpriteBatch b)
    {
      var color = Color.White * alpha;
      base.draw(b, color, ModConstants.DefaultLayerDepth);
    }
  }
}