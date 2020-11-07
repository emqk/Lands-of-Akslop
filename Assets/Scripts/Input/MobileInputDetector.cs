using UnityEngine;

public class MobileInputDetector : MonoBehaviour
{
    bool touchedDown;

    Vector2 currentPosition;
    Vector2 startPosition;

    float currentHoldTime = 0;
    const float longHoldTime = 0.2f;

    public Vector2 GetMoveVectorInverted()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                startPosition = touch.position;
                currentPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                currentPosition = touch.position;
                Vector2 diff = currentPosition - startPosition;
                Debug.Log("Diff: " + diff);
                startPosition = currentPosition;
                return -diff;
            }
        }

        return new Vector2(0, 0);
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            touchedDown = true;

            currentHoldTime += Input.GetTouch(0).deltaTime;
        }
        else
        {
            touchedDown = false;
            currentHoldTime = 0;
        }
    }

    public bool IsTouchDown()
    {
        return (currentHoldTime < longHoldTime);
    }

    public bool IsTouchAndHoldedDown()
    {
        if (currentHoldTime >= longHoldTime)
            return true;

        return false;
    }
}