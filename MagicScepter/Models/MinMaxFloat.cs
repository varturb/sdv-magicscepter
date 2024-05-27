namespace MagicScepter.Models
{
  public class MinMaxFloat
  {
    public float Min { get; set; }
    public float Max { get; set; }

    public MinMaxFloat(float min, float max)
    {
      Min = min;
      Max = max;
    }
  }
}