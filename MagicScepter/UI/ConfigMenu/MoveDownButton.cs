using System.Collections.Generic;
using MagicScepter.Constants;
using MagicScepter.Helpers;
using MagicScepter.Models;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MagicScepter.UI
{
  public class MoveDownButton : ButtonBase
  {
    private readonly int index;
    private readonly bool skip;
    private readonly ConfigMenu configMenu;

    public MoveDownButton(int index, ConfigMenu configMenu, bool skip = false)
      : base(48, 48, new Rectangle(421, 472, 12, 12), 3f, TranslatedKeys.MoveDown)
    {
      this.index = index;
      this.skip = skip;
      this.configMenu = configMenu;
    }

    protected override void SetupTexture()
    {
      SetTexture(Game1.mouseCursors);
    }

    public void UpdatePosition(int x, int y)
    {
      xPositionOnScreen = x;
      yPositionOnScreen = y;
      ClickableComponent.bounds = new Rectangle(x, y, width, height);
    }

    protected override void ButtonPressed()
    {
      if (skip)
        return;
      
      var warpObjects = configMenu.warpObjects;
      var next = warpObjects[index + 1].ConvertToSaveDataEntry();
      var curr = warpObjects[index].ConvertToSaveDataEntry();
      (curr.Order, next.Order) = (next.Order, curr.Order);

      var entiresToSave = new List<SaveDataEntry> { next, curr };
      ModDataHelper.UpdateSaveData(entiresToSave);
      configMenu.RefreshWarpObjects();
      
      Game1.playSound("smallSelect");
    }

    protected override void ButtonHovered(bool hovered)
    {
      if (skip)
        Hovered = false;
    }

    protected override void Draw()
    {
      if (skip)
        return;

      ClickableComponent.draw(
        Game1.spriteBatch, 
        Color.White * 0.8f, 
        GameHelper.CalculateDepth(ClickableComponent.bounds.Y)
      );
    }
  }
}