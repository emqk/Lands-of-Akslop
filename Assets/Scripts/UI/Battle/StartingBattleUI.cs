using UnityEngine;

public class StartingBattleUI : MonoBehaviour
{
    public static StartingBattleUI instance;

    StartingBattleUI()
    {
        instance = this;
    }

    public bool IsEnabled()
    {
        return gameObject.activeSelf;
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
