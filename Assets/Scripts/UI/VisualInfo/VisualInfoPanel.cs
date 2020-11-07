using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VisualInfoPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textUI;

    public void Setup(string text)
    {
        RefreshText(text);
        LookAtMe();
    }

    void RefreshText(string text)
    {
        textUI.text = text;
    }

    private void Update()
    {
        LookAtMe();
        transform.position += transform.up * 1 * Time.deltaTime;
    }

    void LookAtMe()
    {
        transform.LookAt(transform.position * 2 - BattleController.instance.GetBattleCamera().transform.position);
    }
}