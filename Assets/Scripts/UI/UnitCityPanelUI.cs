using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitCityPanelUI : MonoBehaviour
{
    [SerializeField] Image unitIcon;
    [SerializeField] TextMeshProUGUI unitNameText;
    [SerializeField] TextMeshProUGUI currAmountText;
    [SerializeField] TextMeshProUGUI oneUnitCostText;
    [SerializeField] TextMeshProUGUI summarizedCost;
    [SerializeField] Button buyButton;
    int amount = 1;

    Unit unit;
    Army army;
    ArmyUI armyUI;

    private void OnEnable()
    {
        RefreshCurrAmountAmountAndCost(currAmountText, summarizedCost);
    }

    private void Update()
    {
        RefreshButtonInteractability();
    }
    void RefreshButtonInteractability()
    {
        if (CountryManager.instance.PlayerCountry.Inventory.HaveEnoughToBuy(unit.buyRequirements, amount))
            buyButton.interactable = true;
        else
            buyButton.interactable = false;
    }

    public void Refresh(Unit unitToRefresh, Army targetArmy, ArmyUI targetArmyUI)
    {
        unit = unitToRefresh;
        army = targetArmy;
        armyUI = targetArmyUI;

        //UI
        unitIcon.sprite = unitToRefresh.icon;
        unitNameText.text = unit.unitName;
        oneUnitCostText.text = "One unit cost: " + BuyRequirement.GetAsString(unit.buyRequirements);

        unitIcon.GetComponent<UnitIcon>().Setup(unit);
    }

    public void RefreshAmountFromSlider(Slider slider)
    {
        amount = Mathf.RoundToInt(slider.value);
        RefreshCurrAmountAmountAndCost(currAmountText, summarizedCost);
    }
    void RefreshCurrAmountAmountAndCost(TextMeshProUGUI amountText, TextMeshProUGUI costText)
    {
        amountText.text = amount.ToString("f0");
        costText.text = "Total cost: " + BuyRequirement.GetAsString(unit.buyRequirements, amount);
    }

    public void Buy()
    {
        army.AddUnit(new UnitInstance(unit, amount));
        armyUI.Refresh(army);
        CountryManager.instance.PlayerCountry.Inventory.PayRequirements(unit.buyRequirements, amount);

        AudioManager.instance.ClickButton();
    }
}
