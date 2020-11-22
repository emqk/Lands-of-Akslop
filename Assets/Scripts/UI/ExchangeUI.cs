using UnityEngine;
using TMPro;

public class ExchangeUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI enteredAgentText;
    [SerializeField] TextMeshProUGUI dockAgentText;
    [SerializeField] ArmyUI enteredArmyUI;
    [SerializeField] ArmyUI dockedArmyUI;

    WorldAgent enteredWorldAgent;
    WorldAgent dockWorldAgent;

    bool isOpened = false;

    public WorldAgent GetEnteredAgent()
    {
        return enteredWorldAgent;
    }
    public WorldAgent GetDockAgent()
    {
        return dockWorldAgent;
    }

    public bool IsOpened()
    {
        return isOpened;
    }

    public void Refresh()
    {
        enteredAgentText.text = "Entered position: " + enteredWorldAgent.transform.position;
        dockAgentText.text = "Dock position: " + dockWorldAgent.transform.position;
        enteredArmyUI.Refresh(enteredWorldAgent.GetArmy());
        dockedArmyUI.Refresh(dockWorldAgent.GetArmy());
    }

    public void Open(WorldAgent enteredAgent, WorldAgent dockAgent)
    {
        if (enteredAgent == dockAgent)
        {
            Debug.Log("Can not open Exchange UI, because both objects are the same");
            return;
        }
        UIManager.instance.WorldAgentPanel.Close();

        enteredWorldAgent = enteredAgent;
        dockWorldAgent = dockAgent;


        Refresh();

        if (!isOpened)
        {
            gameObject.SetActive(true);
            UIManager.instance.OnUIPanelOpen();
        }

        isOpened = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        AudioManager.instance.ClickButton();
        UIManager.instance.OnUIPanelClose();

        enteredWorldAgent = null;
        dockWorldAgent = null;

        isOpened = false;
    }
}