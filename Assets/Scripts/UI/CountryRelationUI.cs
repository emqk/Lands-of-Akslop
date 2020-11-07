using UnityEngine;
using UnityEngine.UI;

public class CountryRelationUI : MonoBehaviour
{
    [SerializeField] Image goodProgressImg;
    [SerializeField] Image badProgressImg;

    [SerializeField] Button makePeaceButton;
    [SerializeField] Button makeWarButton;

    static Country currentCountry;

    public static CountryRelationUI instance;

    CountryRelationUI()
    {
        instance = this;
    }

    public void Open(Country country)
    {
        currentCountry = country;
        RefreshAll();
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

    void RefreshAll()
    {
        RefreshRelationBar();
        RefreshButtons();
    }

    void RefreshRelationBar()
    {
        Country playerCountry = CountryManager.instance.PlayerCountry;
        CountryRelation countryRelation = CountryManager.instance.GetRelationBetweenCountries(playerCountry, currentCountry);

        float amount = countryRelation.GetAmount();

        //Fill UI realtions bar
        if (amount > 0)
        {
            goodProgressImg.fillAmount = amount / CountryRelation.maxAmount;
            badProgressImg.fillAmount = 0;
        }
        else if (amount < 0)
        {
            goodProgressImg.fillAmount = 0;
            badProgressImg.fillAmount = amount / CountryRelation.minAmount;
        }
        else
        {
            goodProgressImg.fillAmount = 0;
            badProgressImg.fillAmount = 0;
        }
    }
    void RefreshButtons()
    {
        Country playerCountry = CountryManager.instance.PlayerCountry;
        CountryRelation countryRelation = CountryManager.instance.GetRelationBetweenCountries(playerCountry, currentCountry);
        float relationAmount = countryRelation.GetAmount();

        if (relationAmount >= 0)
        {
            makePeaceButton.gameObject.SetActive(false);
            makeWarButton.gameObject.SetActive(true);
        }
        else
        {
            if (relationAmount >= -10)
                makePeaceButton.gameObject.SetActive(true);
            else
                makePeaceButton.gameObject.SetActive(false);

            makeWarButton.gameObject.SetActive(false);
        }
    }

    public void MakePeace()
    {
        Country playerCountry = CountryManager.instance.PlayerCountry;
        CountryRelation countryRelation = CountryManager.instance.GetRelationBetweenCountries(playerCountry, currentCountry);
        countryRelation.MakePeace();

        RefreshAll();
        AudioManager.instance.ClickButton();
    }

    public void MakeWar()
    {
        Country playerCountry = CountryManager.instance.PlayerCountry;
        CountryRelation countryRelation = CountryManager.instance.GetRelationBetweenCountries(playerCountry, currentCountry);
        countryRelation.MakeWar();

        RefreshAll();
        AudioManager.instance.ClickButton();
    }

    public void PayForBetterRelations()
    {
        const int payAmount = 100;
        const int changeRelationAmount = 10;

        Country playerCountry = CountryManager.instance.PlayerCountry;
        Item item = new Item(ItemType.Gold, payAmount);
        if (playerCountry.CanTradeItem(item))
        {
            CountryRelation countryRelation = CountryManager.instance.GetRelationBetweenCountries(playerCountry, currentCountry);
            countryRelation.ChangeAmount(changeRelationAmount);

            playerCountry.Inventory.MoveItemToInventory(item, currentCountry.Inventory);

            RefreshAll();
            AudioManager.instance.ClickButton();
        }
    }
}