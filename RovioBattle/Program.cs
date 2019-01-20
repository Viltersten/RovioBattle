using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;


namespace RovioBattle
{
  class Program
  {
    public static void Main(string[] args)
    {
      IConfigurationRoot config = WireUpConfig();

      Game game = new Game(new Config(config));

      while (!game.Finished)
        game.Advance();

      Console.WriteLine("\n=====================");
      Console.WriteLine("Press any key to stop or any other key to finish   :)");
      Console.ReadLine();
    }

    private static IConfigurationRoot WireUpConfig(string file = "config.json")
    {
      IConfigurationRoot config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(file, false, true)
        .Build();

      EnsureCoherency(config);

      return config;
    }

    private static void EnsureCoherency(IConfigurationRoot config)
    {
      List<string> whines = new List<string>();

      List<IConfigurationSection> attacks = config.GetSection("attacks").GetChildren().ToList();
      if (!attacks.Any()) whines.Add("Attacks node empty.");
      foreach (IConfigurationSection attack in attacks)
      {
        if (attack.GetSection("index").Value == null) whines.Add(attack.Key + " missing index.");
        if (attack.GetSection("name").Value == null) whines.Add(attack.Key + " missing name.");
        if (attack.GetSection("cost").Value == null) whines.Add(attack.Key + " missing cost.");
        if (attack.GetSection("damage").Value == null) whines.Add(attack.Key + " missing damage.");
        if (attack.GetSection("frequency").Value == null) whines.Add(attack.Key + " missing frequency.");
      }

      if (config.GetSection("abilities:pants").Value == null) whines.Add("Pants missing value.");
      if (config.GetSection("abilities:ring").Value == null) whines.Add("Ring missing value.");

      if (config.GetSection("initialHealth").Value == null) whines.Add("Initial health missing value.");
      if (config.GetSection("initialEnergy").Value == null) whines.Add("Initial energy missing value.");

      if (whines.Any())
        throw new Exception("Invalid config!\n" + string.Join("\n", whines.ToArray()));
    }
  }

  public class Config
  {
    public int InitialHealth { get; set; }
    public int InitialEnergy { get; set; }
    public IEnumerable<Attack> Attacks { get; set; }
    public double PantsFactor { get; set; }
    public double RingFactor { get; set; }

    public Config(IConfigurationRoot root)
    {
      InitialHealth = Convert.ToInt32(root.GetSection("initialHealth").Value ?? "100");
      InitialEnergy = Convert.ToInt32(root.GetSection("initialEnergy").Value ?? "100");
      Attacks = root.GetSection("attacks").GetChildren()
        .Select(_ => new Attack(_))
        .OrderBy(_ => _.Index);
      PantsFactor = Convert.ToDouble(root.GetSection("abilities:pants").Value);
      RingFactor = Convert.ToDouble(root.GetSection("abilities:ring").Value);
    }
  }

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
      Index = Convert.ToInt32(section.GetSection("index").Value ?? "0");
      Cost = Convert.ToInt32(section.GetSection("cost").Value ?? "0");
      Damage = Convert.ToInt32(section.GetSection("damage").Value ?? "0");
      Frequency = Convert.ToInt32(section.GetSection("frequency").Value ?? "0");
      Duration = Convert.ToInt32(section.GetSection("duration").Value ?? "0");
      Effect = Convert.ToInt32(section.GetSection("effect").Value ?? "0");
    }
  }

  public class Game
  {
    private readonly Config Config;
    private readonly Random Randomizer;
    private readonly List<Attack> History;
    private readonly Hero Alpha;
    private readonly Hero Beta;

    public int TotalFrequency { get; set; }
    public List<Attack> Attacks { get; set; }
    public Hero Aggressor { get; set; }
    public Hero Defender { get; set; }
    public bool Finished => Defender.Health < 1 || Aggressor.Health < 1;

    public Game(Config config)
    {
      Config = config;
      Randomizer = new Random();
      History = new List<Attack>();

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
        SwapOffensive();
    }

    public void Advance()
    {
      // todo Implement energy gain configuration instead of magic number.
      Alpha.Energy += 2;
      Beta.Energy += 2;

      SwapOffensive();
      Attack attack = DetermineAttack();
      History.Add(attack);

      bool alteration = attack.Index == 1 && Aggressor.HasRing
        || attack.Index == 0 && Defender.HasPants && Randomizer.NextDouble() < Config.PantsFactor;
      Aggressor.Attack(attack);
      Defender.Resist(attack, alteration);

      NarrateStatus(alteration);
    }

    private void SwapOffensive()
    {
      Hero temp = Defender;
      Defender = Aggressor;
      Aggressor = temp;
    }

    private Attack DetermineAttack()
    {
      int attackType = Randomizer.Next(TotalFrequency);
      Attack attack = Attacks.First(_ => _.Threshold >= attackType);

      return Aggressor.Energy > attack.Cost ? attack : Attacks.First();
    }

    private void NarrateStatus(bool alteration)
    {
      Console.WriteLine("\n---------------------");
      Console.WriteLine($"Round #{History.Count}");
      Console.WriteLine($"{Aggressor.Name} attacks with {History.Last().Name}.");
      if (alteration)
        DescribeAlteration();
      DescribeSubsequents(Alpha);
      DescribeSubsequents(Beta);
      Console.WriteLine($"{Alpha.Name} has {DescribeHealth(Alpha.Health)} and {Beta.Name} has {DescribeHealth(Beta.Health)}.");
    }

    private void DescribeAlteration()
    {
      switch (History.Last().Index)
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

  // todo Split classes into separate files.
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

      if (attack.Index == 0 && alteration)
        Health += attack.Damage;
      if (attack.Index == 1 && alteration)
        Health -= attack.Damage;
      if (attack.Index == 3)
        Subsequents.AddRange(Enumerable.Repeat(attack, attack.Duration));
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

  public static class Extensions
  {
    public static bool NextBool(this Random self, double likelihood = .5)
    {
      return self.NextDouble() < likelihood;
    }
  }
}