using System.Collections.Generic;
using System.Linq;

namespace BattleEngine
{
  public class Hero
  {
    public string Name { get; set; }
    public int Health { get; set; }
    public int Energy { get; set; }
    public bool HasPants { get; set; }
    public bool HasRing { get; set; }
    public List<Attack> Subsequents { get; set; }

    public Hero(string name, int health, int energy, bool pants, bool ring)
    {
      Name = name;
      Health = health;
      Energy = energy;
      HasPants = pants;
      HasRing = ring;
      Subsequents = new List<Attack>();
    }

    public void Attack(Attack attack)
    {
      ExecuteSubsequents();
      Energy -= attack.Cost;
    }

    public void Resist(Attack attack, bool alteration)
    {
      ExecuteSubsequents();
      Health -= attack.Damage;
      Subsequents.AddRange(Enumerable.Repeat(attack, attack.Duration));

      if (attack.Index == 0 && alteration)
        Health += attack.Damage;
      if (attack.Index == 1 && alteration)
        Health -= attack.Damage;
    }

    private void ExecuteSubsequents()
    {
      if (Subsequents.Any())
      {
        Attack subsequent = Subsequents.First();
        Subsequents.Remove(subsequent);

        Health -= subsequent.Effect;
      }
    }
  }
}