using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProduceItem
{
    [SerializeField] ItemType producedItemType;
    [SerializeField] int producedItemAmount;
    [SerializeField] float timeToProductItem;
    float defaultTimeToProduceItem;

    readonly float boostTimeMultiplier = 0.7f;

    int cumulatedItems;
    [SerializeField] int maxCumulatedItems = 10;
    bool isProducing = false;


    public void Setup()
    {
        defaultTimeToProduceItem = timeToProductItem;
    }

    public ItemType GetProducedItemType()
    {
        return producedItemType;
    }

    public int GetCumulatedItemsAmount()
    {
        return cumulatedItems;
    }

    public int GetProducedItemsAmountPerTime()
    {
        return producedItemAmount;
    }

    public float GetTimeToProduceItem()
    {
        return timeToProductItem;
    }

    public int GetMaxCumulatedAmount()
    {
        return maxCumulatedItems;
    }

    public void BoostMe()
    {
        timeToProductItem = defaultTimeToProduceItem * boostTimeMultiplier;
    }

    public void UnBoostMe()
    {
        timeToProductItem = defaultTimeToProduceItem;
    }

    public Item TakeItem()
    {
        Item item = new Item(GetProducedItemType(), GetCumulatedItemsAmount());
        cumulatedItems = 0;

        return item;
    }

    public void UpdateMe(bool produceIf)
    {
        if (isProducing == false)
        {
            if (produceIf)
            {
                isProducing = true;
                TimerManager.instance.AddTimer(new Timer(Produce, timeToProductItem));
            }
        }
    }

    public void Produce()
    {
        cumulatedItems = Mathf.Clamp(cumulatedItems + producedItemAmount, 0, maxCumulatedItems);
        isProducing = false;
    }
}