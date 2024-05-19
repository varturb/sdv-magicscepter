using System.Linq;
using MagicScepter.Constants;
using MagicScepter.Handlers;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MagicScepter.Multiplayer
{
  public static class MultiplayerManager
  {
    public static bool CanTeleportToIslandFarm { get; set; } = false;

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
          CanTeleportToIslandFarm = message.CanTeleport;
        }
      }
    }

    public static void OnDayStarted(object sender, DayStartedEventArgs e)
    {
      if (Context.IsMainPlayer)
      {
        var islandTeleportScroll = ScrollHandler.GetTeleportScrolls().FirstOrDefault(x => x.ID == ModConstants.IslandFarmID);
        CanTeleportToIslandFarm = islandTeleportScroll?.CanTeleport ?? false;
      }
      else
      {
        SendSyncRequestMessage();
      }
    }

    private static void SendSyncResponseMessage()
    {
      ModUtility.Helper.Multiplayer.SendMessage(
        new IslandFarmSyncResponseMessage(CanTeleportToIslandFarm),
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