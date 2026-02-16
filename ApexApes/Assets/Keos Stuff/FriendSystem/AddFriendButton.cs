using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

public class AddFriendButton : MonoBehaviourPunCallbacks
{
    public PhotonView PTView;
    public string HandTag = "HandTag";

    public FriendingManager Manager;

    public float AddFriendsTime = 3f;
    public TextMeshPro TimeDisplay;

    string MyID = "";
    List<string> PlayfabIDs = new List<string>();
    bool IsHoldingDown = false;
    float elapsedTime = 0;
    Coroutine addFriendsCoroutine;

    private void Awake()
    {
        GetPlayfabID();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(HandTag) && !string.IsNullOrEmpty(MyID))
        {
            if (Manager.MaxFriends > Manager.friendCount && Manager.LimitFriends)
                return;

            PTView.RPC(nameof(AddIDToList), RpcTarget.AllBuffered, MyID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(HandTag) && !string.IsNullOrEmpty(MyID))
        {
            PTView.RPC(nameof(RemoveIDFromList), RpcTarget.AllBuffered, MyID);
        }
    }

    [PunRPC]
    public void AddIDToList(string ID)
    {
        if (!PlayfabIDs.Contains(ID))
        {
            PlayfabIDs.Add(ID);
        }

        if (PlayfabIDs.Count >= 2 && !IsHoldingDown)
        {
            IsHoldingDown = true;
            addFriendsCoroutine = StartCoroutine(AddFriends());
        }
    }

    [PunRPC]
    public void RemoveIDFromList(string ID)
    {
        if (PlayfabIDs.Contains(ID))
        {
            PlayfabIDs.Remove(ID);
        }

        if (PlayfabIDs.Count < 2 && IsHoldingDown)
        {
            IsHoldingDown = false;
            elapsedTime = 0;
            if (addFriendsCoroutine != null)
            {
                StopCoroutine(addFriendsCoroutine);
                TimeDisplay.text = "Hold To Add Friends";
            }
        }
    }

    private IEnumerator AddFriends()
    {
        elapsedTime = 0;
        while (elapsedTime < AddFriendsTime)
        {
            elapsedTime += Time.deltaTime;
            TimeDisplay.text = (AddFriendsTime - elapsedTime).ToString("F1") + "s";
            yield return null;
        }

        foreach (string ID in PlayfabIDs)
        {
            if (ID != MyID)
            {
                Manager.AddFriendByID(ID);
            }
        }

        TimeDisplay.text = "Friends Added!";
        yield return new WaitForSeconds(1.5f);
        TimeDisplay.text = "Hold To Add Friends";
    }

    private void GetPlayfabID()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
            result => MyID = result.AccountInfo.PlayFabId,
            error => Debug.LogError("Failed Getting ID: " + error.ErrorMessage));
    }
}
