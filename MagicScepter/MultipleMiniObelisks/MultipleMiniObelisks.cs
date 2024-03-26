using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MagicScepter.MultipleMiniObelisksMod
{
  public static class MultipleMiniObelisks
  {
    private const string modUniqueID = "PeacefulEnd.MultipleMiniObelisks";
    private const string modDataKey = $"{modUniqueID}/obelisk-locations";
    private const string miniObelistGameObjectId = "238";

    public static bool IsLoaded(IModHelper helper)
    {
      return helper.ModRegistry.IsLoaded(modUniqueID);
    }

    public static Response GetResponse(IModHelper helper)
    {
      if (FindObelisk() != null)
      {
        return new Response(WarpLocationChoice.MiniObelisk.ToString(), helper.Translation.Get("dialog.location.miniObelisk"));
      }
      return null;
    }

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

    private static MiniObelisk FindObelisk()
    {
      var obelisks = JsonConvert.DeserializeObject<List<MiniObelisk>>(Game1.MasterPlayer.modData[modDataKey]);
      return obelisks.Count > 0 ? obelisks.First() : null;
    }
  }
}