using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardActionManager : MonoBehaviour
{
    [SerializeField] GameObject meleeAttackTemplate;
    [SerializeField] GameObject rangeAttackTemplate;

    public static CardActionManager instance;

    CardActionManager()
    {
        instance = this;
    }

    public void PrepareShootObject(ref GameObject shootObj, UnitAttackType unitAttackType)
    {
        switch (unitAttackType)
        {
            case UnitAttackType.Melee:
                shootObj.GetComponent<MeshFilter>().sharedMesh = meleeAttackTemplate.GetComponent<MeshFilter>().sharedMesh;
                shootObj.transform.localScale = meleeAttackTemplate.transform.localScale;
                shootObj.GetComponent<AudioSource>().clip = meleeAttackTemplate.GetComponent<AudioSource>().clip;
                break;
            case UnitAttackType.Range:
                shootObj.GetComponent<MeshFilter>().sharedMesh = rangeAttackTemplate.GetComponent<MeshFilter>().sharedMesh;
                shootObj.transform.localScale = rangeAttackTemplate.transform.localScale;
                shootObj.GetComponent<AudioSource>().clip = rangeAttackTemplate.GetComponent<AudioSource>().clip;
                break;
            default:
                break;
        }
    }
}
