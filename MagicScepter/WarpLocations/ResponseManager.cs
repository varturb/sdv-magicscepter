using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public static class ResponseManager
  {
    private static List<OrderedResponse> Responses;
    private static List<WarpLocationBase> WarpLocations;

    public static List<Response> GetResponses()
    {
      Initialize();
      AddCancelResponse();

      return Responses.OrderBy(response => response.Order).Select(r => r.Response).ToList();
    }

    public static List<WarpLocationBase> GetWarpLocations()
    {
      Initialize();

      return WarpLocations;
    }

    public static void HandleResponse(string responseKey)
    {
      WarpLocations
        .FirstOrDefault(location => location.DialogKey == responseKey)
        ?.Warp();
    }

    private static void Initialize()
    {
      Responses = new List<OrderedResponse>();
      WarpLocations = new List<WarpLocationBase>();

      AddResponse(new Farm());
      AddResponse(new Beach());
      AddResponse(new Mountain());
      AddResponse(new Desert());
      AddResponse(new Island());
      AddResponse(new IslandFarm());
      AddResponse(new DeepWoods());
      AddResponse(new RidgesideVillage());
      AddResponse(new RidgesideVillageFarm());
      AddResponse(new EastScarp());
      AddResponse(new EastScarpFarm());
      AddResponse(new DowntownZuzu());
      AddResponse(new VisitMountVapius());
      AddMiniObeliskResponses();
    }

    private static void AddResponse(WarpLocationBase location)
    {
      if (location.CanWarp)
      {
        Responses.Add(new(location.Order, new Response(location.DialogKey, location.DialogText)));
        WarpLocations.Add(location);
      }
    }

    private static void AddMiniObeliskResponses()
    {
      var miniObelisks = MiniObeliskManager.GetMiniObelisks();
      foreach (var miniObelisk in miniObelisks)
      {
        if (miniObelisk.CanWarp)
        {
          Responses.Add(new(miniObelisk.Order, new Response(miniObelisk.DialogKey, miniObelisk.DialogText)));
          WarpLocations.Add(miniObelisk);
        }
      }
    }

    private static void AddCancelResponse()
    {
      Responses.Add(new(10000, new Response("dialog.cancel", ModUtility.Helper.Translation.Get("dialog.cancel"))));
    }
  }
}