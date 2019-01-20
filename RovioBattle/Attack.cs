using System;
using Microsoft.Extensions.Configuration;

namespace BattleEngine
{
  public class Attack
  {
    public string Name { get; set; }
    public int Index { get; set; }
    public int Cost { get; set; }
    public int Damage { get; set; }
    public int Frequency { get; set; }
    public int Threshold { get; set; }
    public int Duration { get; set; }
    public int Effect { get; set; }

    public Attack(IConfigurationSection section)
    {
      Name = section.Key;
      Index = Convert.ToInt32(section.GetSection("index").Value);
      Cost = Convert.ToInt32(section.GetSection("cost").Value);
      Damage = Convert.ToInt32(section.GetSection("damage").Value);
      Frequency = Convert.ToInt32(section.GetSection("frequency").Value);
      Duration = Convert.ToInt32(section.GetSection("duration").Value ?? "0");
      Effect = Convert.ToInt32(section.GetSection("effect").Value ?? "0");
    }

    // todo Call contructors in chain reference.
    public Attack(string name, int index, int cost, int damage, int frequency, int duration = 0, int effect = 0)
    {
      Name = name;
      Index = index;
      Cost = cost;
      Damage = damage;
      Frequency = frequency;
      Duration = duration;
      Effect = effect;
    }
  }
}