using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabNameManager : MonoBehaviour
{
    // Static instance allows other scripts to find this easily
    public static PlayFabNameManager Instance;

    [Header("UI References")]
    public TMP_InputField nameInputField;
    public TextMeshProUGUI statusText;

    private void Awake()
    {
        // Singleton logic: ensures this persists across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GetAccountInfo()
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnError);
    }

    private void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        string displayName = result.AccountInfo.TitleInfo.DisplayName;

        // If we are in a scene that has the input field assigned
        if (nameInputField != null && !string.IsNullOrEmpty(displayName))
        {
            nameInputField.text = displayName;
        }
    }

    public void SubmitDisplayName()
    {
        if (nameInputField == null) return;

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nameInputField.text
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }

    private void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        if (statusText != null) statusText.text = "Name Saved!";
    }

    private void OnError(PlayFabError error)
    {
        if (statusText != null) statusText.text = "Error: " + error.ErrorMessage;
    }
}