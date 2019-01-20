using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleEngine
{
  public class Game
  {
    private readonly IConfig Config;
    private readonly Random Randomizer;
    private readonly List<State> History;
    private readonly Hero Alpha;
    private readonly Hero Beta;

    public int TotalFrequency { get; set; }
    public List<Attack> Attacks { get; set; }
    public Hero Aggressor { get; set; }
    public Hero Defender { get; set; }
    public bool Finished => Defender.Health < 1 || Aggressor.Health < 1;

    public Game(IConfig config)
    {
      Config = config;

      Randomizer = new Random();
      if (Config.Seed > 0)
        Randomizer = new Random(Config.Seed);

      History = new List<State>();

      // todo Implement configurable likelihood of abilities to appear;
      Alpha = new Hero("Chamster", Config.InitialHealth, Config.InitialEnergy, Randomizer.NextBool(), Randomizer.NextBool());
      Beta = new Hero("Korre", Config.InitialHealth, Config.InitialEnergy, Randomizer.NextBool(), Randomizer.NextBool());

      Attacks = new List<Attack>();
      foreach (Attack attack in config.Attacks)
      {
        // todo Consider introducing a new second type for Attack class for proper Threshold and Frequency.
        attack.Threshold = Attacks.Sum(_ => _.Frequency);
        Attacks.Add(attack);
      }

      TotalFrequency = Attacks.Sum(_ => _.Frequency);

      Aggressor = Alpha;
      Defender = Beta;
      if (Randomizer.Next(2) == 0)
        SwapInitiative();
    }

    public void Advance()
    {
      // todo Implement energy gain configuration instead of magic number.
      Alpha.Energy += 2;
      Beta.Energy += 2;

      SwapInitiative();
      Attack attack = DetermineAttack();

      bool alteration = attack.Index == 1 && Aggressor.HasRing
        || attack.Index == 0 && Defender.HasPants && Randomizer.NextDouble() < Config.PantsFactor;
      Aggressor.Attack(attack);
      Defender.Resist(attack, alteration);

      History.Add(new State(attack, Alpha, Beta));
      NarrateStatus(alteration);
    }

    public List<State> StateLog()
    {
      return History;
    }

    private void SwapInitiative()
    {
      Hero temp = Defender;
      Defender = Aggressor;
      Aggressor = temp;
    }

    private Attack DetermineAttack()
    {
      int attackType = Randomizer.Next(TotalFrequency);
      Attack attack = Attacks.Last(_ => _.Threshold <= attackType);

      return Aggressor.Energy > attack.Cost ? attack : Attacks.First();
    }

    private void NarrateStatus(bool alteration)
    {
      Console.WriteLine("\n---------------------");
      Console.WriteLine($"Round #{History.Count}");
      Console.WriteLine($"{Aggressor.Name} attacks with {History.Last().AttackName}.");
      if (alteration)
        DescribeAlteration();
      DescribeSubsequents(Alpha);
      DescribeSubsequents(Beta);
      Console.WriteLine($"{Alpha.Name} has {DescribeHealth(Alpha.Health)} and {Beta.Name} has {DescribeHealth(Beta.Health)}.");
    }

    private void DescribeAlteration()
    {
      switch (History.Last().AttackIndex)
      {
        case 0:
          Console.WriteLine("The pants got suspiciously brown but caused evasion.");
          break;
        case 1:
          Console.WriteLine("The ring heated up the balls increasing the damage.");
          break;
      }
    }

    private static void DescribeSubsequents(Hero hero)
    {
      if (hero.Subsequents.Any())
        Console.WriteLine($"{hero.Name} has additional {hero.Subsequents.Count} subsequent damages to sustain.");
    }

    private static string DescribeHealth(int health)
    {
      return health > 0 ? health + " health left" : "died";
    }
  }
}