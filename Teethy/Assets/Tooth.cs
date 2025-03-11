using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooth : MonoBehaviour
{
    public Transform toothTip;
    public LayerMask biteable;
    Vector3 initialPoint;
    [SerializeField]
    float distanceFromInitial, highestDistance;
    bool bite = false;

    // Start is called before the first frame update
    void Start()
    {
        highestDistance = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (bite)
        {
            distanceFromInitial = (toothTip.position - initialPoint).magnitude;
            //Change this so it only works in the direction parallel to the tooth
            if (distanceFromInitial > highestDistance) highestDistance = distanceFromInitial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("touchy");
        if (other.GetComponent<Biteable>())
        {
            Debug.Log("good touchy");
            bite = true;
            initialPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(toothTip.position);
            //Change this in such a way that biting near an old initial point will just make that the new initial point
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Biteable>())
        {
            bite = false;
        }
    }
}
