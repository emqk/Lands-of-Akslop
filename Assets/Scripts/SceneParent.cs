using UnityEngine;

public class SceneParent : MonoBehaviour
{
    static Transform myTransform;

    private void Awake()
    {
        myTransform = transform;
    }

    public static void ParentGameObjectToMe(Transform obj)
    {
        obj.SetParent(myTransform);
    }
}