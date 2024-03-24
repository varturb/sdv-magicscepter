using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Tools;

namespace MagicScepter
{
  public class LocationDialog
  {
    private readonly IModHelper helper;

    public LocationDialog(IModHelper helper)
    {
      this.helper = helper;
    }

    public void ShowLocationDialog()
    {
      var responses = GetLocationResponses();

      if (responses.Count == 2)
      {
        var farmLocation = WarpLocations.GetWarpLocation(WarpLocationChoice.Farm);
        BetterWand.Warp(farmLocation);
        return;
      }

      Game1.currentLocation.createQuestionDialogue(
        helper.Translation.Get("dialog.title"),
        responses.ToArray(),
        GetAnswer
      );
    }

    private List<Response> GetLocationResponses() {
      var responses = new List<Response>
      {
          new(WarpLocationChoice.Farm.ToString(), helper.Translation.Get("dialog.location.farm"))
      };

      foreach (var building in Game1.getFarm().buildings)
      {
        switch (building.buildingType.Value)
        {
          case "Water Obelisk":
            responses.Add(new Response(WarpLocationChoice.Beach.ToString(), helper.Translation.Get("dialog.location.beach")));
            break;
          case "Earth Obelisk":
            responses.Add(new Response(WarpLocationChoice.Mountain.ToString(), helper.Translation.Get("dialog.location.mountain")));
            break;
          case "Desert Obelisk":
            responses.Add(new Response(WarpLocationChoice.Desert.ToString(), helper.Translation.Get("dialog.location.desert")));
            break;
          case "Island Obelisk":
            responses.Add(new Response(WarpLocationChoice.Island.ToString(), helper.Translation.Get("dialog.location.island")));

            var islandWest = Game1.locations.First(loc => loc.Name == "IslandWest") as IslandWest;
            if ((bool)islandWest?.farmObelisk.Value)
              responses.Add(new Response(WarpLocationChoice.IslandFarm.ToString(), helper.Translation.Get("dialog.location.islandFarm")));
            break;
          case "Woods Obelisk":
            responses.Add(new Response(WarpLocationChoice.DeepWoods.ToString(), "Deep Woods"));
            break;
        }
      }

      responses.Add(new Response(WarpLocationChoice.None.ToString(), helper.Translation.Get("dialog.cancel")));

      return responses;
    }

    private void GetAnswer(Farmer farmer, string answer)
    {
      _ = Enum.TryParse(answer, out WarpLocationChoice choice);

      if (choice == WarpLocationChoice.None)
        return;

      var location = WarpLocations.GetWarpLocation(choice);
      BetterWand.Warp(location);
    }
  }
}