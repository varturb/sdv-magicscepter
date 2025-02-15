using System;
using MagicScepter.Constants;

namespace MagicScepter.Models
{
  public class Neighbourhood
  {
    public int Up { get; set; }
    public int Left { get; set; }
    public int Down { get; set; }
    public int Right { get; set; }

    public Neighbourhood(int up = -1, int left = -1, int down = -1, int right = -1)
    {
      Up = up;
      Left = left;
      Down = down;
      Right = right;
    }

    public static OrderThreshold CalculateThreshold(int count)
    {
      var leftThreshold = (int)Math.Round((float)count / 4 - 0.01f);
      var downThreshold = (int)Math.Round((float)count / 4 * 2 + 0.01f);
      var rightThreshold = count > 2 ? (int)Math.Round((float)count / 4 * 3 + 0.01f) : 1;
      var offset = 1;

      return new()
      {
        Up = offset,
        Left = Math.Min(leftThreshold + offset, count),
        Down = Math.Min(downThreshold + offset, count),
        Right = Math.Min(rightThreshold + offset, count)
      };
    }

    public static Neighbourhood GetNeighbourhood(int order, int count, int outsiderID, bool isOutsiderUp = false)
    {
      var t = CalculateThreshold(count);
      var neighbourhood = new Neighbourhood();

      if (count == 1)
      {
        neighbourhood.Up = isOutsiderUp ? outsiderID : -1;
        neighbourhood.Down = isOutsiderUp ? -1 : outsiderID;
      }
      else if (count == 2)
      {
        neighbourhood.Up = order == t.Up
          ? isOutsiderUp
            ? outsiderID : -1
          : order == t.Down
            ? order - 1 : -1;
        neighbourhood.Down = order == t.Down
          ? isOutsiderUp
            ? -1 : outsiderID
          : order == t.Up
            ? order + 1 : -1;
      }
      else
      {
        var substractOffset = order - 1;
        var addOffset = order + 1;
        if (substractOffset < 1) substractOffset = count;
        if (addOffset > count) addOffset = 1;

        neighbourhood.Up = order == t.Up
          ? isOutsiderUp
            ? outsiderID : -1
          : order < t.Down
            ? substractOffset : addOffset;
        neighbourhood.Down = order == t.Down
          ? !isOutsiderUp
            ? outsiderID : -1
          : order > t.Down
            ? substractOffset : addOffset;
        neighbourhood.Left = order == t.Left
          ? -1
          : order < t.Right && order > t.Left
            ? substractOffset : addOffset;
        neighbourhood.Right = order == t.Right
          ? -1
          : order > t.Right || order <= t.Left
            ? substractOffset : addOffset;
      }

      if (ModUtility.Config.Rotation == ModConstants.RotationClockwise) {
        (neighbourhood.Left, neighbourhood.Right) = (neighbourhood.Right, neighbourhood.Left);
      }

      return neighbourhood;
    }
  }
}