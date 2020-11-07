using UnityEngine;
using TMPro;

public class CountryInfoUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countryNameText;

    static Country currentCountry;

    public void Open(Country country)
    {
        currentCountry = country;
        Refresh();
        gameObject.SetActive(true);
        UIManager.instance.OnUIPanelOpen();
    }

    public void Close()
    {
        currentCountry = null;
        gameObject.SetActive(false);
        AudioManager.instance.ClickButton();
        UIManager.instance.OnUIPanelClose();
    }

    public void OpenCountryRelationPanel(CountryRelationUI panel)
    {
        panel.Open(currentCountry);
        AudioManager.instance.ClickButton();
    }

    public void OpenTradePanel(TradeUI panel)
    {
        panel.Open(currentCountry);
        AudioManager.instance.ClickButton();
    }

    void Refresh()
    {
        countryNameText.text = currentCountry.GetCountryName();
    }
}