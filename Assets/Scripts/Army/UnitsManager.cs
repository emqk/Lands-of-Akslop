using UnityEngine;

public enum UnitID
{
    Warrior, Archer, Wall, WallCity, ArcherTowerCity
}

public enum UnitAttackType
{
    Melee, Range
}

public class UnitsManager : MonoBehaviour
{
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Unit[] availableUnits;
    [SerializeField] Unit[] availableCityUnits;

    public static UnitsManager instance;

    private void Awake()
    {
        instance = this;
    }

    public Sprite GetDefaultSprite()
    {
        return defaultSprite;
    }

    public Unit[] GetAllAvailableUnits()
    {
        return availableUnits;
    }

    public Unit GetUnitByID(UnitID targetUnitID)
    {
        foreach (Unit unit in availableUnits)
        {
            if (unit.UnitID == targetUnitID)
                return unit;
        }
        return null;
    }
    
    public Unit GetCityUnitByID(UnitID targetUnitID)
    {
        foreach (Unit unit in availableCityUnits)
        {
            if (unit.UnitID == targetUnitID)
                return unit;
        }
        return null;
    }
}