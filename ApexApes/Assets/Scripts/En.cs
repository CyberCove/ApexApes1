using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class En : MonoBehaviour
{
    public GameObject objectEnable;


    public void OnTriggerEnter()
    {
        objectEnable.SetActive(true);
    }
}