using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;

namespace MagicScepter
{
  public class LocationDialog
  {
    private readonly IModHelper helper;
    private List<WarpLocation> miniObelisks;

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
      var responses = new List<OrderedResponse>
      {
        new (1,new(WarpLocationChoice.Farm.ToString(), helper.Translation.Get("dialog.location.farm")))
      };

      foreach (var building in Game1.getFarm().buildings)
      {
        switch (building.buildingType.Value)
        {
          case Obelisks.Beach:
            responses.Add(new(2, new Response(WarpLocationChoice.Beach.ToString(), helper.Translation.Get("dialog.location.beach"))));
            break;
          case Obelisks.Mountain:
            responses.Add(new(3, new Response(WarpLocationChoice.Mountain.ToString(), helper.Translation.Get("dialog.location.mountain"))));
            break;
          case Obelisks.Desert:
            responses.Add(new(4, new Response(WarpLocationChoice.Desert.ToString(), helper.Translation.Get("dialog.location.desert"))));
            break;
          case Obelisks.Island:
            responses.Add(new(5, new Response(WarpLocationChoice.Island.ToString(), helper.Translation.Get("dialog.location.island"))));

            var islandWest = Game1.locations.First(loc => loc.Name == "IslandWest") as IslandWest;
            if ((bool)islandWest?.farmObelisk.Value)
              responses.Add(new(6, new Response(WarpLocationChoice.IslandFarm.ToString(), helper.Translation.Get("dialog.location.islandFarm"))));
            break;
          case Obelisks.DeepWoods:
            responses.Add(new(7, new Response(WarpLocationChoice.DeepWoods.ToString(), helper.Translation.Get("dialog.location.deepWoods"))));
            break;
          case Obelisks.Ridgeside:
            responses.Add(new(8, new Response(WarpLocationChoice.Ridgeside.ToString(), helper.Translation.Get("dialog.location.ridgeside"))));
            break;
        }
      }

      miniObelisks = new List<WarpLocation>();
      var index = 1;
      foreach (var obj in Game1.getFarm().objects.Pairs)
      {
        if (obj.Value.Name == Obelisks.MiniObelisk)
        {
          var name = $"{obj.Value.Name} #{index}";
          var x = (int)obj.Value.TileLocation.X;
          var y = (int)obj.Value.TileLocation.Y;
          responses.Add(new(8 + index, new Response($"{WarpLocationChoice.MiniObelisk} #{index}", helper.Translation.Get("dialog.location.miniObelisk", new { number = index }))));
          miniObelisks.Add(new WarpLocation(name, x, y));
          index++;
        }
      }

      responses.Add(new(100, new Response(WarpLocationChoice.None.ToString(), helper.Translation.Get("dialog.cancel"))));

      return responses.OrderBy(r => r.Order).Select(r => r.Response).ToList();
    }

    private void HandleAnswer(Farmer farmer, string answer)
    {
      _ = Enum.TryParse(answer, out WarpLocationChoice choice);

      if (answer.Contains(WarpLocationChoice.MiniObelisk.ToString()))
      {
        choice = WarpLocationChoice.MiniObelisk;
      }

      switch (choice)
      {
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
        case WarpLocationChoice.MiniObelisk:
          MiniObeliskWarp(answer);
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

    private void MiniObeliskWarp(string answer)
    {
      var index = int.Parse(answer.Replace($"{WarpLocationChoice.MiniObelisk} #", ""));
      var miniObelisk = miniObelisks[index - 1];
      var warpLocationCoords = GetValidWarpTile(miniObelisk.CoordX, miniObelisk.CoordY);
      if (warpLocationCoords == null)
      {
        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MiniObelisk_NeedsSpace"));
        return;
      }
      var warpLocation = new WarpLocation(WarpLocationChoice.Farm.ToString(), (int)warpLocationCoords.Value.X, (int)warpLocationCoords.Value.Y);
      BetterWand.Warp(warpLocation);
    }

    private static Vector2? GetValidWarpTile(int x, int y)
    {
      var targetLocation = new Vector2(x, y + 1);

      if (CanWarpHere(targetLocation))
      {
        return targetLocation;
      }
      targetLocation = new Vector2(x - 1, y);
      if (CanWarpHere(targetLocation))
      {
        return targetLocation;
      }
      targetLocation = new Vector2(x + 1, y);
      if (CanWarpHere(targetLocation))
      {
        return targetLocation;
      }
      targetLocation = new Vector2(x, y - 1);
      if (CanWarpHere(targetLocation))
      {
        return targetLocation;
      }

      return null;
    }

    private static bool CanWarpHere(Vector2 v)
    {
      return Game1.getFarm().CanItemBePlacedHere(v);
    }
  }
}