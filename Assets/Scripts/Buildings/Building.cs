using System.Collections.Generic;
using UnityEngine;

public class Building : TargetableObject
{
    [SerializeField] protected GameObject flag;

    [SerializeField] List<UnitPair> startingUnits = new List<UnitPair>();

    protected Army army = new Army(100);
    public Army Army
    {
        get
        {
            return army;
        }
    }


    protected void AddStartArmy()
    {
        for (int i = 0; i < startingUnits.Count; i++)
        {
            army.AddUnit(new UnitInstance(startingUnits[i].unit, startingUnits[i].amount));
        }

        startingUnits.Clear();
    }

    protected void SetDefaultCountryIfThereIsNoCurrently()
    {
        if (MyCountry == null)
        {
            Debug.Log("Setting up default country for me, because I don't have any");
            MyCountry = CountryManager.instance.DefaultCountry;
        }
    }

    public virtual void OnSelect()
    {
        Debug.Log("OnSelect() Base");
    }

    public virtual void OnUnselect()
    {
        Debug.Log("OnUnselect() Base");
    }

    public override Army GetArmy()
    {
        return army;
    }


    public void SetMaterialBaseColorToCountryColor(Country country)
    {
        flag.GetComponent<Renderer>().material.SetColor("_Color", country.GetCountryColor());
    }


    public override void OnThisObjectReached(WorldAgent agent)
    {
        EnterThisObject(agent);
    }

    public override void OnBattleWon(TargetableObject attacker)
    {
        if(MyCountry.isPlayerCountry)
            ActionManager.instance.CreateAction(ActionManager.ActionInformationContent.PlayerBuildingCaptured, gameObject);
        
        ChangeRelationsAfterBattle(attacker.MyCountry);
        attacker.MyCountry.CaptureNewBuilding(this);
        OnBattleWonBase();

        //Move random unit from attacker's army after conquering this Building
        if (!attacker.MyCountry.isPlayerCountry)
        {
            MoveOneRandomUnitFromAttackerArmy(attacker);
        }
    }
    void MoveOneRandomUnitFromAttackerArmy(TargetableObject attacker)
    {
        Army attackerArmy = attacker.GetArmy();
        int armyUnitsCount = attackerArmy.GetUnits().Count;

        if (attackerArmy.GetUnits().Count > 0)
        {
            int randomUnitFromAttackerArmy = Random.Range(0, armyUnitsCount);
            attackerArmy.MoveUnitToOtherArmy(randomUnitFromAttackerArmy, GetArmy(), 1);
        }
    }

    public override void OnBattleLost(TargetableObject attacker)
    {
        ChangeRelationsAfterBattle(attacker.MyCountry);
        Destroy(attacker.gameObject);
    }

    public override void ChangeRelationsAfterBattle(Country attackerCountry)
    {
        Debug.LogError("OnConquered() not implemented!");
        throw new System.NotImplementedException();
    }
}