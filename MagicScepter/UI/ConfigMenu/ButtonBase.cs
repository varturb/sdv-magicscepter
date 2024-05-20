using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace MagicScepter.UI
{
  public abstract class ButtonBase : IClickableMenu
  {
    public ClickableTextureComponent ClickableComponent => button;
    protected abstract void SetupTexture();

    private readonly ClickableTextureComponent button;
    public bool Hovered { get; protected set; } = false;
    public string HoverText { get; protected set; }
    private Texture2D texture;

    public ButtonBase(int width, int height, Rectangle textureRect, float scale, string hoverText = "")
    {
      base.width = width;
      base.height = height;
      HoverText = hoverText;

      SetupTexture();

      button = new ClickableTextureComponent(
        default,
        texture,
        textureRect,
        scale
      );
    }

    protected void SetTexture(Texture2D texture)
    {
      this.texture = texture;
    }

    public void SetupIDs(int ID, int upID, int downID, int leftID, int rightID)
    {
      button.myID = ID;
      button.upNeighborID = upID;
      button.downNeighborID = downID;
      button.leftNeighborID = leftID;
      button.rightNeighborID = rightID;
      button.fullyImmutable = true;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (isWithinBounds(x, y))
      {
        ButtonPressed();
        base.receiveLeftClick(x, y, playSound);
      }
    }

    public override void performHoverAction(int x, int y)
    {
      button.tryHover(x, y);
      Hovered = isWithinBounds(x, y);
      ButtonHovered(Hovered);
      base.performHoverAction(x, y);
    }

    protected virtual void ButtonPressed()
    {
    }

    protected virtual void ButtonHovered(bool hovered)
    {
    }

    protected virtual void Draw()
    {
      button.draw(Game1.spriteBatch);
    }

    public override void draw(SpriteBatch b)
    {
      Draw();
    }
  }
}