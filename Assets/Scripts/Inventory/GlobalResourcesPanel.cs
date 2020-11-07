using UnityEngine;
using TMPro;

public class GlobalResourcesPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI woodText;
    [SerializeField] TextMeshProUGUI stoneText;
    [SerializeField] TextMeshProUGUI ironText;
    [SerializeField] TextMeshProUGUI peopleText;
    [SerializeField] TextMeshProUGUI foodText;

    Country playerCountry = null;

    private void Start()
    {
        playerCountry = CountryManager.instance.PlayerCountry;
    }

    public void RefreshPlayerCountryUI()
    {
        if (playerCountry != null)
            RefreshUI(playerCountry.Inventory);
        else
            Debug.LogError("PlayerCountry == null!");
    }

    void RefreshUI(Inventory inventory)
    {
        goldText.text = "Gold: " + inventory.GetItemAmount(ItemType.Gold).ToString();
        woodText.text = "Wood: " + inventory.GetItemAmount(ItemType.Wood).ToString();
        stoneText.text = "Stone: " + inventory.GetItemAmount(ItemType.Stone).ToString();
        ironText.text = "Iron: " + inventory.GetItemAmount(ItemType.Iron).ToString();
        peopleText.text = "People: " + inventory.GetItemAmount(ItemType.Human).ToString();
        foodText.text = "Food: " + inventory.GetItemAmount(ItemType.Food).ToString();
    }
}