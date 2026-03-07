using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Networking;

public class ReportButton : MonoBehaviour
{
    public int ButtonNumber;
    public LeaderBoard LB;
    public string HandTag = "HandTag";
    public Material PressedMaterial;
    private Material UnPressedMaterial;
    private Renderer rend;
    private bool reported;

    void Start()
    {
        rend = GetComponent<Renderer>();
        UnPressedMaterial = rend.material;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(HandTag) && !reported)
        {
            if (ButtonNumber <= 0 || ButtonNumber > PhotonNetwork.PlayerList.Length) return;

            Photon.Realtime.Player reportedPlayer = PhotonNetwork.PlayerList[ButtonNumber - 1];
            Photon.Realtime.Player reporterPlayer = PhotonNetwork.LocalPlayer;

            string reportedID = reportedPlayer.CustomProperties.ContainsKey("PlayFabID") 
                ? reportedPlayer.CustomProperties["PlayFabID"].ToString() : "Unknown";

            string reporterID = reporterPlayer.CustomProperties.ContainsKey("PlayFabID") 
                ? reporterPlayer.CustomProperties["PlayFabID"].ToString() : "Unknown";

            string message = $"🚨 PLAYER REPORTED 🚨\n" +
                             $"Reporter: {reporterPlayer.NickName}\n" +
                             $"Reporter PlayFabID: {reporterID}\n" +
                             $"Reported Player: {reportedPlayer.NickName}\n" +
                             $"Reported PlayFabID: {reportedID}\n" +
                             $"Room: {PhotonNetwork.CurrentRoom?.Name ?? "Unknown"}";

            LB.SendToWebhook(message);

            rend.material = PressedMaterial;
            reported = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(HandTag))
        {
            rend.material = UnPressedMaterial;
            reported = false;
        }
    }
}