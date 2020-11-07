[System.Serializable]
public class UnitInstance
{
    public int currentHP;
    public int currentArmor;
    public int currentStamina;

    Unit unit;
    public int amount;

    public UnitInstance(Unit _unit, int _amount)
    {
        unit = _unit;
        amount = _amount;

        ResetStats();
    }

    public Unit GetUnit()
    {
        return unit;
    }

    public void ResetStats()
    {
        currentHP = unit.hp;
        currentArmor = unit.armor;
        currentStamina = unit.stamina;
    }
}