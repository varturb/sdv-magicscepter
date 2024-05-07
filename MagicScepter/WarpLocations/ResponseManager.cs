using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace MagicScepter.WarpLocations
{
  public static class ResponseManager
  {
    private static IModHelper Helper;
    private static List<OrderedResponse> Responses;
    private static List<WarpLocationBase> WarpLocations;

    internal static void Initialize(IModHelper helper)
    {
      Helper = helper;
    }

    public static List<Response> GetResponses()
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
      AddMiniObeliskResponses();
      AddCancelResponse();

      return Responses.OrderBy(response => response.Order).Select(r => r.Response).ToList();
    }

    public static void HandleResponse(string responseKey)
    {
      WarpLocations
        .FirstOrDefault(location => location.DialogKey == responseKey)
        ?.Warp();
    }

    private static void AddResponse(WarpLocationBase location)
    {
      if (location.CanWarp)
      {
        Responses.Add(new(location.Order, new Response(location.DialogKey, Helper.Translation.Get(location.DialogLabel))));
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
          var responeText = Helper.Translation.Get(miniObelisk.DialogLabel, new { number = miniObelisk.DisplayNumber });
          Responses.Add(new(miniObelisk.Order, new Response(miniObelisk.DialogKey, responeText)));
          WarpLocations.Add(miniObelisk);
        }
      }
    }

    private static void AddCancelResponse()
    {
      Responses.Add(new(10000, new Response("dialog.cancel", Helper.Translation.Get("dialog.cancel"))));
    }
  }
}