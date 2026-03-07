using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Pun;
using Photon.Realtime;

public class LeaderBoard : MonoBehaviourPun
{
    [Header("UI")]
    public TMPro.TMP_Text[] displaySpot;
    public Renderer[] ColorSpot;

    [Header("Discord")]
    public string WebHookURL;

    private bool Kicked = false;

    void Update()
    {
        for (int i = 0; i < displaySpot.Length; i++)
        {
            if (i < PhotonNetwork.PlayerList.Length)
            {
                if (!Kicked)
                    displaySpot[i].text = PhotonNetwork.PlayerList[i].NickName;
                else
                {
                    displaySpot[i].text = "You have been Kicked";
                    displaySpot[i].color = Color.red;
                }
            }
            else
            {
                displaySpot[i].text = "";
                if (i < ColorSpot.Length) ColorSpot[i].material.color = Color.white;
            }
        }
    }

    public void MutePress(int buttonNumber)
    {
        if (buttonNumber <= 0 || buttonNumber > PhotonNetwork.PlayerList.Length) return;

        foreach (var p in FindObjectsOfType<Photon.Pun.PhotonView>())
        {
            if (p.Owner == PhotonNetwork.PlayerList[buttonNumber - 1])
            {
                var speaker = p.GetComponent<Photon.Voice.PUN.PhotonVoiceView>()?.SpeakerInUse;
                if (speaker != null)
                    speaker.mute = !speaker.mute;
            }
        }
    }

    public void KickPress(int buttonNumber)
    {
        if (buttonNumber <= 0 || buttonNumber > PhotonNetwork.PlayerList.Length) return;

        foreach (var p in FindObjectsOfType<Photon.Pun.PhotonView>())
        {
            if (p.Owner == PhotonNetwork.PlayerList[buttonNumber - 1])
            {
                photonView.RPC("KickPlayer", p.Owner);
            }
        }
    }

    [PunRPC]
    void KickPlayer()
    {
        Kicked = true;
    }

    public void Report(int buttonNumber)
    {
        if (buttonNumber <= 0 || buttonNumber > PhotonNetwork.PlayerList.Length) return;

        Photon.Realtime.Player reported = PhotonNetwork.PlayerList[buttonNumber - 1];
        string reportedID = "Unknown";
        if (reported.CustomProperties.ContainsKey("PlayFabID"))
            reportedID = reported.CustomProperties["PlayFabID"].ToString();

        SendToWebhook($"Reported Player: {reported.NickName} ({reportedID})");
    }

    public void SendToWebhook(string message)
    {
        StartCoroutine(PostToDiscord(message));
    }

    IEnumerator PostToDiscord(string message)
    {
        string jsonPayload = "{\"content\":\"" + message + "\"}";
        UnityWebRequest www = new UnityWebRequest(WebHookURL, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            Debug.LogError("Discord Webhook Error: " + www.error);
    }
}