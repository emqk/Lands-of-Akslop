using UnityEngine;
using TMPro;

public class TradeUI : MonoBehaviour
{
    [Header("For gold")]
    [SerializeField] TMP_InputField fgWoodAmount;
    [SerializeField] TMP_InputField fgStoneAmount;
    [SerializeField] TMP_InputField fgIronAmount;
    [SerializeField] TMP_InputField fgPeopleAmount;
    [SerializeField] TMP_InputField fgFoodAmount;

    [SerializeField] TMP_InputField askForGoldAmount;

    [Header("Pay gold")]
    [SerializeField] TMP_InputField payGoldAmount;

    [SerializeField] TMP_InputField pgWoodAmount;
    [SerializeField] TMP_InputField pgStoneAmount;
    [SerializeField] TMP_InputField pgIronAmount;
    [SerializeField] TMP_InputField pgPeopleAmount;
    [SerializeField] TMP_InputField pgFoodAmount;

    static Country otherCountry;

    public void Open(Country _otherCountry)
    {
        otherCountry = _otherCountry;
        gameObject.SetActive(true);
        UIManager.instance.OnUIPanelOpen();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        AudioManager.instance.ClickButton();
        UIManager.instance.OnUIPanelClose();
    }

    //Prevent InputText from being empty
    public void OnChange(TMP_InputField inputField)
    {
        if (inputField.text == "")
            inputField.text = "0";
        else if (inputField.text[0] == '-')
            inputField.text = "0";
    }

    public void AskTradeForGold()
    {
        Item woodToPay = new Item(ItemType.Wood, int.Parse(fgWoodAmount.text));
        Item stoneToPay = new Item(ItemType.Stone, int.Parse(fgStoneAmount.text));
        Item ironToPay = new Item(ItemType.Iron, int.Parse(fgIronAmount.text));
        Item peopleToPay = new Item(ItemType.Human, int.Parse(fgPeopleAmount.text));
        Item foodToPay = new Item(ItemType.Food, int.Parse(fgFoodAmount.text));

        Item goldToGet = new Item(ItemType.Gold, int.Parse(askForGoldAmount.text));

        Country playerCountry = CountryManager.instance.PlayerCountry;
        if (playerCountry.CanTradeItem(woodToPay) &&
            playerCountry.CanTradeItem( stoneToPay) &&
            playerCountry.CanTradeItem(ironToPay) &&
            playerCountry.CanTradeItem(peopleToPay) &&
            playerCountry.CanTradeItem(foodToPay) &&
            otherCountry.CanTradeItem(goldToGet))
        {
            Debug.Log("Want to pay " + woodToPay.amount + " to get: " + goldToGet.amount);

            otherCountry.Inventory.MoveItemToInventory(goldToGet, playerCountry.Inventory);
            playerCountry.Inventory.MoveItemToInventory(woodToPay, otherCountry.Inventory);
            playerCountry.Inventory.MoveItemToInventory(stoneToPay, otherCountry.Inventory);
            playerCountry.Inventory.MoveItemToInventory(ironToPay, otherCountry.Inventory);
            playerCountry.Inventory.MoveItemToInventory(peopleToPay, otherCountry.Inventory);
            playerCountry.Inventory.MoveItemToInventory(foodToPay, otherCountry.Inventory);
        }
        else
        {
            Debug.Log("Can not trade items!");
        }
    }
    
    public void AskTradePayGold()
    {
        Item woodToGet = new Item(ItemType.Wood, int.Parse(pgWoodAmount.text));
        Item stoneToGet = new Item(ItemType.Stone, int.Parse(pgStoneAmount.text));
        Item ironToGet = new Item(ItemType.Iron, int.Parse(pgIronAmount.text));
        Item peopleToGet = new Item(ItemType.Human, int.Parse(pgPeopleAmount.text));
        Item foodToGet = new Item(ItemType.Food, int.Parse(pgFoodAmount.text));

        Item goldToPay = new Item(ItemType.Gold, int.Parse(payGoldAmount.text));

        Country playerCountry = CountryManager.instance.PlayerCountry;
        if (otherCountry.CanTradeItem(woodToGet) &&
            otherCountry.CanTradeItem(stoneToGet) &&
            otherCountry.CanTradeItem(ironToGet) &&
            otherCountry.CanTradeItem(peopleToGet) &&
            otherCountry.CanTradeItem(foodToGet) &&
            playerCountry.CanTradeItem(goldToPay))
        {
            Debug.Log("Want to pay " + woodToGet.amount + " to get: " + goldToPay.amount);

            playerCountry.Inventory.MoveItemToInventory(goldToPay, otherCountry.Inventory);
            otherCountry.Inventory.MoveItemToInventory(woodToGet, playerCountry.Inventory);
            otherCountry.Inventory.MoveItemToInventory(stoneToGet, playerCountry.Inventory);
            otherCountry.Inventory.MoveItemToInventory(ironToGet, playerCountry.Inventory);
            otherCountry.Inventory.MoveItemToInventory(peopleToGet, playerCountry.Inventory);
            otherCountry.Inventory.MoveItemToInventory(foodToGet, playerCountry.Inventory);
        }
        else
        {
            Debug.Log("Can not trade items!");
        }
    }
}