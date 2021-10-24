using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraEffects : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] Transform camPivot;
    [SerializeField] List<Transform> spawnBulletPoints;
    [SerializeField] float[] timesToFireBullet;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float rotationSpeed; // per sec
    [Header("Fired Bullet Properties")]
    [SerializeField] List<float> fireBulletsIn;
    [SerializeField] float transitionFiringModeIn = 5;
    float transitionTime;
    int fireMode;
    GameObject menuEffects;
    private void Start()
    {
        // Set up firing times
        transitionTime = Time.time + transitionFiringModeIn;
        timesToFireBullet = new float[spawnBulletPoints.Count];
        for (int i = 0; i < spawnBulletPoints.Count; i++)
            timesToFireBullet[i] = fireBulletsIn[0]; // <-- set to negative to start shooting immediatly
    }
    private void Update()
    {
        camPivot.Rotate(new Vector3(0f, -rotationSpeed * Time.deltaTime, 0f));

        // Spawn Bullets...
        for (int i = 0; i < spawnBulletPoints.Count; i++)
        {
            if (Time.time >= timesToFireBullet[i])
            {
                // Add random fire time
                float timeAdjust = Random.Range(fireBulletsIn[fireMode] / -4f, fireBulletsIn[fireMode] / 4f);
                timesToFireBullet[i] = Time.time + timeAdjust + fireBulletsIn[fireMode];

                // Fire bullet
                FireBullet(spawnBulletPoints[i]);
            }
        }

        // Change Firing Mode
        if (Time.time >= transitionTime)
        {
            transitionTime = Time.time + transitionFiringModeIn;
            fireMode++;
            if (fireMode == fireBulletsIn.Count) fireMode = 0;
        }
    }
    private void FireBullet(Transform spawn)
    {
        // create if empty
        if (menuEffects == null)
            CreateMenuEffectsObj();

        // New Rotation
        Vector3 rot = spawn.localEulerAngles;
        rot.y += Random.Range(0f, 360f);
        spawn.localEulerAngles = rot;

        // Spawn Bullet
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.transform.position = spawn.position;
        bullet.transform.rotation = spawn.rotation;
        bullet.transform.parent = menuEffects.transform;
    }
    private void CreateMenuEffectsObj()
    {
        menuEffects = new GameObject("Menu Effects");
        menuEffects.transform.parent = transform;
    }
    public void StopEffects()
    {
        if (menuEffects != null)
        {
            foreach (Transform child in menuEffects.transform)
                GameObject.Destroy(child.gameObject);
        }
        gameObject.SetActive(false);
    }
}
