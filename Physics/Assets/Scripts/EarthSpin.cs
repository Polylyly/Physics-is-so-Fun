using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthSpin : MonoBehaviour
{
    public float spinSpeed;

    void Update()
    {
        transform.Rotate(0, 0, spinSpeed, 0);
        if (transform.rotation.z == 360) transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, 0, transform.rotation.w);
    }
}
