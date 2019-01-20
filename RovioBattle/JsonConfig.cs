using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace BattleEngine
{
  public class JsonConfig : IConfig
  {
    public int Seed { get; set; }
    public int InitialHealth { get; set; }
    public int InitialEnergy { get; set; }
    public IEnumerable<Attack> Attacks { get; set; }
    public double PantsFactor { get; set; }
    public double RingFactor { get; set; }

    public JsonConfig(IConfigurationRoot root)
    {
      // todo Apply extension Intify(...).
      Seed = Convert.ToInt32(root.GetSection("seed").Value ?? "0");
      InitialHealth = Convert.ToInt32(root.GetSection("initialHealth").Value);
      InitialEnergy = Convert.ToInt32(root.GetSection("initialEnergy").Value);
      Attacks = root.GetSection("attacks").GetChildren()
        .Select(_ => new Attack(_))
        .OrderBy(_ => _.Index);
      PantsFactor = Convert.ToDouble(root.GetSection("abilities:pants").Value);
      RingFactor = Convert.ToDouble(root.GetSection("abilities:ring").Value);
    }
  }
}