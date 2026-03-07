using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MuteButton : MonoBehaviour
{
    public int ButtonNumber;
    public LeaderBoard LB;
    public string HandTag = "HandTag";
    public Material MutedMaterial;
    private Material UnMutedMaterial;
    private Renderer rend;
    private bool muted;

    void Start()
    {
        rend = GetComponent<Renderer>();
        UnMutedMaterial = rend.material;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(HandTag))
        {
            if (ButtonNumber <= 0 || ButtonNumber > PhotonNetwork.PlayerList.Length) return;

            LB.MutePress(ButtonNumber);

            muted = !muted;
            rend.material = muted ? MutedMaterial : UnMutedMaterial;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(HandTag))
            rend.material = UnMutedMaterial;
    }
}