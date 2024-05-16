namespace MagicScepter.Helpers
{
  public static class GameHelper
  {
    public static float CalculateDepth(int offset = 0)
    {
      return 0.86f + offset / 20000f;
    }
  }
}