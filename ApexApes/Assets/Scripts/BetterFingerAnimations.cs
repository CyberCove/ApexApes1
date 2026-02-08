using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;

public enum HandType
{
    Left,
    Right
}

[ExecuteAlways]
public class BetterFingerAnimations : MonoBehaviourPun, IPunObservable
{
    public HandType handType;
    public float animationSpeed = 15f; // Speed of smooth movement

    [Header("Finger Bones (Drag & Drop)")]
    public List<Transform> buttonBones; // Thumb bones
    public List<Transform> triggerBones; // Index finger bones
    public List<Transform> gripBones; // Middle finger bones

    [Header("Default Open Rotations")]
    public List<Quaternion> buttonOpenRotations = new List<Quaternion>();
    public List<Quaternion> triggerOpenRotations = new List<Quaternion>();
    public List<Quaternion> gripOpenRotations = new List<Quaternion>();

    [Header("Curl Rotations")]
    public List<Quaternion> buttonCurlRotations = new List<Quaternion>();
    public List<Quaternion> triggerCurlRotations = new List<Quaternion>();
    public List<Quaternion> gripCurlRotations = new List<Quaternion>();

    [Header("Edit Mode Options")]
    public bool editMode = false;
    public bool saveOpenRotations = false;
    public bool saveCurlRotations = false;

    private InputDevice inputDevice;

    private float buttonValue; // Current thumb curl value
    private float triggerValue; // Current index finger curl value
    private float gripValue; // Current middle finger curl value

    private float targetButtonValue; // Target thumb curl value
    private float targetTriggerValue; // Target index finger curl value
    private float targetGripValue; // Target middle finger curl value

    private float buttonVelocity; // Smooth velocity for thumb
    private float triggerVelocity; // Smooth velocity for index finger
    private float gripVelocity; // Smooth velocity for middle finger

    private float networkButtonValue; // Synced thumb value
    private float networkTriggerValue; // Synced index finger value
    private float networkGripValue; // Synced middle finger value

    void Start()
    {
        InitializeRotations(buttonBones, buttonOpenRotations, buttonCurlRotations);
        InitializeRotations(triggerBones, triggerOpenRotations, triggerCurlRotations);
        InitializeRotations(gripBones, gripOpenRotations, gripCurlRotations);

        if (photonView.IsMine && !editMode)
        {
            inputDevice = GetInputDevice();
        }
    }

    void Update()
    {
        if (editMode)
        {
            HandleEditMode();
        }
        else
        {
            if (photonView.IsMine && Application.isPlaying)
            {
                AnimateFingers();
            }
            else
            {
                AnimateFingersFromNetwork();
            }
        }
    }

    InputDevice GetInputDevice()
    {
        InputDeviceCharacteristics controllerCharacteristic = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller;

        if (handType == HandType.Left)
        {
            controllerCharacteristic |= InputDeviceCharacteristics.Left;
        }
        else
        {
            controllerCharacteristic |= InputDeviceCharacteristics.Right;
        }

        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristic, inputDevices);

        return inputDevices.Count > 0 ? inputDevices[0] : default;
    }

    void AnimateFingers()
    {
        // Read VR controller inputs
        inputDevice.TryGetFeatureValue(CommonUsages.trigger, out targetTriggerValue); // Target for index finger
        inputDevice.TryGetFeatureValue(CommonUsages.grip, out targetGripValue);       // Target for middle finger
        inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonPressed);
        inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonPressed);

        // Thumb movement
        targetButtonValue = (primaryButtonPressed || secondaryButtonPressed) ? 1f : 0f;

        // Smooth transitions for all fingers
        buttonValue = Mathf.SmoothDamp(buttonValue, targetButtonValue, ref buttonVelocity, 1f / animationSpeed);
        triggerValue = Mathf.SmoothDamp(triggerValue, targetTriggerValue, ref triggerVelocity, 1f / animationSpeed);
        gripValue = Mathf.SmoothDamp(gripValue, targetGripValue, ref gripVelocity, 1f / animationSpeed);

        // Apply rotations to bones
        ApplyRotations(buttonBones, buttonValue, buttonOpenRotations, buttonCurlRotations);
        ApplyRotations(triggerBones, triggerValue, triggerOpenRotations, triggerCurlRotations);
        ApplyRotations(gripBones, gripValue, gripOpenRotations, gripCurlRotations);
    }

    void AnimateFingersFromNetwork()
    {
        // Smooth transitions for networked fingers
        buttonValue = Mathf.Lerp(buttonValue, networkButtonValue, Time.deltaTime * animationSpeed);
        triggerValue = Mathf.Lerp(triggerValue, networkTriggerValue, Time.deltaTime * animationSpeed);
        gripValue = Mathf.Lerp(gripValue, networkGripValue, Time.deltaTime * animationSpeed);

        // Apply rotations to bones
        ApplyRotations(buttonBones, buttonValue, buttonOpenRotations, buttonCurlRotations);
        ApplyRotations(triggerBones, triggerValue, triggerOpenRotations, triggerCurlRotations);
        ApplyRotations(gripBones, gripValue, gripOpenRotations, gripCurlRotations);
    }

    void ApplyRotations(List<Transform> bones, float value, List<Quaternion> openRotations, List<Quaternion> curlRotations)
    {
        for (int i = 0; i < bones.Count; i++)
        {
            if (bones[i] != null && i < openRotations.Count && i < curlRotations.Count)
            {
                Quaternion openRotation = openRotations[i];
                Quaternion curlRotation = curlRotations[i];
                bones[i].localRotation = Quaternion.Lerp(openRotation, curlRotation, value);
            }
        }
    }

    void HandleEditMode()
    {
        if (saveOpenRotations)
        {
            SaveRotations(buttonBones, buttonOpenRotations);
            SaveRotations(triggerBones, triggerOpenRotations);
            SaveRotations(gripBones, gripOpenRotations);

            saveOpenRotations = false;
            Debug.Log("Open rotations saved!");
        }

        if (saveCurlRotations)
        {
            SaveRotations(buttonBones, buttonCurlRotations);
            SaveRotations(triggerBones, triggerCurlRotations);
            SaveRotations(gripBones, gripCurlRotations);

            saveCurlRotations = false;
            Debug.Log("Curl rotations saved!");
        }
    }

    void InitializeRotations(List<Transform> bones, List<Quaternion> openRotations, List<Quaternion> curlRotations)
    {
        while (openRotations.Count < bones.Count)
        {
            openRotations.Add(Quaternion.identity);
        }

        while (curlRotations.Count < bones.Count)
        {
            curlRotations.Add(Quaternion.identity);
        }
    }

    void SaveRotations(List<Transform> bones, List<Quaternion> targetRotations)
    {
        for (int i = 0; i < bones.Count; i++)
        {
            if (bones[i] != null)
            {
                if (i >= targetRotations.Count)
                {
                    targetRotations.Add(bones[i].localRotation);
                }
                else
                {
                    targetRotations[i] = bones[i].localRotation;
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(buttonValue);
            stream.SendNext(triggerValue);
            stream.SendNext(gripValue);
        }
        else
        {
            networkButtonValue = (float)stream.ReceiveNext();
            networkTriggerValue = (float)stream.ReceiveNext();
            networkGripValue = (float)stream.ReceiveNext();
        }
    }
}
