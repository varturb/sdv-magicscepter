using System.Collections.Generic;
using System.Linq;
using MagicScepter.Models;
using MagicScepter.Helpers;
using Newtonsoft.Json;
using StardewValley;

namespace MagicScepter.Handlers
{
  public static class ResponseHandler
  {
    private const string miniObeliskID = "magicscepter_miniobelisk";

    public static List<Response> GetResponses()
    {
      var responses = new List<Response>();
      var warpObjects = GetWarpObjects();

      foreach (var wo in warpObjects)
      {
        responses.Add(new Response(wo.ID, wo.Text));
      }
      responses.Add(new Response("label.cancel", ModUtility.Helper.Translation.Get("label.cancel")));

      return responses;
    }

    public static void HandleResponse(string responseKey)
    {
      GetWarpObjects().FirstOrDefault(wo => wo.ID == responseKey)?.Warp();
    }

    public static List<WarpObject> GetWarpObjects()
    {
      var data = FileHelper.ReadFileData<DataEntry>(@"data.json");
      var warpDataItems = JsonConvert.DeserializeObject<List<DataEntry>>(data);
      var warpObjects = new List<WarpObject>();

      foreach (var dataItem in warpDataItems)
      {
        if (dataItem.ID == miniObeliskID)
        {
          var miniobelisks = GetMiniObeliskObjects(dataItem);
          foreach (var m in miniobelisks)
          {
            if (m.CanWarp)
            {
              warpObjects.Add(m);
            }
          }
          continue;
        }

        var warpObject = new WarpObject(dataItem);
        if (warpObject.CanWarp)
        {
          warpObjects.Add(warpObject);
        }
      }

      return warpObjects.OrderBy(x => x.Order).ToList();
    }

    private static List<MiniObeliskObject> GetMiniObeliskObjects(DataEntry dataItem)
    {
      var miniObeliskObjects = new List<MiniObeliskObject>();
      var miniObelisks = LocationHelper.FindObjects(dataItem.Warp.Do.Name);

      for (var i = 0; i < miniObelisks.Count; i++)
      {
        var m = miniObelisks[i];
        miniObeliskObjects.Add(new MiniObeliskObject(dataItem, i, (int)m.TileLocation.X, (int)m.TileLocation.Y));  
      }
      return miniObeliskObjects;
    }
  }
}