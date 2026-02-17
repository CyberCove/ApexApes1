using System.Collections;
using UnityEngine;
using easyInputs;

public class Vibrations : MonoBehaviour
{
    public float Amplitude;
    public bool LeftHand;
    public float duration = 0.10f;

    void OnTriggerEnter()
    {
        if (LeftHand)
        {
            StartCoroutine(EasyInputs.Vibration(EasyHand.LeftHand, Amplitude, duration));
        }
        else
        {
            StartCoroutine(EasyInputs.Vibration(EasyHand.RightHand, Amplitude, duration));
        }
    }
}
