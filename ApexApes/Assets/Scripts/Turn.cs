using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class Turn : MonoBehaviour
{
    [Tooltip("You put the Gorilla Player gameobject here.")]
    public GameObject GorillaPlayer;
    [Tooltip("The controller you turn with. You should probably set it to right hand.")]
    public XRController controller;
    [Tooltip("Amount in degrees of the turn. You should probably keep this 30 or 45.")]
    public float turnAmount = 45f;
    [Tooltip("How much you have to move the joystick to turn. You shouldn't have to change this.")]
    public float inputThreshold = 0.75f;
    [Tooltip("Wait time between turns. You should probably keep this at 1.")]
    public float turnCooldown = 1;

    private float lastTurnTime;

    private void Update()
    {
        Vector2 inputAxis;
        controller.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

        if (Mathf.Abs(inputAxis.x) > inputThreshold && Time.time - lastTurnTime > turnCooldown)
        {
            if (inputAxis.x > 0)
            {
                GorillaPlayer.transform.Rotate(0, turnAmount, 0);
            }
            else
            {
                GorillaPlayer.transform.Rotate(0, -turnAmount, 0);
            }

            lastTurnTime = Time.time;
        }
    }
}
