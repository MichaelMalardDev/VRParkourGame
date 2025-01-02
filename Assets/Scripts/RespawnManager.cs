using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ContinuousMovementPhysics persoMovement = other.GetComponentInParent<ContinuousMovementPhysics>();
            SOPerso playerData = persoMovement.playerData;
            playerData.respawnPoint = transform.position;
        }
    }
}
