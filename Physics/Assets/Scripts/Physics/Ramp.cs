using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour
{
    public float angle;
    public float coefficientFrictionStatic, coefficientFrictionDynamic;

    Transform rotation;

    // Start is called before the first frame update
    void Start()
    {
        rotation = transform.Find("Rotation");
    }
}
