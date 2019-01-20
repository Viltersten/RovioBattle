namespace BattleEngine
{
  public class State
  {
    public string AttackName { get; set; }
    public int AttackIndex { get; set; }
    public int AlphaEnergy { get; set; }
    public int AlphaHealth { get; set; }
    public int BetaEnergy { get; set; }
    public int BetaHealth { get; set; }

    public State(Attack attack, Hero alpha, Hero beta)
    {
      AttackName = attack.Name;
      AttackIndex = attack.Index;
      AlphaEnergy = alpha.Energy;
      AlphaHealth = alpha.Health;
      BetaEnergy = beta.Energy;
      BetaHealth = beta.Health;
    }
  }
}