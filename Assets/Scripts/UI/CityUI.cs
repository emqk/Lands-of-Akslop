using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CityUI : MonoBehaviour
{
    [SerializeField] ArmyUI cityArmyUI;
    [SerializeField] RectTransform unitsToBuyParent;
    [SerializeField] Button buyWorldAgentButton;
    [SerializeField] TextMeshProUGUI worldAgentBuyRequirementsText;
    [SerializeField] TextMeshProUGUI cityWallBuyRequirementsText;
    [SerializeField] Button buyCityWallsButton;

    static City currCity;
    public static CityUI instance;

    private void Awake()
    {
        instance = this;
    }

    public bool IsSelectedWorldAgentDockedInThisCity()
    {
        if (currCity)
        {
            return currCity.GetWorldActorDockedInObject() == Interaction.instance.SelectedAgent;
        }

        return false;
    }

    public static Army GetCurrentBuildingArmy()
    {
        if (currCity == null)
        {
            Debug.LogError("Can't move unit: currCity == null");
            return null;
        }

        return currCity.Army;
    }
    
    public ArmyUI GetArmyUI()
    {
        return cityArmyUI;
    }

    public void Open(City city)
    {
        currCity = city;

        RefreshUnitsAvailableToBuy();
        worldAgentBuyRequirementsText.text = BuyRequirement.GetAsString(AgentManager.instance.GetWorldAgentBuyRequirements());
        cityWallBuyRequirementsText.text = BuyRequirement.GetAsString(UnitsManager.instance.GetCityUnitByID(UnitID.WallCity).buyRequirements);

        gameObject.SetActive(true);
        cityArmyUI.Open(GetCurrentBuildingArmy());
        UIManager.instance.OnUIPanelOpen();
    }
    public void Close()
    {
        gameObject.SetActive(false);
        cityArmyUI.Close();
        currCity = null;
        AudioManager.instance.ClickButton();
        UIManager.instance.OnUIPanelClose();
    }

    private void Update()
    {
        RefreshWorldAgentBuyButton();
        RefreshCityWallBuyButton();
    }


    void RefreshUnitsAvailableToBuy()
    {
        Unit[] units = UnitsManager.instance.GetAllAvailableUnits();

        int count = units.Length;
        for (int i = 0; i < count; i++)
        {
            unitsToBuyParent.GetChild(i).GetComponent<UnitCityPanelUI>().Refresh(units[i], GetCurrentBuildingArmy(), cityArmyUI);
        }
    }

    public void RefreshWorldAgentBuyButton()
    {
        if (currCity.GetWorldActorDockedInObject())
        {
            buyWorldAgentButton.interactable = false;
            return;
        }

        BuyRequirement[] buyRequirements = AgentManager.instance.GetWorldAgentBuyRequirements();
        const int amount = 1;
        if (currCity.MyCountry.IsTherePlaceForNextWorldAgent() && currCity.MyCountry.Inventory.HaveEnoughToBuy(buyRequirements, amount))
        {
            buyWorldAgentButton.interactable = true;
        }
        else
        {
            buyWorldAgentButton.interactable = false;
        }
    }

    public void BuyWorldAgent()
    {
        BuyRequirement[] buyRequirements = AgentManager.instance.GetWorldAgentBuyRequirements();
        const int amount = 1;
        if (currCity.MyCountry.Inventory.HaveEnoughToBuy(buyRequirements, amount))
        {
            currCity.SpawnWorldAgentInThisCity();
            currCity.MyCountry.Inventory.PayRequirements(buyRequirements, amount);
            AudioManager.instance.ClickButton();
        }
    }


    public void RefreshCityWallBuyButton()
    {
        if (currCity.HaveWalls == false && currCity.CanBuyWalls())
        {
            buyCityWallsButton.interactable = true;
        }
        else
        {
            buyCityWallsButton.interactable = false;
        }
    }

    public void BuyCityWall()
    {
        if (currCity.CanBuyWalls())
        {
            currCity.BuildWalls();
            AudioManager.instance.ClickButton();
        }
    }
}