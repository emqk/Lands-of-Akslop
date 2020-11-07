using UnityEngine;
using TMPro;

public class UnitStatisticsWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI statisticsText;

    Unit unitToShow;
    public static UnitStatisticsWindow instance;

    public UnitStatisticsWindow()
    {
        instance = this;
    }

    public void Open(Unit unit)
    {
        if (unit)
        {
            unitToShow = unit;
            Refresh();

            gameObject.SetActive(true);
        }
    }

    public void Close()
    {
        unitToShow = null;
        gameObject.SetActive(false);
    }

    void Refresh()
    {
        if (unitToShow)
        {
            statisticsText.text = unitToShow.unitName + "\n\n"
            + "HP: " + unitToShow.hp + "\n"
            + "Stamina: " + unitToShow.stamina + "\n"
            + "Damage: " + unitToShow.damageRange.x + " - " + unitToShow.damageRange.y + "\n"
            + "Armor: " + unitToShow.armor + "\n"
            + "Weight: " + unitToShow.weight;
        }
    }
}
