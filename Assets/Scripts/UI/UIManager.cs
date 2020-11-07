using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject blocker;

    [SerializeField] WorldAgentUI worldAgentPanel;
    public WorldAgentUI WorldAgentPanel
    {
        get
        {
            return worldAgentPanel;
        }
    }

    [SerializeField] CityUI cityPanel;
    public CityUI CityPanel
    {
    
    get
        {
            return cityPanel;
        }
    }

    [SerializeField] UtilityBuildingUI utilityBuildingPanel;
    public UtilityBuildingUI UtilityBuildingPanel
    {
        get
        {
            return utilityBuildingPanel;
        }
    }

    [SerializeField] GlobalResourcesPanel globalResourcesPanel;
    public GlobalResourcesPanel GlobalResourcesPanel
    {
        get
        {
            return globalResourcesPanel;
        }
    }

    [SerializeField] ExchangeUI exchangePanel;
    public ExchangeUI ExchangePanel
    {
        get
        {
            return exchangePanel;
        }
    }

    [SerializeField] CountryInfoUI countryInfoPanel;
    public CountryInfoUI CountryInfoPanel
    {
        get
        {
            return countryInfoPanel;
        }
    }


    int openedUIPanels = 0;

    public static UIManager instance;

    UIManager()
    {
        instance = this;
    }

    public void OnUIPanelOpen(bool openWithBackground = true)
    {
        TimeManager.SetPauseTime();
        
        if(!(TacticalMode.IsEnabled() && openedUIPanels == 0) && openWithBackground)
            blocker.SetActive(true);

        openedUIPanels ++;
        Debug.Log("UI opened: " + openedUIPanels);
    }
    public void OnUIPanelClose()
    {
        openedUIPanels --;

        if (openedUIPanels <= 0)
        {
            TimeManager.SetDefaultTime();
            openedUIPanels = 0;
            blocker.SetActive(false);
        }

        if (openedUIPanels == 1 && TacticalMode.IsEnabled())
        {
            blocker.SetActive(false);
        }

        Debug.Log("UI closed: " + openedUIPanels);
    }

    public static bool IsMouseOverUI()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;

        // return EventSystem.current.IsPointerOverGameObject();
    }

    public void RefreshCurrentlySelectedWorldAgentArmyUI()
    {
        if(Interaction.instance.SelectedAgent)
            WorldAgentPanel.GetArmyUI().Refresh(Interaction.instance.SelectedAgent.GetArmy());
    }
}