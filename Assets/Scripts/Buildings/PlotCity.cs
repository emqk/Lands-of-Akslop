using System.Collections.Generic;
using UnityEngine;

public class PlotCity : TargetableObject
{
    Army army = new Army(999999999);

    public static PlotCity instance;

    [SerializeField] List<UnitPair> startingUnits = new List<UnitPair>();

    private void Start()
    {
        Debug.Log("Setting up default country for PlotCity, because it is Awake()");
        MyCountry = CountryManager.instance.DefaultCountry;

        AddStartArmy();

        instance = this;

        SetWallsActive();
    }
    void AddStartArmy()
    {
        for (int i = 0; i < startingUnits.Count; i++)
        {
            army.AddUnit(new UnitInstance(startingUnits[i].unit, startingUnits[i].amount));
        }

        startingUnits.Clear();
    }

    public override void ChangeRelationsAfterBattle(Country attackerCountry)
    {
        Debug.LogError("Relations can not be changed because this is PlotCity");
    }

    public override Army GetArmy()
    {
        return army;
    }

    void SetWallsActive()
    {
        haveWalls = true;
    }

    public override void OnBattleLost(TargetableObject attacker)
    {
        Debug.Log("PlotCity won. Try again to win");
        Destroy(attacker.gameObject);
        SetWallsActive();
    }

    public override void OnBattleWon(TargetableObject attacker)
    {
        Debug.Log("PlotCity has been conquered!");
        MyCountry = attacker.MyCountry;
        GameStatus.CheckGameStatus();
        SetWallsActive();
    }

    public override void OnThisObjectReached(WorldAgent enteredAgent)
    {
        Debug.LogError("PlotCity entered");
        EnterThisObject(enteredAgent);
    }
}