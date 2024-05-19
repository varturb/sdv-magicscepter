namespace MagicScepter
{
  public class ModConfig
  {
    public static ModConfig Defaults { get; } = new();

    public bool UseOldDialogMenu { get; set; } = false;
  }
}