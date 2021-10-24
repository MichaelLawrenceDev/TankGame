using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public class BulletPhysics : MonoBehaviour
{
    [Header("Bullet Properties")]
    [SerializeField] bool offset = false;
    [SerializeField] ParticleSystem trail;
    [SerializeField] int damage;
    [SerializeField] float speed;
    [SerializeField] float timeTillKill;
    Rigidbody rb;
    Player owner;
    Vector3 lastVelocity;
    Vector3 forward;
    float gracePeriod = 0.05f; // Time until bullet becomes active
    bool collisionActive = false;
    bool isDestroyed = false;
    float currentTime = 0;
    GameObject bulletPivot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
    }

    void Update()
    {
        // Move bullet
        if (!isDestroyed)
        {
            // Destroy if time elapsed 
            if (currentTime > timeTillKill)
                StartCoroutine(DestroyBullet());
            if (currentTime > gracePeriod) collisionActive = true;
            currentTime += Time.deltaTime;
        }
    }
    public void changeOwnership(Player player)
    {
        owner = player;
    }

    private void FixedUpdate()
    {
        Vector3 moveDir = new Vector3(1, 0, 0);
        //rb.MovePosition(rb.position + transform.TransformDirection(moveDir) * speed * Time.deltaTime);
        forward = offset ? -transform.up : transform.forward;
        rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collisionActive && collision.gameObject.layer == LayerMask.NameToLayer("player"))
        {
            // Deal damage to player
            TankController tc = collision.gameObject.GetComponent<TankController>();
            if (tc.dealDamage(damage))
                StartCoroutine(DestroyBullet());
            else
                Richochet(collision);
        }
        else if (collision.gameObject.layer != LayerMask.NameToLayer("ground"))
            Richochet(collision);
    }
    private void Richochet(Collision collision)
    {
        Vector3 reflectDir = Vector3.Reflect(transform.forward, collision.GetContact(0).normal);
        transform.rotation = Quaternion.LookRotation(reflectDir);
        rb.velocity = transform.forward * speed;
    }
    IEnumerator DestroyBullet()
    {
        // disable for 2s, let trail finish
        isDestroyed = true;
        rb.detectCollisions = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.isKinematic = true;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        rb.velocity = Vector3.zero;
        trail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        yield return new WaitForSeconds(2f);

        Destroy(bulletPivot);
        Destroy(gameObject);
    }
}
