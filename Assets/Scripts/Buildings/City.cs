using UnityEngine;
using System.Collections.Generic;

public class City : Building
{
    [SerializeField] List<GameObject> wallObjs;
    [SerializeField] List<ProduceItem> produceItem = new List<ProduceItem>();


    private void Awake()
    {
        foreach (ProduceItem item in produceItem)
        {
            item.Setup();
        }
    }

    private void Start()
    {
        SetDefaultCountryIfThereIsNoCurrently();
        AddStartArmy();

        SetWallsActive(false);
    }

    public override void OnThisObjectReached(WorldAgent agent)
    {
        EnterThisObject(agent);
        OnThisObjectEntrancePosition(agent);
    }
    public void OnThisObjectEntrancePosition(WorldAgent agent)
    {
        if (agent.MyCountry == MyCountry)
        {
            Debug.Log("City Entered!");
        }
    }

    private void Update()
    {
        foreach (ProduceItem item in produceItem)
        {
            item.UpdateMe(CountryManager.instance.IsItDefaultCountry(MyCountry) == false);
            if (item.GetCumulatedItemsAmount() > 0)
            {
                Item takenItem = item.TakeItem();
                MyCountry.AddItem(takenItem);
            }
        }
    }

    public override void OnSelect()
    {
        Debug.Log("City selected");
        UIManager.instance.CityPanel.Open(this);
    }

    public override void OnUnselect()
    {
        Debug.Log("City unselected");
        //UIManager.instance.CityPanel.Close();
    }

    public bool CanBuyWalls()
    {
        Unit unit = UnitsManager.instance.GetCityUnitByID(UnitID.WallCity);
        BuyRequirement[] buyRequirements = unit.buyRequirements;
        const int amount = 1;

        return MyCountry.Inventory.HaveEnoughToBuy(buyRequirements, amount);
    }

    void PayForWalls()
    {
        Unit cityWallUnit = UnitsManager.instance.GetCityUnitByID(UnitID.WallCity);
        BuyRequirement[] buyRequirements = cityWallUnit.buyRequirements;
        const int amount = 1;

        MyCountry.Inventory.PayRequirements(buyRequirements, amount);
    }

    public void BuildWalls()
    {
        haveWalls = true;
        PayForWalls();
        SetWallsActive(true);
    }

    public void DestroyWalls()
    {
        haveWalls = false;
        SetWallsActive(false);
    }

    void SetWallsActive(bool value)
    {
        foreach (GameObject wall in wallObjs)
        {
            wall.SetActive(value);
        }
    }

    public bool SpawnWorldAgentInThisCity()
    {
        if (worldAgentDock == null)
        {
            WorldAgent worldAgent = MyCountry.SpawnWorldAgent(entrance.position, MyCountry.isPlayerCountry);
            OnThisObjectReached(worldAgent);
            return true;
        }

        return false;
    }

    public override void ChangeRelationsAfterBattle(Country attackerCountry)
    {
        if (!attackerCountry.isDefaultCountry)
        {
            Debug.Log("Changing relations because City has been conquered");
            CountryRelation countryRelation = CountryManager.instance.GetRelationBetweenCountries(MyCountry, attackerCountry);
            countryRelation.ChangeAmount(-20);
        }
    }
}