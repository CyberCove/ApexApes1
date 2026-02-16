using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class TagPurchase : MonoBehaviour
{
    [Header("COSMETICS")]
    public GameObject enable;
    public GameObject disable;

    [Header("BUY")]
    [Tooltip("Type the cosmetic tag name manually here")]
    public string cosmeticTag;

    public int coinsPrice;
    public Playfablogin playfablogin;

    private void Start()
    {
        if (PlayerPrefs.GetInt(cosmeticTag, 0) == 1)
        {
            enable.SetActive(true);
            disable.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("HandTag"))
            return;

        if (PlayerPrefs.GetInt(cosmeticTag, 0) == 1)
            return;

        if (playfablogin.coins < coinsPrice)
        {
            Debug.Log("Not enough coins!");
            return;
        }

        BuyItem();
    }

    void BuyItem()
    {
        var request = new SubtractUserVirtualCurrencyRequest
        {
            VirtualCurrency = "AD",
            Amount = coinsPrice
        };

        PlayFabClientAPI.SubtractUserVirtualCurrency(
            request,
            OnSubtractCoinsSuccess,
            OnError
        );
    }

    void OnSubtractCoinsSuccess(ModifyUserVirtualCurrencyResult result)
    {
        PlayerPrefs.SetInt(cosmeticTag, 1);
        PlayerPrefs.Save();

        enable.SetActive(true);
        disable.SetActive(true);
        gameObject.SetActive(false);

        Playfablogin.instance.GetVirtualCurrencies();
    }

    void OnError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
}
