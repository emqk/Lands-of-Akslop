using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAgentUI : MonoBehaviour
{
    [SerializeField] ArmyUI armyUI;

    public void Open(Army army)
    {
        armyUI.Open(army);
        gameObject.SetActive(true);
    }

    public void Close()
    {
        armyUI.Close();
        gameObject.SetActive(false);
        AudioManager.instance.ClickButton();
    }

    public ArmyUI GetArmyUI()
    {
        return armyUI;
    }
}