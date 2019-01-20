using System.Collections.Generic;

namespace BattleEngine
{
  public interface IConfig
  {
    int Seed { get; set; }
    int InitialHealth { get; set; }
    int InitialEnergy { get; set; }
    IEnumerable<Attack> Attacks { get; set; }
    double PantsFactor { get; set; }
    double RingFactor { get; set; }
  }
}