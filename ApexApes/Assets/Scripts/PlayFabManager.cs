using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Normal.GorillaTemplate
{
    public class PlayFabManager : MonoBehaviour
    {
        public static PlayFabManager Instance { get; private set; }

        private static readonly string __configureMessage =
            "PlayFab is not configured. Assign your Title ID using \"PlayFab > MakePlayFabSharedSettings\" in the Unity Editor.";

        private Task __initializationTask;
        private Dictionary<string, List<CatalogItem>> __catalogs = new();
        private Dictionary<string, int> __virtualCurrencies = new();
        private List<ItemInstance> __inventory = new List<ItemInstance>();

        public bool isConfigured => !string.IsNullOrWhiteSpace(PlayFabSettings.TitleId);
        public bool isLoggedIn => PlayFabClientAPI.IsClientLoggedIn();
        public bool isBanned { get; private set; } = false;

        public delegate void CatalogUpdatedHandler(string catalog, List<CatalogItem> items);
        public event CatalogUpdatedHandler onCatalogChanged;

        public delegate void CurrencyBalanceUpdatedHandler(string currencyCode, int newBalance);
        public event CurrencyBalanceUpdatedHandler onCurrencyBalanceChanged;

        public delegate void InventoryUpdatedHandler(List<ItemInstance> items);
        public event InventoryUpdatedHandler onInventoryChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _ = InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            if (__initializationTask != null) await __initializationTask;
            else __initializationTask = DoInitializeAsync();
            await __initializationTask;
        }

        private async Task DoInitializeAsync()
        {
            if (!isConfigured)
            {
                Debug.LogWarning(__configureMessage);
                return;
            }

            string deviceId = SystemInfo.deviceUniqueIdentifier;
            var loginTcs = new TaskCompletionSource<bool>();

            PlayFabClientAPI.LoginWithCustomID(
                new LoginWithCustomIDRequest
                {
                    CustomId = deviceId,
                    CreateAccount = true
                },
                result =>
                {
                    Debug.Log(result.NewlyCreated
                        ? $"Logged into PlayFab as new user {result.PlayFabId}"
                        : $"Logged into PlayFab as existing user {result.PlayFabId}");

                    _ = RefreshCatalogAsync();
                    _ = RefreshInventoryAsync();
                    loginTcs.SetResult(true);
                },
                error =>
                {
                    if (error.Error == PlayFabErrorCode.AccountBanned) isBanned = true;
                    Debug.LogError($"Failed to login to PlayFab: {error.GenerateErrorReport()}");
                    loginTcs.SetResult(false);
                }
            );

            await loginTcs.Task;
            __initializationTask = null;
        }

        public int GetCurrencyBalance(string currencyCode) => __virtualCurrencies.GetValueOrDefault(currencyCode, 0);

        public List<ItemInstance> GetInventory() => __inventory;

        public bool IsItemInInventory(string itemId) => __inventory.Exists(item => item.ItemId == itemId);

        public async Task<List<CatalogItem>> GetCatalogAsync(string catalog = null)
        {
            if (string.IsNullOrWhiteSpace(catalog)) catalog = string.Empty;
            if (!__catalogs.TryGetValue(catalog, out var items)) await RefreshCatalogAsync(catalog);
            __catalogs.TryGetValue(catalog, out items);
            return items ?? new List<CatalogItem>();
        }

        public async Task<CatalogItem> GetItemFromCatalogAsync(string itemId, string catalog = null)
        {
            if (string.IsNullOrWhiteSpace(itemId)) return null;
            var items = await GetCatalogAsync(catalog);
            return items.FirstOrDefault(item => item.ItemId == itemId);
        }

        public async Task RefreshCatalogAsync(string catalog = null)
        {
            if (!CheckConfiguredAndLoggedIn("Cannot refresh catalog: ")) return;
            if (string.IsNullOrWhiteSpace(catalog)) catalog = null;

            var tcs = new TaskCompletionSource<bool>();

            PlayFabClientAPI.GetCatalogItems(
                new GetCatalogItemsRequest { CatalogVersion = catalog },
                result =>
                {
                    catalog ??= string.Empty;
                    __catalogs[catalog] = result.Catalog;
                    onCatalogChanged?.Invoke(catalog, result.Catalog);
                    tcs.SetResult(true);
                },
                error =>
                {
                    Debug.LogError($"Failed to refresh catalog {catalog}: {error.GenerateErrorReport()}");
                    tcs.SetResult(false);
                }
            );

            await tcs.Task;
        }

        public async Task RefreshInventoryAsync()
        {
            if (!CheckConfiguredAndLoggedIn("Cannot refresh inventory: ")) return;

            var tcs = new TaskCompletionSource<bool>();

            PlayFabClientAPI.GetUserInventory(
                new GetUserInventoryRequest(),
                result =>
                {
                    __virtualCurrencies = result.VirtualCurrency;
                    __inventory = result.Inventory;

                    foreach (var currency in __virtualCurrencies)
                        onCurrencyBalanceChanged?.Invoke(currency.Key, currency.Value);

                    onInventoryChanged?.Invoke(__inventory);
                    tcs.SetResult(true);
                },
                error =>
                {
                    Debug.LogError($"Failed to refresh inventory: {error.GenerateErrorReport()}");
                    tcs.SetResult(false);
                }
            );

            await tcs.Task;
        }

        public async Task<bool> AddCurrencyAsync(string currencyCode, int amount)
        {
            if (!CheckConfiguredAndLoggedIn("Cannot add currency: ")) return false;

            var tcs = new TaskCompletionSource<bool>();

            PlayFabClientAPI.AddUserVirtualCurrency(
                new AddUserVirtualCurrencyRequest
                {
                    VirtualCurrency = currencyCode,
                    Amount = amount
                },
                result =>
                {
                    __virtualCurrencies[currencyCode] = result.Balance;
                    onCurrencyBalanceChanged?.Invoke(currencyCode, result.Balance);
                    tcs.SetResult(true);
                },
                error =>
                {
                    Debug.LogError($"Failed to add {amount} {currencyCode}: {error.GenerateErrorReport()}");
                    tcs.SetResult(false);
                }
            );

            return await tcs.Task;
        }

        private bool CheckConfiguredAndLoggedIn(string prefix)
        {
            if (!isConfigured) { Debug.LogError(prefix + __configureMessage); return false; }
            if (!isLoggedIn) { Debug.LogError(prefix + "User not logged in."); return false; }
            return true;
        }
    }
}
