using StardewValley;
using MagicScepter.WarpLocations;
using System.Linq;
using MagicScepter.UI;

namespace MagicScepter
{
  public static class ActionHandler
  {
    public static void HandleWarpAction()
    {
      if (ModUtility.Config.UseOldDialogMenu)
      {
        ShowWarpDialog();
      }
      else
      {
        ShowWarpMenu();
      }
    }

    private static void ShowWarpDialog()
    {
      var responses = ResponseManager.GetResponses();
      if (responses.Count == 2)
      {
        var farmResponseKey = new WarpLocations.Farm().DialogKey;
        ResponseManager.HandleResponse(farmResponseKey);
        return;
      }

      Game1.player.currentLocation.createQuestionDialogue(
        ModUtility.Helper.Translation.Get("dialog.title"),
        responses.ToArray(),
        HandleAnswer
      );
    }

    private static void HandleAnswer(Farmer farmer, string answer)
    {
      ResponseManager.HandleResponse(answer);
    }

    private static void ShowWarpMenu()
    {
      var warpLocations = ResponseManager.GetWarpLocations();
      if (warpLocations.Count == 1)
      {
        var farmResponse = warpLocations.First();
        ResponseManager.HandleResponse(farmResponse.DialogKey);
      }
      else
      {
        Game1.activeClickableMenu = new WarpMenu(warpLocations);
      }
    }
  }
}