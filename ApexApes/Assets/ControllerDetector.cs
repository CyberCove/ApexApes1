using UnityEngine;
using UnityEngine.XR;

public class ControllerDetector : MonoBehaviour
{
    [Header("Made By Nelom! Dont Have To Credit!")]
    [Header("Settings")]
    public GameObject LeftController;
    public GameObject RightController;
    public Transform LeftPosition;
    public Transform RightPosition;

    [Header("How long it should check uhh boi!")]
    public float Timing = 0.5f; 

    private float timeboi;

    void Update()
    {
        timeboi += Time.deltaTime;

        if (timeboi >= Timing)
        {
            timeboi = 0f;
            CheckControllers();
        }
    }

    void CheckControllers()
    {


        // Left Controller
        if (!IsControllerConnected(XRNode.LeftHand))
        {
            if (LeftController != null && LeftPosition != null)
            {
                LeftController.transform.position = LeftPosition.position;
                LeftController.transform.rotation = LeftPosition.rotation;
                Debug.Log("Left Controller Was Not Found!");
            }
        }

        // Right Controller
        if (!IsControllerConnected(XRNode.RightHand))
        {
            if (RightController != null && RightPosition != null)
            {
                RightController.transform.position = RightPosition.position;
                RightController.transform.rotation = RightPosition.rotation;
                Debug.Log("Right Controller Was Not Found wow");
            }
        }
    }

    bool IsControllerConnected(XRNode node)
    {
        var inputDevices = new System.Collections.Generic.List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(node, inputDevices);

        return inputDevices.Count > 0;
    }
}