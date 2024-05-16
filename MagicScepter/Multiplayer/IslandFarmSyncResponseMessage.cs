namespace MagicScepter.Multiplayer
{
  public class IslandFarmSyncResponseMessage
  {
    public bool CanTeleport { get; }

    public IslandFarmSyncResponseMessage(bool canTeleport)
    {
      CanTeleport = canTeleport;
    }
  }
}