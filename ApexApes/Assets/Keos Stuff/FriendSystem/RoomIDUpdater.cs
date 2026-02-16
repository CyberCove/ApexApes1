using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class RoomIDUpdater : MonoBehaviour
{
    public float UpdateRatio = 60;

    private void Awake()
    {
        StartCoroutine(SetRoomID());
    }

    private IEnumerator SetRoomID()
    {
        SetRoomIDForPlayer(PhotonNetwork.IsConnected ? PhotonNetwork.CurrentRoom.Name : "None");
        yield return new WaitForSeconds(UpdateRatio);
        StartCoroutine(SetRoomID());
    }
    void SetRoomIDForPlayer(string roomID)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "RoomID", roomID }
            }
        };

        PlayFabClientAPI.UpdateUserData(request, OnSetRoomIDSuccess, OnPlayFabError);
    }

    void OnSetRoomIDSuccess(UpdateUserDataResult result)
    {
        Debug.Log("RoomID set successfully.");
    }

    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("Error setting RoomID: " + error.GenerateErrorReport());
    }
}
