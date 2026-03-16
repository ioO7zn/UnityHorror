using UnityEngine;
using Unity.Netcode;

public class PickableItem : NetworkInteractable 
{
    public ItemData data; 

    // UnityEventから呼ばれる関数
    public void RequestPickup() 
    {
        // 1. 【重要】消す前にインベントリに追加する
        // 通信ラグを考慮し、操作した本人（LocalClient）の画面で即座に実行します
        var localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (localPlayer != null && localPlayer.TryGetComponent<PlayerInventory>(out var inventory))
        {
            inventory.AddItem(data);
            Debug.Log($"<color=green>[Inventory]</color> {data.itemName} を追加しました");
        }

        // 2. サーバーへ消去を依頼
        if (IsServer)
        {
            ExecuteDespawn();
        }
        else
        {
            RequestDespawnRpc();
        }
    }

    // 修正ポイント：[Rpc] 属性を付与し、InvokePermissionを設定
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestDespawnRpc()
    {
        ExecuteDespawn();
    }

    private void ExecuteDespawn()
    {
        if (NetworkObject != null && NetworkObject.IsSpawned)
        {
            // 修正ポイント：警告対策。第1引数(destroy)をfalseに設定
            // これにより「In-scene network objects」の警告を回避し、安全にネットワークから切り離します
            NetworkObject.Despawn(false);
            
            // ネットワークから消えた後、メモリからも削除する
            Destroy(gameObject); 
        }
    }
}