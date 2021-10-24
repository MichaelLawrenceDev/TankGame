using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetAttractor : MonoBehaviour
{
    // https://www.youtube.com/watch?v=gHeQ8Hr92P4&t=7s

    [SerializeField] float gravity = -10f;

    public void Attract(Transform body, bool freezeRotation, bool ignoreGravity, float gravityMultiplier = 1) 
    {
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;

        if (!ignoreGravity) body.GetComponent<Rigidbody>().AddForce(gravityUp * gravity * gravityMultiplier);
        if (freezeRotation)
        {
            Quaternion targetRot = Quaternion.FromToRotation(bodyUp,gravityUp) * body.rotation;
            body.rotation = targetRot;
        }
    }
}
