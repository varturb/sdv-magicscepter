using Microsoft.Xna.Framework;

namespace MagicScepter.MultipleMiniObelisksMod
{
  public class MiniObelisk
  {
    public string LocationName { get; set; }
    public Vector2 Tile { get; set; }
    public string CustomName { get; set; }

    public MiniObelisk(string locationName, Vector2 tile, string customName)
    {
      LocationName = locationName;
      Tile = tile;
      CustomName = customName;
    }
  }
}