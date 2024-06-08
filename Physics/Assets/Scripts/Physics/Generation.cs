using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generation : MonoBehaviour
{
    public Transform problemSpawnPoint;

    [Header("Prefabs")]
    public GameObject NormalFlatGround;
    public GameObject NormalRamp, FrictionFlatGround, FrictionRamp, TensionProb, ViscousDragProb, UpthrustProb;
    public GameObject DisplacementProb, VelocityProb, AccelerationProb, TimeProb, ProjectileProb;
    public GameObject MomentumProb, ImpulseProb, SecondLawProb, CentripetalProb;

    void ProblemDecider()
    {

    }

    //Forces
    void Normal()
    {
        //Flat ground
        GameObject problem = Instantiate(NormalFlatGround, problemSpawnPoint);
        problem.GetComponentInChildren<Mass>().mass = Random.Range(0.01f, 1000);
        float answer = problem.GetComponentInChildren<Mass>().mass * Physics.gravity.y * -1;
        //Ramp

    }

    void Friction()
    {
        //Flat ground

        //Ramp

    }

    void Tension()
    {

    }

    void ViscousDrag()
    {

    }

    void Upthrust()
    {

    }

    void Kinematics()
    {
        //Displacement

        //Velocity

        //Acceleration

        //Time

        //Projectile Motion

    }

    void Momentum()
    {
        //Momentum of an object

        //Impulse

        //Newton's Second Law

    }

    void CircularMotion()
    {
        //Centripetal Force

    }
}
