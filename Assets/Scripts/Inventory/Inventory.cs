[System.Serializable]
public class Inventory
{
    Item[] items = {
        new Item(ItemType.Gold, 0),
        new Item(ItemType.Wood, 0),
        new Item(ItemType.Iron, 0),
        new Item(ItemType.Stone, 0),
        new Item(ItemType.Human, 0),
        new Item(ItemType.Food, 0),
    };

    float maxWeight;
    float currentWeight;

    public Inventory(int _maxWeight)
    {
        maxWeight = _maxWeight;
    }

    Item GetItem(ItemType itemType)
    {
        foreach (Item currItem in items)
        {
            if (currItem.itemType == itemType)
            {
                return currItem;
            }
        }

        return null;
    }

    public int GetItemAmount(ItemType itemType)
    {
        return GetItem(itemType).amount;
    }

    public void MoveItemToInventory(Item item, Inventory otherInventory)
    {
        otherInventory.AddItem(item);
        DecreaseItem(item);
    }

    public void AddItem(Item item)
    {
        Item inventoryItem = GetItem(item.itemType);
        inventoryItem.amount += item.amount;

        RefreshAll();
    }
    public void AddItem(BuyRequirement[] buyRequirement)
    {
        foreach (BuyRequirement item in buyRequirement)
        {
            Item inventoryItem = GetItem(item.itemType);
            inventoryItem.amount += item.amount;
        }

        RefreshAll();
    }

    Item RemoveItem(int index)
    {
        Item removedItem = new Item(items[index].itemType, items[index].amount);
        items[index].amount = 0;

        RefreshAll();

        return removedItem;
    }

    public void DecreaseItem(ItemType itemType, int amount)
    {
        int indexToDecrease = GetItemIndex(itemType);
        items[indexToDecrease].amount -= amount;

        RefreshAll();
    }

    public void DecreaseItem(Item item)
    {
        Item inventoryItem = GetItem(item.itemType);
        inventoryItem.amount -= item.amount;

        RefreshAll();
    }

    public bool WillEveryItemWillBePositiveOrZeroAfterAdding(BuyRequirement[] buyRequirement)
    {
        foreach (BuyRequirement item in buyRequirement)
        {
            Item inventoryItem = GetItem(item.itemType);
            if (inventoryItem.amount + item.amount < 0)
                return false;
        }

        return true;
    }

    int GetItemIndex(ItemType itemType)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].itemType == itemType)
                return i;
        }

        return -1;
    }

    public bool HaveEnoughToBuy(BuyRequirement[] buyRequirements, int amount)
    {
        foreach (BuyRequirement currentRequirement in buyRequirements)
        {
            if (GetItemAmount(currentRequirement.itemType) < currentRequirement.amount * amount)
                return false;
        }

        return true;
    }
    
    public bool HaveEnoughToBuy(Item item)
    {
        UnityEngine.Debug.LogError("GetItemAmount(item.itemType): " + GetItemAmount(item.itemType) + " item.amount: " + item.amount);
        if (GetItemAmount(item.itemType) < item.amount)
            return false;

        return true;
    }

    public void PayRequirements(BuyRequirement[] buyRequirements, int amount)
    {
        foreach (BuyRequirement currentRequirement in buyRequirements)
        {
            DecreaseItem(currentRequirement.itemType, currentRequirement.amount * amount);
        }
    }

    public void PayRequirements(Item item)
    {
        DecreaseItem(item.itemType, item.amount);
    }

    public void MoveAllItemsToOtherInventory(Inventory otherInventory)
    {
        for (int i = 0; i < items.Length; i++)
        {
            Item item = RemoveItem(i);
            otherInventory.AddItem(item);
        }
    }


    void RefreshAll()
    {
        RefreshCurrentWeight();
        RefreshPlayerCountryInventory();
    }

    void RefreshCurrentWeight()
    {
        float result = 0;

        foreach (Item currItem in items)
        {
            result += Item.GetItemWeightByType(currItem.itemType) * currItem.amount;
        }

        currentWeight = result;
    }

    void RefreshPlayerCountryInventory()
    {
        UIManager.instance.GlobalResourcesPanel.RefreshPlayerCountryUI();
    }
}

