using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject mainPanel;
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] PostProcessVolume ppVolume;
    [SerializeField] PostProcessProfile ppProfile;
    [SerializeField] AudioMixer audioMixer;

    static bool isEnabled = false;

    private void Start()
    {
        SetDropDown();
    }

    public static bool IsEnabled()
    {
        return isEnabled;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleActive();
        }
    }

    public void ToggleActive()
    {
        AudioManager.instance.ClickButton();
        if (mainPanel.activeSelf)
            Close();
        else
            Open();
    }

    void Open()
    {
        mainPanel.SetActive(true);
        isEnabled = true;
        UIManager.instance.OnUIPanelOpen();
    }

    void Close()
    {
        mainPanel.SetActive(false);
        isEnabled = false;

        UIManager.instance.OnUIPanelClose();
    }

    void SetDropDown()
    {
        string[] names = QualitySettings.names;
        List<TMP_Dropdown.OptionData> datas = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < names.Length; i++)
        {
            datas.Add(new TMP_Dropdown.OptionData(names[i]));
        }
        qualityDropdown.AddOptions(datas);

        qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
    }

    public void OnValueChange() 
    {
        ChangeQualitySettings();
        AudioManager.instance.ClickButton();
    }

    void ChangeQualitySettings()
    {
        int currentVSyncCount = QualitySettings.vSyncCount;
        QualitySettings.SetQualityLevel(qualityDropdown.value);
        QualitySettings.vSyncCount = currentVSyncCount;
    }


    public void SetVSync(Toggle toggle)
    {
        if(toggle.isOn)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
    }

    public void SetSSAO(Toggle toggle)
    {
        ppProfile.GetSetting<AmbientOcclusion>().active = toggle.isOn;
        ChangePPEffect();
    }

    void ChangePPEffect()
    {
        ppVolume.profile = ppProfile;
    }

    public void SetAudioMasterVoume(Slider slider)
    {
        float amount = slider.value;
        if (amount == slider.minValue)
            amount = -100;

        audioMixer.SetFloat("MasterVolume", amount);
    }

    public void SetAudioMusicVoume(Slider slider)
    {
        float amount = slider.value;
        if (amount == slider.minValue)
            amount = -100;

        audioMixer.SetFloat("MusicVolume", amount);
    }

    public void SetAudioSoundsVoume(Slider slider)
    {
        float amount = slider.value;
        if (amount == slider.minValue)
            amount = -100;

        audioMixer.SetFloat("SoundsVolume", amount);
    }

    public void QuitGame()
    {
        AudioManager.instance.ClickButton();
        Application.Quit();
    }
}