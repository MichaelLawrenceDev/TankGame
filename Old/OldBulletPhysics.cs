using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

public class OldBulletPhysics : MonoBehaviour
{
    [Header("Bullet Properties")]
    [SerializeField] ParticleSystem trail;
    [SerializeField] int damage;
    [SerializeField] float speed;
    [SerializeField] float timeTillKill;
    float gracePeriod = 0f; // Time until bullet becomes active
    bool collisionActive = false;
    bool isDestroyed = false;
    float currentTime = 0;
    GameObject bulletPivot;

    public void Start()
    {
        // set pivot to center
        bulletPivot = new GameObject("Bullet Pivot");
        bulletPivot.transform.rotation = transform.rotation;
        transform.parent = bulletPivot.transform;
    }

    public void Update()
    {
        // Move bullet
        if (!isDestroyed)
        {
            bulletPivot.transform.Rotate(new Vector3(0, 0, speed * Time.deltaTime));

            // Destroy if time elapsed
            if (currentTime > timeTillKill)
            {
                StartCoroutine(DestroyBullet()); // Destroy in 2s
            }

            if (currentTime > gracePeriod) collisionActive = true;
            currentTime += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collisionActive && other.gameObject.layer == LayerMask.NameToLayer("player"))
        {
            TankController tc = other.gameObject.GetComponent<TankController>();
            tc.dealDamage(damage);
            StartCoroutine(DestroyBullet());
        }
    }
    IEnumerator DestroyBullet()
    {
        // disable for 2s, let trail finish
        isDestroyed = true;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        trail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        yield return new WaitForSeconds(2f);

        Destroy(bulletPivot);
        Destroy(gameObject);
    }
}
