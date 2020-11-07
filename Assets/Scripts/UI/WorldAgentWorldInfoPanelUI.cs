using UnityEngine;
using TMPro;

public class WorldAgentWorldInfoPanelUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI remainingDistanceText;
    [SerializeField] TextMeshProUGUI remainingTimeText;
    WorldAgent snapTarget;

    Vector3 offset = new Vector3(0, 4.5f, 0);

    public static WorldAgentWorldInfoPanelUI instance;

    WorldAgentWorldInfoPanelUI()
    {
        instance = this;
    }

    public void Open(WorldAgent target)
    {
        gameObject.SetActive(true);
        snapTarget = target;
        transform.localScale = new Vector3(0.01f, 0.01f , 0.01f);
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
            transform.position = snapTarget.transform.position + offset;
            transform.LookAt(RTSCamera.instance.transform);
            transform.Rotate(new Vector3(0, 180, 0), Space.Self);
            RefreshUI();
        }
        else
        {
            Debug.LogError("I don't have target!");
        }
    }

    void RefreshUI()
    {
        float pathDistance = NavMeshUtils.GetAgentCurrentDestinationPathDistance(snapTarget.GetNavMeshAgent());
        float estimatedTime = pathDistance / snapTarget.GetMaxSpeed();

        if (pathDistance < 0)
        {
            pathDistance = 0;
            estimatedTime = 0;
        }
        remainingDistanceText.text = "Distance left: " + pathDistance.ToString("f1");
        remainingTimeText.text = "Estimated time left: " + estimatedTime.ToString("f1") + "s";
    }
}
