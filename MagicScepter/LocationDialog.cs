using System.Linq;
using StardewModdingAPI;
using StardewValley;
using MagicScepter.WarpLocations;

namespace MagicScepter
{
  public class LocationDialog
  {
    private static IModHelper Helper;

    public static void Initialize(IModHelper helper)
    {
      Helper = helper;
    }

    public static void ShowLocationDialog()
    {
      var responses = ResponseManager.GetResponses();

      if (responses.Count == 2)
      {
        var farmResponse = responses.First().responseKey;
        ResponseManager.HandleResponse(farmResponse);
        return;
      }

      Game1.player.currentLocation.createQuestionDialogue(
        Helper.Translation.Get("dialog.title"),
        responses.ToArray(),
        HandleAnswer
      );
    }

    private static void HandleAnswer(Farmer farmer, string answer)
    {
      ResponseManager.HandleResponse(answer);
    }
  }
}