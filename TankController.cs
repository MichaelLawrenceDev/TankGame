using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using UnityEditorInternal;
using System;
using System.Diagnostics;

public class TankController : MonoBehaviour
{
    [Header("Components:")] 
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] GameObject gatlingBullet;
    [SerializeField] GameObject defaultBullet;
    Animator anim;

    [Header("Controls:")]
    [SerializeField] KeyCode keyForward;
    [SerializeField] KeyCode keyBack;
    [SerializeField] KeyCode keyLeft;
    [SerializeField] KeyCode keyRight;
    [SerializeField] KeyCode keyShoot;

    [Header("Tank Properties:")]
    [SerializeField] float rotSmoothAmount = 0.01f;
    [SerializeField] float speedSmoothAmount = 0.01f;
    [SerializeField] float forwardSpeed;
    [Range(0, 360)]
    [SerializeField] float rotSpeed;
    [HideInInspector] public float currentSpeed; // Accessed by animator
    Vector3 moveDir;
    float moveSmooth = 0;
    float rotSmooth = 0;
    Vector3 rotDir;
    Rigidbody rb;
    Player player;

    [Header("Combat Properties:")]
    [SerializeField] float health;
    float currentHealth;
    [Tooltip("Time until next shot may be fired.")]
    [SerializeField] float fireDelay;
    [Tooltip("Amount of stored shots, 0 = unlimited")]
    [SerializeField] float vollyAmount;
    [Tooltip("Time until shot is ready to fire, ignore if vollyAmount is set to 0.")]
    [SerializeField] float chargeTime;
    [SerializeField] int weaponType = 0;
    [SerializeField] int timeTillPickupDisable = 10;
    [HideInInspector] public bool isFiring = false;
    float currentFireTime;
    float currentChargeTime;
    float vollyCount;

    private void Start()
    {
        currentFireTime = -fireDelay;
        currentChargeTime = -currentChargeTime;
        vollyCount = vollyAmount;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        currentHealth = health;
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.TransformDirection(moveDir) * forwardSpeed * Time.deltaTime);
        if (player.isDead() && rb.velocity == Vector3.zero)
        {
            // if dead, freeze body when not moving.
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
        }
    }
    private void Update()
    {
        UpdateMove(); // Forward & back movement
        Rotate(); // Left & Right movement
        if (!player.isDead())
        {
            switch (weaponType)
            {
                default: // no special weapons ... 
                    DefaultShooting();
                    break;
                case 1: // Gatling Gun
                    GatlingShooting();
                    break;
            }
        }

        UpdateAnimator();
    }
    public void setPlayer(Player player)
    {
        this.player = player;
    }
    private void UpdateAnimator()
    {
        anim.SetBool("Killed?", player.isDead());
        anim.SetBool("isFiring", isFiring);
        anim.SetFloat("currentSpeed", Mathf.Clamp(forwardSpeed * moveSmooth, -20f, 20f));
        anim.SetFloat("currentRot", rotSmooth * (rotSpeed/360));
        anim.SetInteger("weaponType", weaponType);
        //anim.SetFloat("Spinning Speed", spinningSpeed);
    }
    public void SpawnBullet(int type) // default bullet
    {
        GameObject bulletObj;
        switch (type)
        {
            default: // no special weapons ... 
                AudioManager.PlaySound(Sound.DefaultFire, transform.position);
                bulletObj = Instantiate(defaultBullet);
                bulletObj.transform.rotation = bulletSpawnPoint.rotation;
                vollyCount--;
                break;
            case 1: // Gatling Gun
                AudioManager.PlaySound(Sound.GatlingFire, transform.position);
                bulletObj = Instantiate(gatlingBullet);
                Vector3 rot = bulletSpawnPoint.eulerAngles;
                rot.y += Random.Range(-3f, 3f);
                bulletObj.transform.eulerAngles = rot;
                break;
        }
        AudioManager.PlaySound(Sound.DefaultFire, transform.position);
        bulletObj.transform.position = bulletSpawnPoint.position;
        bulletObj.GetComponent<BulletPhysics>().changeOwnership(player);
    }
    private void UpdateMove()
    {
        if (!player.isDead())
        {
            if (Input.GetKey(keyForward)) // Forward
                moveSmooth = Mathf.Clamp(moveSmooth + speedSmoothAmount, -1f, 1f);
            if (Input.GetKey(keyBack)) // Backward
                moveSmooth = Mathf.Clamp(moveSmooth - speedSmoothAmount, -1f, 1f);
        }
        if ((player.isDead() || !Input.GetKey(keyBack) && !Input.GetKey(keyForward)) && moveSmooth != 0)
        {
            moveSmooth += (moveSmooth > 0 ? -1 : 1) * speedSmoothAmount;
            if (Mathf.Abs(moveSmooth) < speedSmoothAmount) moveSmooth = 0;
        }

        moveDir = new Vector3(moveSmooth, 0, 0);
    }
    private void Rotate()
    {
        if (!player.isDead())
        {
            if (Input.GetKey(keyLeft))
                rotSmooth = Mathf.Clamp(rotSmooth - rotSmoothAmount, -1f, 1f);
            if (Input.GetKey(keyRight))
                rotSmooth = Mathf.Clamp(rotSmooth + rotSmoothAmount, -1f, 1f);
        }
        if ((player.isDead() || !Input.GetKey(keyLeft) && !Input.GetKey(keyRight)) && rotSmooth != 0)
        {
            rotSmooth += (rotSmooth > 0 ? -1 : 1) * rotSmoothAmount * 2;
            if (Mathf.Abs(rotSmooth) < rotSmoothAmount) rotSmooth = 0;
        }

        rotDir = new Vector3(0, rotSmooth * Time.deltaTime * rotSpeed, 0);
        transform.Rotate(rotDir);
    }

    private void DefaultShooting()
    {
        isFiring = false;
        if (Input.GetKey(keyShoot) || Input.GetKeyDown(keyShoot)) // has input...
        {
            if ((Time.time > currentFireTime + fireDelay) && vollyCount != 0) // delay time has elapsed and has a shot charged...
            {
                // update timers...
                currentFireTime = Time.time;
                if (vollyCount == vollyAmount)
                    currentChargeTime = Time.time;

                // FIRE!
                isFiring = true;
            }
        }

        // Error sound if trying to fire when no shell is avaliable
        if (Input.GetKeyDown(keyShoot) && vollyCount == 0)
            AudioManager.PlaySound(Sound.Error, transform.position);

        // Charge shell when avaliable.
        if ((vollyCount < vollyAmount) && (Time.time > currentChargeTime + chargeTime))
        {
            currentChargeTime = Time.time;
            AudioManager.PlaySound(Sound.Charged, transform.position);
            vollyCount++;
        }
    }
    private void GatlingShooting()
    {
        isFiring = Input.GetKey(keyShoot) || Input.GetKeyDown(keyShoot);
    }
    public bool dealDamage(int amount) 
    {
        if (!player.isDead()) // if already dead, dont deal damage.
        {
            player.AddHP(-amount);
            if (player.isDead()) // if died from this damage...
            {
                isFiring = false;
                AudioManager.PlaySound(Sound.Explosion, transform.position);
            }
            return true;
        }
        return false;
    }
    public bool isDead()
    {
        return player.isDead();
    }
    public bool Pickup(int type)
    {
        if (!player.isDead())
        {
            AudioManager.PlaySound(Sound.Pickup, transform.position);
            UnityEngine.Object.FindObjectOfType<GameManager>().PlayPickupSong(timeTillPickupDisable);
            weaponType = type;
            if (timeTillPickupDisable > 0) // do not disable if 0
                StartCoroutine(DisablePickupIn(timeTillPickupDisable));
        }
        return !player.isDead();
    }

    IEnumerator DisablePickupIn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        vollyCount = vollyAmount;
        weaponType = 0;
    }
    public void ResetTank()
    {
        currentHealth = health;
        vollyCount = vollyAmount;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.isKinematic = false;
    }
}
