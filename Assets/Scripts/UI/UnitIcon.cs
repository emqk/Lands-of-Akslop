using UnityEngine.EventSystems;
using UnityEngine;

public class UnitIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected Unit unitToRefresh;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UnitStatisticsWindow.instance.Open(unitToRefresh);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CloseInfoWindow();
    }

    protected void CloseInfoWindow()
    {
        UnitStatisticsWindow.instance.Close();
    }

    public void Setup(Unit unit)
    {
        unitToRefresh = unit;
    }
}
