using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] MobileInputDetector mobileInputDetector;

    public static InputManager instance;

    private void Awake()
    {
        instance = this;
    }

    public Vector2 GetMoveVector()
    {
        #if UNITY_ANDROID 
                return mobileInputDetector.GetMoveVectorInverted() / 10f;
        #else
                return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));        
        #endif
    }

    public bool IsLeftPressedDown()
    {
        #if UNITY_ANDROID
                return mobileInputDetector.IsTouchDown();
        #else
                return Input.GetButtonDown("Fire1");        
        #endif
    }

    public bool IsRightPressedDown()
    {
        #if UNITY_ANDROID
                return mobileInputDetector.IsTouchAndHoldedDown();
        #else
                return Input.GetButtonDown("Fire2");        
        #endif
    }

    public Vector2 GetCursorPosition()
    {
        #if UNITY_ANDROID
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;
        else
            return new Vector2(-9999999, -9999999);
        #else
                return Input.mousePosition;        
        #endif
    }
}
