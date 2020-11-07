using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUtils : MonoBehaviour
{
    [SerializeField] GameObject baseSceneSettingsParent;

    public static SceneUtils instance;

    private void Awake()
    {
        instance = this;
    }

    public void EnterBattleScene()
    {
        baseSceneSettingsParent.SetActive(false);
        AudioManager.instance.PlayMusic(MusicType.battleMusic);
    }

    public void ExitBattleScene()
    {
        baseSceneSettingsParent.SetActive(true);
        AudioManager.instance.PlayMusic(MusicType.worldMusic);
    }

    public void LoadBattleSceneAdditive()
    {
        SceneManager.LoadSceneAsync("BattleSceneForest", LoadSceneMode.Additive);
    }

    public void UnloadBattleScene()
    {
        SceneManager.UnloadSceneAsync("BattleSceneForest");
    }
}
