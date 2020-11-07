using UnityEngine;

public class StartingBattleUI : MonoBehaviour
{
    public static StartingBattleUI instance;

    StartingBattleUI()
    {
        instance = this;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        UIManager.instance.OnUIPanelOpen(false);
    }
    
    public void Close()
    {
        gameObject.SetActive(false);
        UIManager.instance.OnUIPanelClose();
    }
}
