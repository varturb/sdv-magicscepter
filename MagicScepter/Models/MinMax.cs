namespace MagicScepter.Models
{
  public class MinMax
  {
    public int Min { get; set; }
    public int Max { get; set; }

    public MinMax(int min, int max)
    {
      Min = min;
      Max = max;
    }
  }
}