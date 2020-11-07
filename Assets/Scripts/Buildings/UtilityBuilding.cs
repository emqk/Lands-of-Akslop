using System.Collections.Generic;
using UnityEngine;

public class UtilityBuilding : Building
{
    [SerializeField] ProduceItem produceItem;
    float currentBoostTimeLeft;

    public float CurrentBoostTimeLeft { get => currentBoostTimeLeft; set => currentBoostTimeLeft = value; }

    private void Awake()
    {
        produceItem.Setup();
    }

    private void Start()
    {
        SetDefaultCountryIfThereIsNoCurrently();
        AddStartArmy();
    }

    public override void OnThisObjectReached(WorldAgent agent)
    {
        EnterThisObject(agent);
    }

    public override void OnSelect()
    {
        Debug.Log("UtilityBuilding selected");
        UIManager.instance.UtilityBuildingPanel.Open(this);
    }

    public override void OnUnselect()
    {
        Debug.Log("UtilityBuilding unselected");
        //UIManager.instance.CityPanel.Close();
    }

    public override void ChangeRelationsAfterBattle(Country attackerCountry)
    {
        if (!attackerCountry.isDefaultCountry)
        {
            Debug.Log("Changing relations because WorldAgent has been conquered");
            CountryRelation countryRelation = CountryManager.instance.GetRelationBetweenCountries(MyCountry, attackerCountry);
            countryRelation.ChangeAmount(-10);
        }
    }

    private void Update()
    {
        produceItem.UpdateMe(CountryManager.instance.IsItDefaultCountry(MyCountry) == false);

        if (produceItem.GetCumulatedItemsAmount() > 0)
        {
            Item takenItem = produceItem.TakeItem();
            MyCountry.AddItem(takenItem);
        }

        ControlBoost();
    }
    void ControlBoost()
    {
        if (CurrentBoostTimeLeft > 0)
        {
            CurrentBoostTimeLeft -= Time.deltaTime;
            produceItem.BoostMe();
        }
        else
        {
            CurrentBoostTimeLeft = 0;
            produceItem.UnBoostMe();
        }
    }

    public ItemType GetProducedItemType()
    {
        return produceItem.GetProducedItemType();
    }

    public int GetProducedItemAmountPerTime()
    {
        return produceItem.GetProducedItemsAmountPerTime();
    }

    public float GetTimeToProduceItem()
    {
        return produceItem.GetTimeToProduceItem();
    }

    public int GetMaxCumulatedItemAmount()
    {
        return produceItem.GetMaxCumulatedAmount();
    }

    public Item TakeItem()
    {
        return produceItem.TakeItem();
    }

}