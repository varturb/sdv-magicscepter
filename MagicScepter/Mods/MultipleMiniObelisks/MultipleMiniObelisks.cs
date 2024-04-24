using System.Collections.Generic;
using StardewValley;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MagicScepter.Mods.MultipleMiniObelisks
{
  public static class MultipleMiniObelisks
  {
    private const string modUniqueID = "PeacefulEnd.MultipleMiniObelisks";
    private const string modDataKey = $"{modUniqueID}/obelisk-locations";
    private const string miniObelistGameObjectId = "238";

    public static void OpenWarpMenu()
    {
      var obelisk = FindObelisk();
      if (obelisk != null)
      {
        var location = Game1.getLocationFromName(obelisk.LocationName);
        var existingObject = location.getObjectAtTile((int)obelisk.Tile.X, (int)obelisk.Tile.Y);
        if (existingObject != null)
        {
          var tempObject = new Object(new Vector2(-1, -1), miniObelistGameObjectId);
          tempObject.checkForAction(Game1.player);
        }
      }
    }

    public static bool CanWarp()
    {
      return FindObelisk() != null;
    }

    private static MiniObelisk FindObelisk()
    {
      var obelisks = JsonConvert.DeserializeObject<List<MiniObelisk>>(Game1.MasterPlayer.modData[modDataKey]);
      return obelisks.Count > 0 ? obelisks.First() : null;
    }
  }
}