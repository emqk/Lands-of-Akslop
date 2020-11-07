using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionNotificationUI : MonoBehaviour
{
    [SerializeField] Image colorImage;
    [SerializeField] TextMeshProUGUI contentText;
    GameObject targetObj = null;

    float timeLeft;

    public void Refresh(ActionManager.ActionData actionData, float lifeTime)
    {
        colorImage.color = actionData.color;
        contentText.text = actionData.content;
        targetObj = actionData.actionObj;

        timeLeft = lifeTime;
    }

    public void OnClick()
    {
        if (targetObj)
        {
            RTSCamera.instance.LookAtTransform(targetObj.transform.position);
        }
    }

    private void Update()
    {
        if (timeLeft <= 0)
        {
            Destroy(gameObject);
        }

        timeLeft -= Time.unscaledDeltaTime;
    }
}
