using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using System.Threading.Tasks;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using TMPro;

public class Playfablogin : MonoBehaviour
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
    [SerializeField]
    public int coins;
    [Header("BANNED")]
    public string bannedscenename;
    [Header("TITLE DATA")]
    public TextMeshPro MOTDText;
    [Header("PLAYER DATA")]
    public TextMeshPro UserName;
    public TextMeshPro PlayFabIDText;
    public string StartingUsername;
    public string name;
    [SerializeField]
    public bool UpdateName;
    



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
            Debug.Log("Playfablogin: Already logged in, retrieving data...");
            OnLoginSuccess(null);
        }
        else
        {
            Debug.Log("Playfablogin: Not logged in yet, attempting login...");
            login();
        }
    }

    public void login()
    {
        Debug.Log("Playfablogin: Starting login...");
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
            Debug.Log("Playfablogin: Login successful! PlayFab ID: " + result.PlayFabId);
        }
        else
        {
            Debug.Log("Playfablogin: Using existing PlayFab session");
        }
        
        GetAccountInfoRequest InfoRequest = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(InfoRequest, AccountInfoSuccess, OnError);
        GetVirtualCurrencies();
        GetMOTD();
    }

    public void AccountInfoSuccess(GetAccountInfoResult result)
    {
        MyPlayFabID = result.AccountInfo.PlayFabId;
        Debug.Log("Playfablogin: Got account info - PlayFab ID: " + MyPlayFabID);
        
        if (PlayFabIDText != null)
        {
            PlayFabIDText.text = "PlayFab ID: " + MyPlayFabID;
            Debug.Log("Playfablogin: Updated PlayFabIDText");
        }
        else
        {
            Debug.LogWarning("Playfablogin: PlayFabIDText is not assigned!");
        }

        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
        (result) =>
        {
            foreach (var item in result.Inventory)
            {
                if (item.CatalogVersion == CatalogName)
                {
                    for (int i = 0; i < specialitems.Count; i++)
                    {
                        if (specialitems[i].name == item.ItemId)
                        {
                            specialitems[i].SetActive(true);
                        }
                    }
                    for (int i = 0; i < disableitems.Count; i++)
                    {
                        if (disableitems[i].name == item.ItemId)
                        {
                            disableitems[i].SetActive(false);
                        }
                    }
                }
            }
        },
        (error) =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    async void Update()
    { 
    }

    public void GetVirtualCurrencies()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetUserInventorySuccess, OnError);
    }

    void OnGetUserInventorySuccess(GetUserInventoryResult result)
    {
        Debug.Log("Playfablogin: Got inventory. Available currencies: " + string.Join(", ", result.VirtualCurrency.Keys));
        
        if (result.VirtualCurrency.TryGetValue("AD", out int currencyAmount))
        {
            coins = currencyAmount;
            if (currencyText != null)
            {
                currencyText.text = "You have " + coins.ToString() + " " + CurrencyName;
                Debug.Log("Playfablogin: Updated currency text to: " + currencyText.text);
            }
            else
            {
                Debug.LogWarning("Playfablogin: currencyText is not assigned!");
            }
        }
        else
        {
            Debug.LogWarning("Playfablogin: Currency 'AD' not found in PlayFab account. Available currencies: " + string.Join(", ", result.VirtualCurrency.Keys));
            if (currencyText != null)
            {
                currencyText.text = "0 " + CurrencyName;
            }
        }
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Playfablogin: PlayFab Error - " + error.GenerateErrorReport());
        
        if (error.Error == PlayFabErrorCode.AccountBanned)
        {
            SceneManager.LoadScene(bannedscenename);
        }
    }
    //Get TitleData

    public void GetMOTD()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), MOTDGot, OnError);
    }

    public void MOTDGot(GetTitleDataResult result)
    {
        if (result.Data == null || result.Data.ContainsKey("MOTD") == false)
        {
            Debug.Log("No MOTD");
            return;
        }
        MOTDText.text = result.Data["MOTD"];
        
    }


}