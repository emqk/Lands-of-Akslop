using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] Texture2D defaultCursor;
    [SerializeField] Texture2D interactionCursor;

    public enum CursorType
    {
        defaultCursor, interactionCursor
    }

    public static CursorManager instance;

    CursorManager()
    {
        instance = this;
    }

    void Awake()
    {
        SetCursorTexture(CursorType.defaultCursor);
    }

    public void SetCursorTexture(CursorType cursorType)
    {
        switch (cursorType)
        {
            case CursorType.defaultCursor:
                SetCursorTexture(defaultCursor);
                break;
            case CursorType.interactionCursor:
                SetCursorTexture(interactionCursor);
                break;
            default:
                break;
        }
    }
    void SetCursorTexture(Texture2D texture)
    {
        Cursor.SetCursor(texture, new Vector2(0, 0), CursorMode.Auto);
    }
}
