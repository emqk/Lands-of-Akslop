using UnityEngine;

public class VisualInfoManager : MonoBehaviour
{
    public VisualInfoPanel visualInfoPanelPrefab;

    public static VisualInfoManager instance;

    VisualInfoManager()
    {
        instance = this;
    }

    public void CreateVisualInfo(string text, Vector3 position, float destroyTime, Canvas targetCanvas)
    {
        VisualInfoPanel visualInfoPanel = Instantiate(visualInfoPanelPrefab);
        visualInfoPanel.transform.SetParent(targetCanvas.transform);
        visualInfoPanel.transform.position = position + new Vector3(0, 2, 0);
        visualInfoPanel.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        visualInfoPanel.Setup(text);

        Destroy(visualInfoPanel.gameObject, destroyTime);
    }
}