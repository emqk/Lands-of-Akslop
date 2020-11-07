using UnityEngine;

public class WorldEventUIManager : MonoBehaviour
{
    public static WorldEventUIManager instance;

    WorldEventUIManager()
    {
        instance = this;
    }

    public void ShowEvent(WorldEventUIPanel eventObj, WorldEvent worldEvent)
    {
        eventObj.Open(worldEvent);
    }
}