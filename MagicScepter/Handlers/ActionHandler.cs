using StardewValley;
using System.Linq;
using MagicScepter.UI;

namespace MagicScepter.Handlers
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
      var responses = ResponseHandler.GetResponses();
      if (responses.Count == 2)
      {
        var farmResponseKey = responses.First().responseKey;
        ResponseHandler.HandleResponse(farmResponseKey);
        return;
      }

      Game1.player.currentLocation.createQuestionDialogue(
        ModUtility.Helper.Translation.Get("label.title"),
        responses.ToArray(),
        HandleAnswer
      );
    }

    private static void HandleAnswer(Farmer farmer, string answer)
    {
      ResponseHandler.HandleResponse(answer);
    }

    private static void ShowWarpMenu()
    {
      var warpObjects = ResponseHandler.GetWarpObjects();
      if (warpObjects.Count == 1)
      {
        var farmResponse = warpObjects.First();
        ResponseHandler.HandleResponse(farmResponse.ID);
      }
      else
      {
        Game1.activeClickableMenu = new WarpMenu(warpObjects);
      }
    }
  }
}