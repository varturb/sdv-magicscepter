using MagicScepter.Constants;

namespace MagicScepter
{
  public class ModConfig
  {
    public bool UseOldDialogMenu { get; set; } = false;
    public int Radius { get; set; } = ModConstants.DefaultScrollsRadius;
    public float Scale { get; set; } = ModConstants.DefaultScrollsScale;
    public float SelectedScale { get; set; } = ModConstants.DefaultSelectedScrollScale;
  }
}