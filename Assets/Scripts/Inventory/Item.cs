[System.Serializable]
public class Item
{
    public Item(ItemType _itemType, int _amount)
    {
        itemType = _itemType;
        amount = _amount;
    }

    public readonly ItemType itemType;
    public int amount;

    public static float GetItemWeightByType(ItemType iType)
    {
        switch (iType)
        {
            case ItemType.Gold:
                return 0.2f;
            case ItemType.Wood:
                return 1f;
            case ItemType.Iron:
                return 2f;
            case ItemType.Stone:
                return 1.7f;
            case ItemType.Human:
                return 2f;
            case ItemType.Food:
                return 0.1f;
        }

        return -999999;
    }
}

[System.Serializable]
public enum ItemType
{
    Gold, Wood, Iron, Stone,
    Human, Food
}