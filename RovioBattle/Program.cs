using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace BattleEngine
{
  class Program
  {
    public static void Main(string[] args)
    {
      IConfigurationRoot config = WireUpConfig();

      Game game = new Game(new JsonConfig(config));

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
        .AddJsonFile(file, false, false)
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
}