using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkedRagdollSpawner : MonoBehaviour
{
    [Header("Make sure ur ragdoll is in the Resources folder")]
    public GameObject Ragdoll;
    [Header("The location where the ragdolls spawn")]
    public Transform SpawnPos;
    [Header("collider used for ur hand so handtag or ur finger colliders")]
    public string HandTag = "HandTag";
    [Header("the cooldown between spawning")]
    public float SpawnCooldown = 0.5f;
    private bool CanSpawn = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(HandTag) && !CanSpawn)
        {
            StartCoroutine(Spawn());
        }
    }

   

    IEnumerator Spawn()
    {
        CanSpawn = true;
        PhotonNetwork.Instantiate(Ragdoll.name, SpawnPos.position, SpawnPos.rotation);
        yield return new WaitForSeconds(SpawnCooldown);
        CanSpawn = false;
    }
}
