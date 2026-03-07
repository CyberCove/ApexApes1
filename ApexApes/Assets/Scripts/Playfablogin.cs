using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

public class Playfablogin : MonoBehaviourPunCallbacks
{
    [Header("COSMETICS")]
    public static Playfablogin instance;
    public string MyPlayFabID;
    public string CatalogName;
    public List<GameObject> specialitems;
    public List<GameObject> disableitems;

    [Header("CURRENCY")]
    public string CurrencyName;
    public TextMeshPro currencyText;
    [SerializeField] public int coins;

    [Header("BANNED")]
    public string bannedSceneName;

    [Header("TITLE DATA")]
    public TextMeshPro MOTDText;

    [Header("PLAYER DATA")]
    public TextMeshPro UserName;
    public TextMeshPro PlayFabIDText;
    public string StartingUsername;
    public string name;
    [SerializeField] public bool UpdateName;

    public void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(InitializeWithDelay());
    }

    IEnumerator InitializeWithDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.Log("Already logged in, retrieving data...");
            OnLoginSuccess(null);
        }
        else
        {
            Debug.Log("Logging in to PlayFab...");
            Login();
        }
    }

    public void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnError);
    }

    public void OnLoginSuccess(LoginResult result)
    {
        if (result != null)
        {
            MyPlayFabID = result.PlayFabId;
            Debug.Log("PlayFab ID: " + MyPlayFabID);
        }

        // Update UI
        if (PlayFabIDText != null)
            PlayFabIDText.text = "PlayFab ID: " + MyPlayFabID;

        // Sync PlayFab ID to Photon
        SyncPhotonPlayFabID();

        // Get account info and inventory
        GetAccountInfo();
        GetVirtualCurrencies();
        GetMOTD();
    }

    void SyncPhotonPlayFabID()
    {
        if (!string.IsNullOrEmpty(MyPlayFabID) && PhotonNetwork.IsConnected)
        {
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash["PlayFabID"] = MyPlayFabID;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            Debug.Log("Synced PlayFab ID to Photon: " + MyPlayFabID);
        }
    }

    public override void OnJoinedRoom()
    {
        // Sync ID whenever joining a room
        SyncPhotonPlayFabID();
    }

    public void GetAccountInfo()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), AccountInfoSuccess, OnError);
    }

    public void AccountInfoSuccess(GetAccountInfoResult result)
    {
        MyPlayFabID = result.AccountInfo.PlayFabId;
        Debug.Log("Got account info - PlayFab ID: " + MyPlayFabID);

        if (PlayFabIDText != null)
            PlayFabIDText.text = "PlayFab ID: " + MyPlayFabID;

        // Apply inventory for cosmetics
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            inventoryResult =>
            {
                foreach (var item in inventoryResult.Inventory)
                {
                    if (item.CatalogVersion == CatalogName)
                    {
                        for (int i = 0; i < specialitems.Count; i++)
                            if (specialitems[i].name == item.ItemId)
                                specialitems[i].SetActive(true);

                        for (int i = 0; i < disableitems.Count; i++)
                            if (disableitems[i].name == item.ItemId)
                                disableitems[i].SetActive(false);
                    }
                }
            },
            error =>
            {
                Debug.LogError(error.GenerateErrorReport());
            });
    }

    public void GetVirtualCurrencies()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetUserInventorySuccess, OnError);
    }

    void OnGetUserInventorySuccess(GetUserInventoryResult result)
    {
        if (result.VirtualCurrency.TryGetValue("AD", out int currencyAmount))
            coins = currencyAmount;
        else
            coins = 0;

        if (currencyText != null)
            currencyText.text = "You have " + coins + " " + CurrencyName;
    }

    public void GetMOTD()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), MOTDGot, OnError);
    }

    public void MOTDGot(GetTitleDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("MOTD"))
            MOTDText.text = result.Data["MOTD"];
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("PlayFab Error: " + error.GenerateErrorReport());
        if (error.Error == PlayFabErrorCode.AccountBanned)
            SceneManager.LoadScene(bannedSceneName);
    }
}