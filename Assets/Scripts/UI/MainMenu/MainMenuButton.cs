using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    float HoverScaleMultiplier = 1.03f;
    float changeScaleSpeed = 20;
    Vector2 defaultScale;
    bool isCursorOnMe = false;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        isCursorOnMe = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isCursorOnMe = false;
    }

    void Awake() 
    {
        defaultScale = transform.localScale;
    }

    void Update()
    {
        if (isCursorOnMe)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, defaultScale * HoverScaleMultiplier, changeScaleSpeed * Time.deltaTime);
        }
        else
        {
            transform.localScale = Vector2.Lerp(transform.localScale, defaultScale, changeScaleSpeed * Time.deltaTime);
        }
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
