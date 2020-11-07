using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    [SerializeField] float speed = 10;
    [SerializeField] float lerpSpeed = 8;
    [SerializeField] float maxOffset = 15;
    float startX;
    Vector3 targetPos = new Vector3();

    void Awake()
    {
        targetPos = transform.position;
        startX = targetPos.x;
    }

    void Update()
    {
        float hor = Input.GetAxisRaw("Horizontal");

        targetPos += new Vector3(hor * Time.unscaledDeltaTime * speed, 0, 0);
        targetPos = new Vector3(Mathf.Clamp(targetPos.x, startX - maxOffset, startX + maxOffset), targetPos.y, targetPos.z);

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.unscaledDeltaTime * lerpSpeed);
    }
}
