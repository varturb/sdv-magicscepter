using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace MagicScepter
{
  public class ModEntry : Mod
  {
    LocationDialog dialog;

    public override void Entry(IModHelper helper)
    {
      dialog = new LocationDialog(Helper);
      helper.Events.Input.ButtonPressed += OnButtonPressed;
    }

    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
      if (CanWandBeUsed(e))
      {
        dialog.ShowLocationDialog();
      }
    }

    private static bool CanWandBeUsed(ButtonPressedEventArgs e)
    {
      var player = Game1.player;

      return Context.IsWorldReady
          && Game1.activeClickableMenu == null
          && player.IsLocalPlayer
          && player.CurrentTool != null
          && player.CurrentTool is Wand
          && player.CanMove
          && !player.isRidingHorse()
          && !player.bathingClothes.Value
          && e.Button.IsUseToolButton()
          && !InOnScreenMenu(e.Cursor);
    }

    private static bool InOnScreenMenu(ICursorPosition cursor)
    {
      bool save = Game1.uiMode;
      Game1.uiMode = true;
      Vector2 v = cursor.GetScaledScreenPixels();
      Game1.uiMode = save;
      int x = (int)v.X;
      int y = (int)v.Y;
      for (int i = 0; i < Game1.onScreenMenus.Count; i++)
      {
        if (Game1.onScreenMenus[i].isWithinBounds(x, y))
        {
          return true;
        }
      }
      return false;
    }
  }
}
