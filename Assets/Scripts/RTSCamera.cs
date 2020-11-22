using UnityEngine;

public class RTSCamera : MonoBehaviour
{

    [SerializeField] float movementSpeed = 30;
    [Tooltip("Go to target Y speed")]
    [SerializeField] float scrollSpeed = 5;
    [Tooltip("Scroll input value multiplier")]
    [SerializeField] float scrollIntensity = 25;

    [SerializeField] float minHeight;
    [SerializeField] float maxHeight = 65;

    Vector3 targetPos;
    float targetY = 0;

    private static bool isZoomingOnBattleStart = false;
    public static bool IsZoomingOnBattleStart { get => isZoomingOnBattleStart;}
    public static RTSCamera instance;


    RTSCamera()
    {
        instance = this;
    }

    public static void StartZoomBattleStart()
    {
        isZoomingOnBattleStart = true;
    }
    
    public static void StopZoomBattleStart()
    {
        isZoomingOnBattleStart = false;
    }

    private void Start()
    {
        targetY = transform.position.y;
        targetPos = transform.position;
    }

    void Update()
    {
        Vector2 inputMoveVector = InputManager.instance.GetMoveVector();
        float hor = inputMoveVector.x;
        float ver = inputMoveVector.y;
        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");

        Vector3 newPosOffsset = new Vector3();
        newPosOffsset += Vector3.forward * ver;
        newPosOffsset += -Vector3.left * hor;

        targetY += -scroll * scrollIntensity;
        targetY = Mathf.Clamp(targetY, minHeight, maxHeight);

        Vector3 moveVec = newPosOffsset.normalized;
        targetPos += moveVec * movementSpeed * Time.unscaledDeltaTime;
        targetPos.y = targetY;

        if (UIManager.instance.IsAnyPanelOpened_NotCountPause())
            return;

#if UNITY_ANDROID
        transform.position += new Vector3(inputMoveVector.x, 0, inputMoveVector.y) * movementSpeed/2 * Time.unscaledDeltaTime;
#else
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.unscaledDeltaTime * movementSpeed / 2f);
#endif
    }


    public void LookAtTransform(Vector3 targetTransformPosition)
    {
        float offsetZ = (Mathf.Sin(transform.localEulerAngles.x) * transform.localPosition.y) * 2f;
        Vector3 newTarget = new Vector3(targetTransformPosition.x, transform.position.y, targetTransformPosition.z + offsetZ);
        ChangeTargetPosition(newTarget);
    }

    void ChangeTargetPosition(Vector3 pos)
    {
        targetPos = pos;
    }

    public float GetDistanceToTarget()
    {
        return Vector3.Distance(transform.position, targetPos);
    }
}