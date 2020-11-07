using UnityEngine;
using System.Collections.Generic;

public class WorldItem : TargetableObject
{
    [System.Serializable]
    public class ItemsWrapper
    {
        public BuyRequirement[] itemsReq;

        public BuyRequirement[] Get()
        {
            return itemsReq;
        }
    }

    [SerializeField] public ItemsWrapper[] items;
    [SerializeField] List<UnitPair> startingUnits = new List<UnitPair>();

    protected Army army = new Army(100);

    static List<WorldItem> allWorldItems = new List<WorldItem>();


    public override void ChangeRelationsAfterBattle(Country attackerCountry)
    {
        throw new System.NotImplementedException();
    }

    public override Army GetArmy()
    {
        return army;
    }

    public override void OnBattleLost(TargetableObject attacker)
    {
        Destroy(attacker.gameObject);
    }

    public override void OnBattleWon(TargetableObject attacker)
    {
        attacker.MyCountry.Inventory.AddItem(items[0].Get());
        DestroyMe();
    }

    public override void OnThisObjectReached(WorldAgent enteredAgent)
    {
        EnterThisObject(enteredAgent);
    }

    public static List<WorldItem> GetAllWorldItems()
    {
        return allWorldItems;
    }

    private void Awake()
    {
        allWorldItems.Add(this);
    }

    private void Start()
    {
        MyCountry = CountryManager.instance.DefaultCountry;
        AddStartArmy();
    }
    protected void AddStartArmy()
    {
        for (int i = 0; i < startingUnits.Count; i++)
        {
            army.AddUnit(new UnitInstance(startingUnits[i].unit, startingUnits[i].amount));
        }

        startingUnits.Clear();
    }

    protected void DestroyMe()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        allWorldItems.Remove(this);
    }
}