using System.Collections.Generic;

[System.Serializable]
public class Army
{
    int maxWeight;
    int currentWeight;
    List<UnitInstance> units = new List<UnitInstance>();

    public Army(int _maxWeight)
    {
        maxWeight = _maxWeight;
    }

    public bool IsThisArmyStrongerThan(Army otherArmy)
    {
        return GetArmyStrength() > otherArmy.GetArmyStrength();
    }

    public int GetArmyStrength()
    {
        int sum = 0;
        foreach (UnitInstance unit in units)
        {
            sum += unit.amount * unit.GetUnit().GetStrength();
        }

        return sum;
    }

    void RefreshCurrentWeight()
    {
        int result = 0;

        foreach (UnitInstance currUnityInstance in units)
        {
            result += currUnityInstance.GetUnit().weight * currUnityInstance.amount;
        }

        currentWeight = result;
    }

    public void ResetUnits()
    {
        UnityEngine.Debug.Log("Reset all units in army");
        foreach (UnitInstance unitInstance in units)
        {
            unitInstance.ResetStats();
        }
    }

    public void RemoveAllUnits()
    {
        units.Clear();
        RefreshCurrentWeight();
    }

    public void ChangeUnitsAmountByPercentage(float value)
    {
        foreach (UnitInstance unit in units)
        {
            unit.amount = UnityEngine.Mathf.RoundToInt(unit.amount * value);
        }
    }

    public List<UnitInstance> GetUnits()
    {
        return units;
    }
    void RemoveUnitAtIndex(int index)
    {
        units.RemoveAt(index);
        RefreshCurrentWeight();
    }
    void RemoveUnitAtIndex(int index, int amount)
    {
        units[index].amount -= amount;

        if (units[index].amount == 0)
            RemoveUnitAtIndex(index);

        RefreshCurrentWeight();
    }

    // Remove all units which has amount <= 0
    public void RemoveDestroyedUnits()
    {
        for (int i = units.Count - 1; i >= 0; i--)
        {
            if(units[i].amount <= 0)
            RemoveUnitAtIndex(i);
        }
    }

    public void MoveAllUnitsToOtherArmy(Army otherArmy)
    {
        for (int i = 0; i < units.Count; i++)
        {
            MoveUnitToOtherArmy(i, otherArmy);
        }
    }

    public void MoveUnitToOtherArmy(int unitIndex, Army targetArmy)
    {
        if ((units.Count-1) < unitIndex)
        {
            UnityEngine.Debug.LogError("Unit which you want to move to other army is null! count: " + (units.Count-1) + " index: " + unitIndex);
            return;
        }

        targetArmy.AddUnit(units[unitIndex]);
        RemoveUnitAtIndex(unitIndex);

        if(ArmyUI.GetCurrentWorldAgentArmy() != null)
            UIManager.instance.WorldAgentPanel.GetArmyUI().Refresh(ArmyUI.GetCurrentWorldAgentArmy());

        if (CityUI.GetCurrentBuildingArmy() != null)
            CityUI.instance.GetArmyUI().Refresh(CityUI.GetCurrentBuildingArmy());
        else if (UtilityBuildingUI.GetCurrentBuildingArmy() != null)
            UtilityBuildingUI.instance.GetArmyUI().Refresh(UtilityBuildingUI.GetCurrentBuildingArmy());
    }

    public void MoveUnitToOtherArmy(int unitIndex, Army targetArmy, int amount)
    {
        if ((units.Count - 1) < unitIndex)
        {
            UnityEngine.Debug.LogError("Unit which you want to move to other army is null! count: " + (units.Count - 1) + " index: " + unitIndex);
            return;
        }

        if (amount > units[unitIndex].amount)
        {
            UnityEngine.Debug.LogError("There aren't enough units to move!");
            return;
        }

        targetArmy.AddUnit(new UnitInstance(units[unitIndex].GetUnit(), amount));
        RemoveUnitAtIndex(unitIndex, amount);

        if (ArmyUI.GetCurrentWorldAgentArmy() != null)
            UIManager.instance.WorldAgentPanel.GetArmyUI().Refresh(ArmyUI.GetCurrentWorldAgentArmy());

        if (CityUI.GetCurrentBuildingArmy() != null)
            CityUI.instance.GetArmyUI().Refresh(CityUI.GetCurrentBuildingArmy());
        else if (UtilityBuildingUI.GetCurrentBuildingArmy() != null)
            UtilityBuildingUI.instance.GetArmyUI().Refresh(UtilityBuildingUI.GetCurrentBuildingArmy());
    }

    public void AddUnit(UnitInstance unitInstance)
    {
        if (StackToExistingUnit(unitInstance) == false)
        {
            AddNewUnitToList(unitInstance);
        }

        RefreshCurrentWeight();
    }
    bool StackToExistingUnit(UnitInstance unitInstance)
    {
        int unitIndex = FindUnitByID(unitInstance.GetUnit().UnitID);
        if (unitIndex >= 0)
        {
            units[unitIndex].amount += unitInstance.amount;
            return true;
        }

        return false;
    }
    void AddNewUnitToList(UnitInstance unitInstance)
    {
        units.Add(unitInstance);
    }

    int FindUnitByID(UnitID unitID)
    {
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].GetUnit().UnitID == unitID)
                return i;
        }
        return -1;
    }
}