using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UnitFieldUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] Image iconImage;

    [SerializeField] bool showUnitInfoOnHover = true;

    UnitInstance unitToRefresh;
    ArmyUI myArmyUI;
    int index = -1;


    public void OnPointerEnter(PointerEventData eventData)
    {
        if(showUnitInfoOnHover)
            UnitStatisticsWindow.instance.Open(unitToRefresh.GetUnit());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CloseInfoWindow();
    }

    void CloseInfoWindow()
    {
        if(showUnitInfoOnHover)
            UnitStatisticsWindow.instance.Close();
    }

    public void Setup(ArmyUI armyUI)
    {
        myArmyUI = armyUI;
        index = transform.GetSiblingIndex();
    }

    public void Refresh(UnitInstance unitInstance)
    {
        if (unitInstance != null)
        {
            unitToRefresh = unitInstance;
            iconImage.sprite = unitInstance.GetUnit().icon;
            amountText.text = unitInstance.GetUnit().name + " x " + unitInstance.amount.ToString("f0");
        }
        else
        {
            unitToRefresh = null;
            iconImage.sprite = UnitsManager.instance.GetDefaultSprite();
            amountText.text = "-";
        }
    }

    public void MoveUnitToSecondArmy()
    {
        Army worldAgentArmy = ArmyUI.GetCurrentWorldAgentArmy();
        Army currBuildingArmy = null;

        if (UIManager.instance.ExchangePanel.IsOpened())
        {
            worldAgentArmy = UIManager.instance.ExchangePanel.GetEnteredAgent().GetArmy();
            currBuildingArmy = UIManager.instance.ExchangePanel.GetDockAgent().GetArmy();
        }
        else if (CityUI.GetCurrentBuildingArmy() != null)
        {
            if (CityUI.instance.IsSelectedWorldAgentDockedInThisCity())
            {
                currBuildingArmy = CityUI.GetCurrentBuildingArmy();
            }
        }
        else if (UtilityBuildingUI.GetCurrentBuildingArmy() != null)
        {
            if (UtilityBuildingUI.instance.IsSelectedWorldAgentDockedInThisUtilityBuilding())
            {
                currBuildingArmy = UtilityBuildingUI.GetCurrentBuildingArmy();
            }
        }     

        if (worldAgentArmy != null && currBuildingArmy != null)
        {
            CloseInfoWindow();
          
            if (myArmyUI.isItCity)
            {
                currBuildingArmy.MoveUnitToOtherArmy(index, worldAgentArmy);
            }
            else
            {
                worldAgentArmy.MoveUnitToOtherArmy(index, currBuildingArmy);
            }

            if (UIManager.instance.ExchangePanel.IsOpened())
            {
                UIManager.instance.ExchangePanel.Refresh();
            }
        }

        AudioManager.instance.ClickButton();
    }
}
