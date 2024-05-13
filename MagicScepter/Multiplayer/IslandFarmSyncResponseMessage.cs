namespace MagicScepter.Multiplayer
{
  public class IslandFarmSyncResponseMessage
  {
    public bool CanWarp { get; }

    public IslandFarmSyncResponseMessage(bool canWarp)
    {
      CanWarp = canWarp;
    }
  }
}