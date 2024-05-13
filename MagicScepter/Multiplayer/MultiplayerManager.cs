using System.Linq;
using MagicScepter.Handlers;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MagicScepter.Multiplayer
{
  public static class MultiplayerManager
  {
    public static bool CanWarpToIslandFarm { get; set; } = false;
    private const string islandFarmID = "magicscepter_islandfarm";

    public static void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
    {
      if (e.FromModID != ModUtility.Manifest.UniqueID)
      {
        return;
      }

      if (e.Type == nameof(IslandFarmSyncRequestMessage))
      {
        if (Context.IsMainPlayer)
        {
          SendSyncResponseMessage();
        }
      }
      else if (e.Type == nameof(IslandFarmSyncResponseMessage))
      {
        if (!Context.IsMainPlayer)
        {
          var message = e.ReadAs<IslandFarmSyncResponseMessage>();
          CanWarpToIslandFarm = message.CanWarp;
        }
      }
    }

    public static void OnDayStarted(object sender, DayStartedEventArgs e)
    {
      if (Context.IsMainPlayer)
      {
        var islandWarpObject = ResponseHandler.GetWarpObjects().FirstOrDefault(x => x.ID == islandFarmID);
        CanWarpToIslandFarm = islandWarpObject?.CanWarp ?? false;
      }
      else
      {
        SendSyncRequestMessage();
      }
    }

    private static void SendSyncResponseMessage()
    {
      ModUtility.Helper.Multiplayer.SendMessage(
        new IslandFarmSyncResponseMessage(CanWarpToIslandFarm), 
        nameof(IslandFarmSyncResponseMessage), 
        modIDs: new[] { ModUtility.Manifest.UniqueID }
      );
    }

    private static void SendSyncRequestMessage()
    {
      ModUtility.Helper.Multiplayer.SendMessage(
        new IslandFarmSyncRequestMessage(), 
        nameof(IslandFarmSyncRequestMessage), 
        modIDs: new[] { ModUtility.Manifest.UniqueID }
      );
    }
  }
}