using System;

namespace BattleEngine
{
  public static class Extensions
  {
    public static int Intfy(this string self, int fallBack = 0)
    {
      return Convert.ToInt32(self ?? "" + fallBack);
    }

    public static bool NextBool(this Random self, double likelihood = .5)
    {
      return self.NextDouble() < likelihood;
    }
  }
}