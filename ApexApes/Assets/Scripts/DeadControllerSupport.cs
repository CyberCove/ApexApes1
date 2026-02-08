using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DeadControllerSupport : MonoBehaviour
{
    public Transform noLeftPos;
    public Transform noRightPos;
    public bool leftFound;
    public bool rightFound;
    public Transform left;
    public Transform right;

    private void Start()
    {
#if UNITY_EDITOR
        return;
#else

        InvokeRepeating("CheckControllers", 0.1f, 10f);
#endif
    }


    void CheckControllers()
    {
        List<InputDevice> leftHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, leftHandDevices);

        if (leftHandDevices.Count > 0)
        {
            leftFound = true;
        }

        List<InputDevice> rightHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, rightHandDevices);
        if (rightHandDevices.Count > 0)
        {
            rightFound = true;
        }

    }


    private void Update()
    {

#if UNITY_EDITOR
        return;
#else

            if (!leftFound)
            {
                left.position = noLeftPos.position;
                left.rotation = noLeftPos.rotation;
            }
            if (!rightFound)
            {
                right.position = noRightPos.position;
                right.rotation = noRightPos.rotation;
            }
#endif
    }
}

