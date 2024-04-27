namespace MagicScepter.Multiplayer
{
  public class IslandFarmBroadcastMessage
  {
    public bool CanWarp { get; }

    public IslandFarmBroadcastMessage(bool canWarp)
    {
      CanWarp = canWarp;
    }
  }
}