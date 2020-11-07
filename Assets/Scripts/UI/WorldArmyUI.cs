using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldArmyUI : MonoBehaviour
{
    [SerializeField] ArmyUI armyUI;

    TargetableObject snapTarget;

    public static WorldArmyUI instance;

    WorldArmyUI()
    {
        instance = this;
    }

    public void Open(TargetableObject target)
    {
        snapTarget = target;
        transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        gameObject.SetActive(true);
    }

    public void Close()
    {
        snapTarget = null;
        gameObject.SetActive(false);
    }


    private void Update()
    {
        if (snapTarget)
        {
            transform.position = snapTarget.transform.position + snapTarget.showArmyOffsetUI;
            transform.LookAt(RTSCamera.instance.transform);
            transform.Rotate(new Vector3(0, 180, 0), Space.Self);
            RefreshUI();
        }
    }

    void RefreshUI()
    {
        armyUI.Refresh(snapTarget.GetArmy());
    }
}