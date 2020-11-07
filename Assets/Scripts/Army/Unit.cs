using UnityEngine;

[CreateAssetMenu(menuName = "Unit", fileName = "NewUnit")]
public class Unit : ScriptableObject
{
    public UnitID UnitID;
    public UnitAttackType unitAttackType;

    public CardActionBase cardAction;

    public string unitName;
    public int weight;
    
    public int hp;
    public int armor;
    public int stamina;
    public int attackRange;
    public Vector2Int damageRange;

    public Sprite icon;
    public BuyRequirement[] buyRequirements;

    public int GetStrength()
    {
        return ((damageRange.x + damageRange.y) / 2) + hp;
    }
}