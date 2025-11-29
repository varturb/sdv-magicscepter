using System.Collections.Generic;
using System.Linq;
using MagicScepter.Constants;
using MagicScepter.Mods;
using StardewValley;
using StardewValley.Buildings;

namespace MagicScepter.Helpers
{
  public static class LocationHelper
  {
    public static Building FindBuilding(string name)
    {
      try
      {
        var building = Game1.getFarm().buildings.FirstOrDefault(building => building.buildingType.Value.StartsWith(name));
        if (building != null) return building;

        if (ModManager.IsModLoaded(SupportedMod.EastScarp))
        {
          building = Game1.getLocationFromName(ModConstants.EastScarpFarmLocation).buildings.FirstOrDefault(building => building.buildingType.Value == name);
          if (building != null) return building;
        }

        if (ModManager.IsModLoaded(SupportedMod.RidgesideVillage))
        {
          building = Game1.getLocationFromName(ModConstants.RidgesideFarmLocation).buildings.FirstOrDefault(building => building.buildingType.Value == name);
          if (building != null) return building;
        }

        return null;
      }
      catch
      {
        return null;
      }
    }

    public static List<Object> FindObjects(string name)
    {
      return Game1.getFarm().objects.Pairs
        .ToList()
        .FindAll(obj => obj.Value.Name == name)
        .Select(obj => obj.Value)
        .ToList();
    }
  }
}