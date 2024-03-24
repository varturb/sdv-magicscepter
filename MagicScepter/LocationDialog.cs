using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

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
        HandleAnswer
      );
    }

    private List<Response> GetLocationResponses()
    {
      var responses = new List<Response>
      {
          new(WarpLocationChoice.Farm.ToString(), helper.Translation.Get("dialog.location.farm"))
      };

      foreach (var building in Game1.getFarm().buildings)
      {
        Console.WriteLine(building.buildingType.Value);
        switch (building.buildingType.Value)
        {
          case Obelisks.Beach:
            responses.Add(new Response(WarpLocationChoice.Beach.ToString(), helper.Translation.Get("dialog.location.beach")));
            break;
          case Obelisks.Mountain:
            responses.Add(new Response(WarpLocationChoice.Mountain.ToString(), helper.Translation.Get("dialog.location.mountain")));
            break;
          case Obelisks.Desert:
            responses.Add(new Response(WarpLocationChoice.Desert.ToString(), helper.Translation.Get("dialog.location.desert")));
            break;
          case Obelisks.Island:
            responses.Add(new Response(WarpLocationChoice.Island.ToString(), helper.Translation.Get("dialog.location.island")));

            var islandWest = Game1.locations.First(loc => loc.Name == "IslandWest") as IslandWest;
            if ((bool)islandWest?.farmObelisk.Value)
              responses.Add(new Response(WarpLocationChoice.IslandFarm.ToString(), helper.Translation.Get("dialog.location.islandFarm")));
            break;
          case Obelisks.DeepWoods:
            responses.Add(new Response(WarpLocationChoice.DeepWoods.ToString(), helper.Translation.Get("dialog.location.deepWoods")));
            break;
          case Obelisks.Ridgeside:
            responses.Add(new Response(WarpLocationChoice.Ridgeside.ToString(), helper.Translation.Get("dialog.location.ridgeside")));
            break;
        }
      }

      responses.Add(new Response(WarpLocationChoice.None.ToString(), helper.Translation.Get("dialog.cancel")));

      return responses;
    }

    private void HandleAnswer(Farmer farmer, string answer)
    {
      _ = Enum.TryParse(answer, out WarpLocationChoice choice);

      switch(choice) {
        case WarpLocationChoice.Farm:
        case WarpLocationChoice.IslandFarm:
          WandWarp(choice);
          break;  
        case WarpLocationChoice.Beach:
        case WarpLocationChoice.Mountain:
        case WarpLocationChoice.Desert:
        case WarpLocationChoice.Island:
        case WarpLocationChoice.DeepWoods:
        case WarpLocationChoice.Ridgeside:
          ObeliskWarp(choice, farmer);
          break;
        case WarpLocationChoice.None:
        default:
          break;
      }
    }

    private static void WandWarp(WarpLocationChoice choice) 
    {
      var location = WarpLocations.GetWarpLocation(choice);
      BetterWand.Warp(location);
    }

    private static void ObeliskWarp(WarpLocationChoice choice, Farmer farmer)
    {
      var obelisk = choice switch 
      {
        WarpLocationChoice.Beach => Obelisks.Beach,
        WarpLocationChoice.Mountain => Obelisks.Mountain,
        WarpLocationChoice.Desert => Obelisks.Desert,
        WarpLocationChoice.Island => Obelisks.Island,
        WarpLocationChoice.DeepWoods => Obelisks.DeepWoods,
        WarpLocationChoice.Ridgeside => Obelisks.Ridgeside,
        _ => null
      };
      
      if (obelisk == null)
        return;

      foreach (var building in Game1.getFarm().buildings)
      {
        if (building.buildingType.Value == obelisk)
        {
          building.doAction(new Vector2(building.tileX.Value, building.tileY.Value), farmer);
          break;
        }
      }
    }
  }
}