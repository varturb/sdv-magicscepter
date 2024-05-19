using System.Collections.Generic;
using System.Linq;
using MagicScepter.Constants;
using MagicScepter.Models;
using MagicScepter.Helpers;
using Newtonsoft.Json;

namespace MagicScepter.Handlers
{
  public static class ScrollHandler
  {
    public static void HandleResponse(string responseKey)
    {
      GetTeleportScrolls().FirstOrDefault(wo => wo.ID == responseKey)?.Teleport();
    }

    public static List<TeleportScroll> GetTeleportScrolls()
    {
      var data = FileHelper.ReadFileData<DataEntry>(@"data.json");
      var teleportDataItems = JsonConvert.DeserializeObject<List<DataEntry>>(data);
      var teleportScrolls = new List<TeleportScroll>();

      foreach (var dataItem in teleportDataItems)
      {
        if (dataItem.ID == ModConstants.MiniObeliskID)
        {
          var miniobelisks = GetMiniObeliskScrolls(dataItem);
          foreach (var m in miniobelisks)
          {
            if (m.CanTeleport)
            {
              teleportScrolls.Add(m);
            }
          }
          continue;
        }

        var teleportScroll = new TeleportScroll(dataItem);
        if (teleportScroll.CanTeleport)
        {
          teleportScrolls.Add(teleportScroll);
        }
      }

      return teleportScrolls.AdjustOrder();
    }

    private static List<MiniObeliskScroll> GetMiniObeliskScrolls(DataEntry dataItem)
    {
      var miniObeliskScrolls = new List<MiniObeliskScroll>();
      var miniObelisks = LocationHelper.FindObjects(dataItem.Action.Do.Name);

      for (var i = 0; i < miniObelisks.Count; i++)
      {
        var m = miniObelisks[i];
        miniObeliskScrolls.Add(new MiniObeliskScroll(dataItem, i, (int)m.TileLocation.X, (int)m.TileLocation.Y));
      }
      return miniObeliskScrolls;
    }
  }
}