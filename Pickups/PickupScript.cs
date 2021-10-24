using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.UIElements;
using UnityEngine;

public class PickupScript : MonoBehaviour
{
    [Header("Pickup Information")]
    [SerializeField] int weaponType;
    bool preventPickup = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("player") && !preventPickup)
        {
            TankController tc = other.GetComponent<TankController>();
            if (tc.Pickup(weaponType))
            {
                preventPickup = true;
                Destroy(transform.root.gameObject);
            }
                
        }
    }
}
