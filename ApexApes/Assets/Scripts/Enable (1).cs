using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enable : MonoBehaviour
{
    public GameObject objectEnable;


    public void OnTriggerEnter()
    {
        objectEnable.SetActive(true);
    }
}