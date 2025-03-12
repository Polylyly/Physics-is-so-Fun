using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooth : MonoBehaviour
{
    public Transform toothTip, toothTop;
    public LayerMask biteable;
    Transform initialPoint;
    public float toothLength, maxDistanceBetweenInitialPoints, offset;
    public Color toothColour;
    public GameObject holeDecal, bloodDecal;
    [SerializeField]
    float distanceFromInitial, highestDistance;
    bool bite = false;
    Color bloodColour;

    // Start is called before the first frame update
    void Start()
    {
        highestDistance = 0;
        toothLength = (toothTip.position - toothTop.position).magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        if (bite)
        {
            distanceFromInitial = Mathf.Clamp((Vector3.Dot(toothTip.position - initialPoint.position, -transform.up)), 0, toothLength);
            if (distanceFromInitial > highestDistance)
            {
                highestDistance = distanceFromInitial;
                float lerpValue = distanceFromInitial / toothLength;
                GetComponent<Renderer>().material.SetVector("_Offset", new Vector2(0, Mathf.Clamp01(lerpValue - offset)));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("touchy");
        if (other.GetComponent<Biteable>())
        {
            if (initialPoint != null)
            {
                float distanceBetweenInitials = (other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(toothTip.position) - initialPoint.position).magnitude;

                if (distanceBetweenInitials <= maxDistanceBetweenInitialPoints) //Something to check if it's close to old one
                {
                    bite = true;
                }
                else
                {
                    bite = true;
                    string gameObjectName = new string("Initial " + other.gameObject);
                    initialPoint = new GameObject(gameObjectName).transform;
                    initialPoint.position = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(toothTip.position);
                    initialPoint.parent = other.transform;
                    bloodColour = other.GetComponent<Biteable>().bloodColour;
                    if (bloodColour != GetComponent<Renderer>().material.GetColor("_BloodColour"))
                    {
                        highestDistance = 0;
                        float lerpValue = highestDistance / toothLength;
                        GetComponent<Renderer>().material.SetVector("_Offset", new Vector2(0, Mathf.Clamp01(lerpValue - offset)));
                    }
                    GetComponent<Renderer>().material.SetColor("_BloodColour", bloodColour);
                    GetComponent<Renderer>().material.SetColor("_ToothColour", toothColour);

                    //Below is the creation of the decals
                    //GameObject decalHole = Instantiate(holeDecal); //Might need to replace with decal projector
                    //decalHole.transform.parent = other.transform;
                    //decalHole.transform.position = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(toothTip.position);
                    //Sizing and stuff

                    //GameObject decalBlood = Instantiate(bloodDecal); //Might need to replace with decal projector
                    //decalBlood.transform.parent = other.transform;
                    //decalBlood.transform.position = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(toothTip.position);
                    //Sizing and stuff
                }
            }
            else
            {
                bite = true;
                string gameObjectName = new string("Initial " + other.gameObject);
                initialPoint = new GameObject(gameObjectName).transform;
                initialPoint.position = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(toothTip.position);
                initialPoint.parent = other.transform;
                bloodColour = other.GetComponent<Biteable>().bloodColour;
                GetComponent<Renderer>().material.SetColor("_BloodColour", bloodColour);
                GetComponent<Renderer>().material.SetColor("_ToothColour", toothColour);

                //Below is the creation of the decals
                //GameObject decalHole = Instantiate(holeDecal);
                //decalHole.transform.parent = other.transform;
                //decalHole.transform.position = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(toothTip.position);
                //Sizing and stuff

                //GameObject decalBlood = Instantiate(bloodDecal);
                //decalBlood.transform.parent = other.transform;
                //decalBlood.transform.position = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(toothTip.position);
                //Sizing and stuff
            }
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
