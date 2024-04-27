using MagicScepter.WarpLocations;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MagicScepter.Multiplayer
{
  public static class MultiplayerManager
  {
    private static IManifest manifest;
    private static IModHelper helper;
    public static bool CanWarpToIslandFarm { get; set; } = false;

    public static void Initialize(IModHelper modHelper, IManifest modManifest)
    {
      helper = modHelper;
      manifest = modManifest;
    }

    public static void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
    {
      if (e.FromModID != manifest.UniqueID)
      {
        return;
      }

      if (e.Type == nameof(IslandFarmUpdateMessage))
      {
        if (Context.IsMainPlayer)
        {
          SendBroadcastMessage();
        }
      }
      else if (e.Type == nameof(IslandFarmBroadcastMessage))
      {
        if (!Context.IsMainPlayer)
        {
          var message = e.ReadAs<IslandFarmBroadcastMessage>();
          CanWarpToIslandFarm = message.CanWarp;
        }
      }
    }

    public static void OnDayStarted(object sender, DayStartedEventArgs e)
    {
      if (Context.IsMainPlayer)
      {
        CanWarpToIslandFarm = new IslandFarm().CanWarp;
      }
      else
      {
        SendUpdateMessage();
      }
    }

    private static void SendBroadcastMessage()
    {
      helper.Multiplayer.SendMessage(new IslandFarmBroadcastMessage(CanWarpToIslandFarm), nameof(IslandFarmBroadcastMessage), modIDs: new[] { manifest.UniqueID });
    }

    private static void SendUpdateMessage()
    {
      helper.Multiplayer.SendMessage(new IslandFarmUpdateMessage(), nameof(IslandFarmUpdateMessage), modIDs: new[] { manifest.UniqueID });
    }
  }
}