using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArmyUI : MonoBehaviour
{
    [SerializeField] RectTransform unitsParent;

    public bool isItCity;

    static Army currWorldAgentArmy;

    public static Army GetCurrentWorldAgentArmy()
    {
        return currWorldAgentArmy;
    }

    private void Awake()
    {
        SetupUnitsUIPanel();
    }

    void SetupUnitsUIPanel()
    {
        int unitsCount = unitsParent.childCount;
        for (int i = 0; i < unitsCount; i++)
        {
            unitsParent.GetChild(i).GetComponent<UnitFieldUI>().Setup(this);
        }
    }


    public void Open(Army army)
    {
        if (isItCity == false)
        {
            Debug.Log("Settign world agent army to: " + army);

            currWorldAgentArmy = army;
        }

        gameObject.SetActive(true);
        Refresh(army);
    }

    public void Close()
    {
        gameObject.SetActive(false);

        if (isItCity == false)
        {
            currWorldAgentArmy = null;
        }
    }

    public void Refresh(Army army)
    {
        List<UnitInstance> unitInstances = new List<UnitInstance>(army.GetUnits());
        int unitsCount = unitInstances.Count;
        for (int i = 0; i < unitsParent.childCount; i++)
        {
            if (i < unitsCount)
            {
                unitsParent.GetChild(i).GetComponent<UnitFieldUI>().Refresh(unitInstances[i]);
            }
            else
            {
                unitsParent.GetChild(i).GetComponent<UnitFieldUI>().Refresh(null);
            }
        }
    }
}