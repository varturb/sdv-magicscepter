namespace MagicScepter.Models
{
  public class DataEntry
  {
    public string ID { get; set; }
    public int Order { get; set; }
    public string TranslationKey { get; set; }
    public int SpriteOffset { get; set; }
    public WarpDoWhen Warp { get; set; }
  }
}