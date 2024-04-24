using System;

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

    public WarpLocationBase()
    {
      DialogKey = DialogLabel;
    }

    public abstract void Warp();
  }
}