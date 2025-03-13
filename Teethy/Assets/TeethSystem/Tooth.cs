using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ch.sycoforge.Decal;

public class Tooth : MonoBehaviour
{
    public Transform toothTip, toothTop;
    public LayerMask biteable;
    Transform initialPoint;
    public float toothLength, maxDistanceBetweenInitialPoints, offset, offsetAtNull;
    public Color toothColour;
    public GameObject holeDecal;
    public SkinnedMeshRenderer avatarMesh;
    public int[] materialNumbers;
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
        if (highestDistance == 0)
        {
            foreach (int x in materialNumbers) avatarMesh.materials[x].SetVector("_Offset", new Vector2(0, offsetAtNull));
        }
        if (bite)
        {
            distanceFromInitial = Mathf.Clamp((Vector3.Dot(toothTip.position - initialPoint.position, -transform.up)), 0, toothLength);
            if (distanceFromInitial > highestDistance)
            {
                highestDistance = distanceFromInitial;
                float lerpValue = distanceFromInitial / toothLength;
                foreach (int x in materialNumbers) avatarMesh.materials[x].SetVector("_Offset", new Vector2(0, Mathf.Clamp01(lerpValue - offset)));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Biteable>())
        {
            if (initialPoint != null)
            {
                float distanceBetweenInitials = (other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(toothTip.position) - initialPoint.position).magnitude;

                if (distanceBetweenInitials <= maxDistanceBetweenInitialPoints) //Checks if close to old bite
                {
                    Debug.Log("Too close");
                    bite = true;
                }
                else
                {
                    Debug.Log("Chomp 2");
                    bite = true;
                    string gameObjectName = new string("Initial " + other.gameObject);
                    initialPoint = new GameObject(gameObjectName).transform;
                    initialPoint.position = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(toothTip.position);
                    initialPoint.parent = other.transform;
                    bloodColour = other.GetComponent<Biteable>().bloodColour;
                    
                    foreach (int x in materialNumbers)
                    {
                        if (bloodColour != avatarMesh.materials[x].GetColor("_BloodColour"))
                        {
                            highestDistance = 0;
                            float lerpValue = highestDistance / toothLength;
                            avatarMesh.materials[x].SetVector("_Offset", new Vector2(0, Mathf.Clamp01(lerpValue - offset)));
                        }
                        avatarMesh.materials[x].SetColor("_BloodColour", bloodColour);
                        avatarMesh.materials[x].SetColor("_ToothColour", toothColour);
                    }

                    //Below is the creation of the decals
                    GameObject decalHole = Instantiate(holeDecal); //Might need to replace with decal projector
                    decalHole.transform.parent = other.transform;
                    decalHole.transform.position = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(toothTip.position);
                    decalHole.GetComponent<EasyDecal>().DecalMaterial.color = bloodColour;
                    decalHole.GetComponent<EasyDecal>().ProjectionTarget = other.GetComponent<Biteable>().decalTarget;
                    decalHole.GetComponent<EasyDecal>().Distance = 0.1f;
                    //Sizing and stuff
                }
            }
            else
            {
                Debug.Log("chomp");

                bite = true;
                string gameObjectName = new string("Initial " + other.gameObject);
                initialPoint = new GameObject(gameObjectName).transform;
                initialPoint.position = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(toothTip.position);
                initialPoint.parent = other.transform;
                bloodColour = other.GetComponent<Biteable>().bloodColour;
                foreach (int x in materialNumbers)
                {
                    avatarMesh.materials[x].SetColor("_BloodColour", bloodColour);
                    avatarMesh.materials[x].SetColor("_ToothColour", toothColour);
                }

                //Below is the creation of the decals
                GameObject decalHole = Instantiate(holeDecal);
                decalHole.transform.parent = other.transform;
                decalHole.transform.position = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(toothTip.position);
                decalHole.GetComponent<EasyDecal>().DecalMaterial.color = bloodColour;
                decalHole.GetComponent<EasyDecal>().ProjectionTarget = other.GetComponent<Biteable>().decalTarget;
                decalHole.GetComponent<EasyDecal>().Distance = 0.1f;
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
