using Microsoft.Xna.Framework;

namespace MagicScepter.WarpLocations
{
  public abstract class WarpLocationBase
  {
    public abstract int Order { get; }
    internal abstract string LocationName { get; }
    public abstract string DialogLabel { get; }
    internal abstract string ObeliskName { get; }
    public abstract bool CanWarp { get; }
    public string DialogKey;
    public string DialogText;
    public abstract Rectangle SpirteSource { get; }

    public WarpLocationBase()
    {
      DialogKey = DialogLabel;
      DialogText = ModUtility.Helper.Translation.Get(DialogKey);
    }

    public abstract void Warp();
  }
}