using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerInventory : NetworkBehaviour
{
    [Header("全てのアイテムデータを登録（全プレイヤー共通）")]
    [SerializeField] private List<ItemData> allItemDatabase = new List<ItemData>();

    [SerializeField] private List<int> victoryItemIds = new List<int> { 1, 2, 3 };

    // 内部的なIDリスト（サーバーが管理し、Ownerに同期される）
    
    private NetworkList<int> itemIds = new NetworkList<int>();

    // 表示用のリスト（UIなどで使用）
    public List<ItemData> currentItems = new List<ItemData>();
    public System.Action OnInventoryChanged;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = Object.FindFirstObjectByType<GameManager>();
    }

    private void Awake()
    {
        // NetworkListの初期化（ここで行うのが最も安全）
        itemIds = new NetworkList<int>(
            null,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server // サーバーだけが書き込める
        );
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            itemIds.OnListChanged += (_) => RefreshLocalList();
            RefreshLocalList();
        }
    }

    // サーバーがアイテムを追加する窓口
    public void AddItem(int id)
    {
        if (!IsServer) return;
        itemIds.Add(id);

        CheckVictoryCondition();
    }

    // ★今回のメイン：捨てる実行窓口
    public void DoDrop(int index = -1)
    {
        if (!IsOwner) return;

        // 指定がなければ最後を捨てる
        int targetIndex = (index == -1) ? currentItems.Count - 1 : index;

        if (targetIndex >= 0 && targetIndex < currentItems.Count)
        {
            // サーバーに「この番号を消して実体を出して」と頼む
            DropItemServerRpc(targetIndex);
        }
    }

    [Rpc(SendTo.Server)]
    private void DropItemServerRpc(int index)
    {
        if (index < 0 || index >= itemIds.Count) return;

        int idToDrop = itemIds[index];
        var data = allItemDatabase.Find(d => d.itemID == idToDrop);

        if (data != null && data.worldPrefab != null)
        {
            // 1. 先に実体を生成して Spawn する
            Vector3 spawnPos = transform.position + (transform.forward * 1.5f) + (Vector3.up * 0.8f);
            GameObject droppedObj = Instantiate(data.worldPrefab, spawnPos, Quaternion.identity);
            
            if (droppedObj.TryGetComponent<PickableItem>(out var pickable))
            {
                pickable.data = data;
            }
            
            droppedObj.GetComponent<NetworkObject>().Spawn(true);

            // 2. 「最後」にリストから消す
            itemIds.RemoveAt(index);
        }
    }

    private void RefreshLocalList()
    {
        currentItems.Clear();
        foreach (int id in itemIds)
        {
            var data = allItemDatabase.Find(d => d.itemID == id);
            if (data != null) currentItems.Add(data);
        }
        OnInventoryChanged?.Invoke();
    }

    public List<ItemData> GetItemList() => currentItems;

    private void CheckVictoryCondition()
    {
        if (!IsServer || gameManager == null) return;

        // 現在のインベントリに、勝利に必要なIDが全て含まれているか確認
        bool hasAllKeys = true;
        foreach (int requiredId in victoryItemIds)
        {
            if (!itemIds.Contains(requiredId))
            {
                hasAllKeys = false;
                break;
            }
        }

        // 揃っていたら、GameManagerに通知
        if (hasAllKeys)
        {
            Debug.Log($"[Server] プレイヤー {OwnerClientId} が勝利条件を達成しました！");
            gameManager.BroadcastVictoryRpc(OwnerClientId); // GameManagerの勝利RPCを呼ぶ
        }
    }


}