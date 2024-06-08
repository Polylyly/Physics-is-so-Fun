using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mass : MonoBehaviour
{
    public float mass;

    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<Rigidbody>().mass = mass;
    }
}
