using MagicScepter.Constants;
using MagicScepter.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MagicScepter.UI
{
  public class RenameButton : ButtonBase
  {
    private readonly int index;
    private readonly ConfigMenu configMenu;

    public RenameButton(int index, ConfigMenu configMenu)
      : base(56, 48, new Rectangle(64, 16, 16, 16), 3f, TranslatedKeys.Rename)
    {
      this.index = index;
      this.configMenu = configMenu;
    }

    protected override void SetupTexture()
    {
      var texture = Game1.temporaryContent.Load<Texture2D>(PathConstants.EmoteMenuTexturePath);
      SetTexture(texture);
    }

    public void UpdatePosition(int x, int y)
    {
      xPositionOnScreen = x;
      yPositionOnScreen = y;
      ClickableComponent.bounds = new Rectangle(x, y, width, height);
    }

    protected override void ButtonPressed()
    {
      Game1.playSound("smallSelect");
      Game1.activeClickableMenu = new RenameMenu(configMenu, configMenu.warpObjects[index]);
    }

    protected override void Draw()
    {
      ClickableComponent.draw(
        Game1.spriteBatch, 
        Color.White * 0.8f, 
        GameHelper.CalculateDepth(ClickableComponent.bounds.Y)
      );
    }
  }
}