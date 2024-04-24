using System.Collections.Generic;
using System.Linq;
using MagicScepter.Mods;
using StardewValley;
using StardewValley.Buildings;

namespace MagicScepter.WarpLocations
{
  public static class LocationHelper
  {
    private const string EastScarpFarmLocation = "EastScarp_MeadowFarm";

    public static Building FindBuilding(string name)
    {
      var building = Game1.getFarm().buildings.FirstOrDefault(building => building.buildingType.Value == name);
      if (building != null) return building;

      if (ModManager.IsModLoaded(SupportedMod.EastScarp))
      {
        building = Game1.getLocationFromName(EastScarpFarmLocation).buildings.FirstOrDefault(building => building.buildingType.Value == name);
        if (building != null) return building;
      }

      return null;
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