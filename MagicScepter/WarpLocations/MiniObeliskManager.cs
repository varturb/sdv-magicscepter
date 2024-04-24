using System.Collections.Generic;
using MagicScepter.Mods;

namespace MagicScepter.WarpLocations
{
  public static class MiniObeliskManager
  {
    private const string MiniObeliskName = "Mini-Obelisk";

    public static List<MiniObelisk> GetMiniObelisks()
    {
      if (ModManager.IsModLoaded(SupportedMod.MultipleMiniObelisks))
      {
        return GetMultipleMiniObelisks();
      }

      return GetVanillaMiniObelisks();
    }

    private static List<MiniObelisk> GetMultipleMiniObelisks()
    {
      return new List<MiniObelisk>() { new() };
    }

    private static List<MiniObelisk> GetVanillaMiniObelisks()
    {
      var list = new List<MiniObelisk>();
      var index = 0;
      foreach (var obj in LocationHelper.FindObjects(MiniObeliskName))
      {
        var coords = new Coords((int)obj.TileLocation.X, (int)obj.TileLocation.Y);
        var miniObelisk = new MiniObelisk(index++, coords);
        list.Add(miniObelisk);
      }

      return list;
    }
  }
}