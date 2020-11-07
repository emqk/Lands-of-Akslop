using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldEventUIPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Transform buttonsParent;

    bool isOpened = false;
    WorldEvent currentWorldEvent;

    public static WorldEventUIPanel instance;

    WorldEventUIPanel()
    {
        instance = this;
    }

    public void Open(WorldEvent worldEvent)
    {
        currentWorldEvent = worldEvent;
        Refresh(worldEvent.GetWorldEventData());
        SetButtonActivity();

        isOpened = true;
        gameObject.SetActive(true);
        UIManager.instance.OnUIPanelOpen();
    }

    public void Close()
    {
        isOpened = false;
        gameObject.SetActive(false);
        UIManager.instance.OnUIPanelClose();
    }

    void SetButtonActivity()
    {
        for (int i = 0; i < currentWorldEvent.items.Length; i++)
        {
            SetActiveButton(i, currentWorldEvent.GetEnteredWorldAgent().MyCountry.Inventory.WillEveryItemWillBePositiveOrZeroAfterAdding(currentWorldEvent.items[i].Get()));
        }
    }
    void SetActiveButton(int index, bool value)
    {
        buttonsParent.GetChild(index).GetComponent<Button>().interactable = value;
    }

    public void Refresh(WorldEvent.WorldEventData worldEventData)
    {
        descriptionText.text = worldEventData.description;

        for (int i = 0; i < buttonsParent.childCount; i++)
        {
            if (i < worldEventData.buttonsData.Length)
            {
                buttonsParent.GetChild(i).gameObject.SetActive(true);
                buttonsParent.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = worldEventData.buttonsData[i].buttonsText;
            }
            else
            {
                buttonsParent.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (isOpened)
        {
            SetButtonActivity();
        }
    }

    public void ChooseOption(int optionIndex)
    {
        currentWorldEvent.Option(optionIndex);
        Close();
    }
}