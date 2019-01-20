using System.Collections.Generic;
using System.Linq;
using BattleEngine;
using Xunit;

namespace Testing
{
  public class UnitTests
  {
    [Fact]
    public void SingleHitCausesSimpleDamage()
    {
      PhonyConfig config = new PhonyConfig(7);
      Game game = new Game(config);

      game.Advance();
      game.Advance();
      State final = game.StateLog().Last();

      Assert.Equal(15, final.AlphaHealth);
      Assert.Equal(15, final.BetaHealth);
    }

    [Fact]
    public void InsufficientEnergyDowngradesAttack()
    {
      PhonyConfig config = new PhonyConfig(7);
      config.Attacks = new List<Attack> { new Attack("weak", 10, 0, 1, 1), new Attack("strong", 11, 10, 1, 99) };
      Game game = new Game(config);

      game.Advance();
      State final = game.StateLog().Last();

      Assert.Equal("weak", final.AttackName);
    }

    [Fact]
    public void PoisonousAttackAffectsMultipleRounds()
    {
      PhonyConfig config = new PhonyConfig(7);
      config.Attacks = new List<Attack> { new Attack("regular", 10, 0, 1, 1), new Attack("poisonous", 11, 0, 0, 99, 1, 5) };
      Game game = new Game(config);

      game.Advance();
      game.Advance();
      State final = game.StateLog().Last();

      Assert.Equal(15, final.BetaHealth);
    }

    [Fact]
    public void GameFinishesWhenHeroDies()
    {
      PhonyConfig config = new PhonyConfig(7);
      config.Attacks = new List<Attack> { new Attack("harsh_AF", 10, 0, 99, 1) };
      Game game = new Game(config);

      int round = 0;
      for (; round < 100 && !game.Finished; round++)
        game.Advance();

      State final = game.StateLog().Last();

      Assert.True(0 > final.BetaHealth);
      Assert.True(game.Finished);
      Assert.True(100 > round);
    }
  }
}
