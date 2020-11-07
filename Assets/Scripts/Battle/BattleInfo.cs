public class BattleInfo
{
    TargetableObject attacker;
    TargetableObject defender;

    public TargetableObject GetAttacker()
    {
        return attacker;
    }

    public TargetableObject GetDefender()
    {
        return defender;
    }

    public void Setup(TargetableObject _attacker, TargetableObject _defender)
    {
        attacker = _attacker;
        defender = _defender;
    }

    public void Clear()
    {
        attacker = null;
        defender = null;
    }
}