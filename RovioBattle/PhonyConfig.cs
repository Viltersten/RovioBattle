using System.Collections.Generic;

namespace BattleEngine
{
  public class PhonyConfig : IConfig
  {
    public int Seed { get; set; }
    public int InitialHealth { get; set; }
    public int InitialEnergy { get; set; }
    public IEnumerable<Attack> Attacks { get; set; }
    public double PantsFactor { get; set; }
    public double RingFactor { get; set; }

    public PhonyConfig(int seed)
    {
      Seed = seed;
      InitialHealth = 20;
      InitialEnergy = 0;
      Attacks = new List<Attack> { new Attack("test1", 10, 0, 5, 4) };
      PantsFactor = .75;
      RingFactor = 2;
    }
  }
}