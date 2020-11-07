using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TacticalMode : MonoBehaviour
{
    static bool isEnabled = false;
    [SerializeField] PostProcessVolume volume;
    Vignette chromaticAberrationLayer = null;


    public static TacticalMode instance;
    
    TacticalMode()
    {
        instance = this;
    }

    public static bool IsEnabled()
    {
        return isEnabled;
    }

    private void Start()
    {
        volume.profile.TryGetSettings(out chromaticAberrationLayer);
        chromaticAberrationLayer.intensity.value = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && BattleManager.isDuringBattle == false)
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (isEnabled && !PauseMenu.IsEnabled())
        {
            Disable();
            AudioManager.instance.ClickButton();
        }
        else if(!PauseMenu.IsEnabled())
        {
            AudioManager.instance.ClickButton();
            Enable();
        }
    }

    void Enable()
    {
        isEnabled = true;
        UIManager.instance.OnUIPanelOpen();

        volume.profile.TryGetSettings(out chromaticAberrationLayer);
        chromaticAberrationLayer.intensity.value = 0.5f;
    }

    public void Disable()
    {
        isEnabled = false;
        UIManager.instance.OnUIPanelClose();

        volume.profile.TryGetSettings(out chromaticAberrationLayer);
        chromaticAberrationLayer.intensity.value = 0;
    }
}
