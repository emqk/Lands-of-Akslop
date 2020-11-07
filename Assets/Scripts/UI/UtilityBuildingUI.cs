using UnityEngine;

public class UtilityBuildingUI : MonoBehaviour
{
    [SerializeField] ArmyUI cityArmyUI;

    [SerializeField] TMPro.TextMeshProUGUI boostTimeTMP;
    [SerializeField] TMPro.TextMeshProUGUI obtainedItemAmountTMP;
    [SerializeField] UnityEngine.UI.Button boostButton;
    [SerializeField] UnityEngine.UI.Button obtainedItemButton;
    float boostTimePerOneFood = 2;

    Item itemToPayForBoost = new Item(ItemType.Food, 1);

    static UtilityBuilding currUtilityBuilding;
    public static UtilityBuildingUI instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        /*  if (!currUtilityBuilding.GetWorldActorDockedInObject() || currUtilityBuilding.GetCulumatedItemAmount() <= 0)
              obtainedItemButton.interactable = false;
          else
              obtainedItemButton.interactable = true;

          if (currUtilityBuilding)
              RefreshObtainedItemInfo();*/

        RefreshWholePanelUI();
    }

    public void Open(UtilityBuilding utilityBuilding)
    {
        currUtilityBuilding = utilityBuilding;
        gameObject.SetActive(true);
        cityArmyUI.Open(GetCurrentBuildingArmy());

        RefreshWholePanelUI();

        gameObject.SetActive(true);

        UIManager.instance.OnUIPanelOpen();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        cityArmyUI.Close();
        currUtilityBuilding = null;
        AudioManager.instance.ClickButton();
        UIManager.instance.OnUIPanelClose();
    }

    public bool IsSelectedWorldAgentDockedInThisUtilityBuilding()
    {
        if (currUtilityBuilding)
        {
            return currUtilityBuilding.GetWorldActorDockedInObject() == Interaction.instance.SelectedAgent;
        }

        return false;
    }


    public static Army GetCurrentBuildingArmy()
    {
        if (currUtilityBuilding == null)
        {
            Debug.LogError("Can't move unit: currUtilityBuilding == null");
            return null;
        }

        return currUtilityBuilding.Army;
    }

    public ArmyUI GetArmyUI()
    {
        return cityArmyUI;
    }

    void RefreshWholePanelUI()
    {
        RefreshBoostButtonUI();
        RefreshObtainedItemInfoUI();
        RefreshBoostTimeUI();
    }

    void RefreshObtainedItemInfoUI()
    {
        if (currUtilityBuilding)
        {
            obtainedItemAmountTMP.text = "Produces: " + currUtilityBuilding.GetProducedItemAmountPerTime().ToString() + " " + currUtilityBuilding.GetProducedItemType().ToString() 
                                        + " / " + currUtilityBuilding.GetTimeToProduceItem() + "s.";
        }
    }

    void RefreshBoostTimeUI()
    {
        boostTimeTMP.text = "Boost time left: " + currUtilityBuilding.CurrentBoostTimeLeft.ToString("f0") + "s.";
    }

    void RefreshBoostButtonUI()
    {
        if (CanMyCountryPayForBoost())
            boostButton.interactable = true;
        else
            boostButton.interactable = false;
    }

    public void GiveMeFood(int amount = 1)
    {
        if (CanMyCountryPayForBoost())
        {
            currUtilityBuilding.MyCountry.Inventory.PayRequirements(itemToPayForBoost);

            currUtilityBuilding.CurrentBoostTimeLeft += boostTimePerOneFood * amount;
            RefreshBoostTimeUI();
        }
        else
        {
            Debug.LogError("Can not give food to this building, because MyCountry don't have enough food!");
        }
    }

    bool CanMyCountryPayForBoost()
    {
        return currUtilityBuilding.MyCountry.Inventory.HaveEnoughToBuy(itemToPayForBoost);
    }

   /* public void TakeProducedItem()
    {
        WorldAgent worldAgent = currUtilityBuilding.GetWorldActorDockedInObject();

        if (worldAgent)
        {
            Item item = currUtilityBuilding.TakeItem();
            worldAgent.Inventory.AddItem(item);
        }
    }*/
}