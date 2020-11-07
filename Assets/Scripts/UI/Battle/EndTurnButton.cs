using UnityEngine;
using UnityEngine.EventSystems;

public class EndTurnButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool isCursorOverMe = false;

    public void OnClick()
    {
        BattleController.instance.EndTurn();
        AudioManager.instance.ClickButton();
    }

    public bool IsCursorOverMe()
    {
        return isCursorOverMe;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isCursorOverMe = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isCursorOverMe = false;
    }
}
