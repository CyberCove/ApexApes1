using PlayFab.ClientModels;
using PlayFab;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FriendingManager : MonoBehaviour
{
    [Header("Limit Friends ?")]
    public bool LimitFriends = false;
    public int MaxFriends = 6;

    public List<FriendSlot> FriendSlots = new List<FriendSlot>();

    public TextSetting FriendName;
    public TextSetting RoomName;

    [System.Serializable]
    public class TextSetting
    {
        public string FrontText;
        public string BackText;
    }

    [System.Serializable]
    public class FriendSlot
    {
        public TextMeshPro NameDisplay;

        [Header("SOON")]
        public TextMeshPro RoomDisplay;
    }

    //priv stuff
    [HideInInspector]
    public int friendCount = 0; //just there for info lol its not used anymore but it used to, wait i have an usage for it in mind lol let me code that rel quick
    int pageIDX = 0;

    private void Start()
    {
        RefreshFriendList();
    }

    public void RefreshFriendList()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(), OnGetFriendsListSuccess, OnPlayFabError);
    }

    private void OnGetFriendsListSuccess(GetFriendsListResult result)
    {
        friendCount = result.Friends.Count;

        for (int i = 0; i < FriendSlots.Count; i++)
        {
            if (friendCount > i)
            {
                FriendSlots[i].NameDisplay.text = FriendName.FrontText + result.Friends[i].TitleDisplayName + FriendName.BackText;
                StartCoroutine(UpdateRoomID(result.Friends[i].FriendPlayFabId, FriendSlots[i].RoomDisplay));
            }
            else
            {
                FriendSlots[i].NameDisplay.text = FriendName.FrontText + "None" + FriendName.BackText;
                FriendSlots[i].RoomDisplay.text = RoomName.FrontText + "None" + RoomName.BackText;
            }
        }
    }

    private IEnumerator UpdateRoomID(string playfabID, TextMeshPro roomDisplay)
    {
        string roomID = string.Empty;

        PlayFabClientAPI.GetUserData(new GetUserDataRequest { PlayFabId = playfabID },
            result =>
            {
                if (result.Data != null)
                {
                    if (result.Data.ContainsKey("RoomID"))
                    {
                        roomID = RoomName.FrontText + result.Data["RoomID"].Value + RoomName.BackText;
                        roomDisplay.text = roomID;
                    }
                }
            },
            error =>
            {
                Debug.LogError("Error getting Player Data: " + error.GenerateErrorReport());
            });

        yield return null;
    }


    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("Error retrieving friend list: " + error.GenerateErrorReport());
    }

    //editor
    [HideInInspector]
    public string friendtoaddid = "";

    public void AddFriendByID(string IDToAdd)
    {
        var request = new AddFriendRequest { FriendPlayFabId = IDToAdd };
        PlayFabClientAPI.AddFriend(request, OnFriendAdded, OnError);
    }

    private void OnFriendAdded(AddFriendResult result)
    {
        RefreshFriendList();
        Debug.Log("Friend added successfully!");
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error adding friend: " + error.ErrorMessage);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(FriendingManager))]
public class FriendingManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FriendingManager script = (FriendingManager)target;

        if (GUILayout.Button("Refresh"))
        {
            script.RefreshFriendList();
        }

        GUILayout.Label("Adding Friends Manualy");

        script.friendtoaddid = GUILayout.TextField(script.friendtoaddid);

        if (!string.IsNullOrEmpty(script.friendtoaddid))
        {
            if (GUILayout.Button("Add Friend"))
            {
                script.AddFriendByID(script.friendtoaddid);
            }
        }
    }
}

#endif