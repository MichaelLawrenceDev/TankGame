using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class Attract : MonoBehaviour
{
    [SerializeField] PlanetAttractor planet;
    [SerializeField] float gravityMultiplier = 1f;
    [SerializeField] bool freezeRotation;
    [SerializeField] bool ignoreGravity = false;
    [SerializeField] Vector3 offsetRotation = Vector3.zero;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        if (planet == null)
            planet = FindObjectOfType<PlanetAttractor>();

        rb = GetComponent<Rigidbody>();
        if (freezeRotation) rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        planet.Attract(transform, freezeRotation, ignoreGravity, gravityMultiplier);
    }
}
