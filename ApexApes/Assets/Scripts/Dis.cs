using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dis : MonoBehaviour
{
    public GameObject objectDisable;

    private void OnTriggerEnter(Collider other)
    {
        objectDisable.SetActive(false);
    }
}
