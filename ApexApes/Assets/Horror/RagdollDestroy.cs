using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollDestroy : MonoBehaviour
{
    [Header ("THIS SCRIPT WAS MADE BY B.AWESOME")]
    [Header("Drag this script onto your ragdoll prefab while inside the prefab itself.")]

    [Header("Set this to the ragdoll prefab.")]
    public GameObject Ragdoll;
    [Header("Set this to the amount of time you want the ragdoll to be there.")]
    public float destroyTime = 5f;
    private bool isDestroying = false;

    void Update()
    {
        if (Ragdoll.activeSelf && !isDestroying)
        {
            isDestroying = true;
            StartCoroutine(DestroyObject());
        }
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(destroyTime);

        Destroy(this.gameObject);
    }
}
