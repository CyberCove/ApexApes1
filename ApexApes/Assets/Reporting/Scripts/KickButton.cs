using UnityEngine;

public class KickButton : MonoBehaviour
{
    public int ButtonNumber;
    public LeaderBoard LB;
    public string HandTag = "HandTag";
    public Material PressedMaterial;
    private Material UnPressedMaterial;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        UnPressedMaterial = rend.material;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(HandTag))
        {
            LB.KickPress(ButtonNumber);
            rend.material = PressedMaterial;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(HandTag))
            rend.material = UnPressedMaterial;
    }
}