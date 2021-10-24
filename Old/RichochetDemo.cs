using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RichochetDemo : MonoBehaviour
{
    Rigidbody rb;
    Vector3 startPos;
    Quaternion startRot;
    float speed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddRelativeForce(new Vector3(0, 0, 1) * speed, ForceMode.VelocityChange);
        startPos = transform.position;
        startRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reset
            transform.position = startPos;
            transform.rotation = startRot;
            rb.velocity = Vector3.zero;
            rb.AddRelativeForce(new Vector3(0, 0, 1) * 5f, ForceMode.VelocityChange);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        Vector3 normal = collision.GetContact(0).normal;
        Vector3 reflect = Vector3.Reflect(rb.velocity, normal);

        Debug.Log("normal: " + normal);
        Debug.Log("reflect: " + reflect);
        Debug.Log(Vector3.Angle(rb.velocity, normal));
        // rb.velocity = reflect * speed;
        Vector3 rot = transform.localEulerAngles;
        rot.y += Vector3.Angle(rb.velocity, normal);
        transform.localEulerAngles = rot;
        rb.velocity = transform.forward * speed;
    }
}
