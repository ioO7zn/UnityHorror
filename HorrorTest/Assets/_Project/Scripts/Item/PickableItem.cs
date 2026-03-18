using UnityEngine;
using Unity.Netcode;

public class PickableItem : NetworkInteractable 
{
    [Header("このアイテムのデータ")]
    public ItemData data;

    protected override void OnServerInteract(ulong clientId)
    {
        if (!IsServer || data == null) return;

        // 1. サーバー側でプレイヤーを特定
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            if (client.PlayerObject.TryGetComponent<PlayerInventory>(out var inventory))
            {
                // 2. インベントリに追加
                inventory.AddItem(data.itemID);
                
                // 3. ネットワークから消去（Despawnが先！）
                if (NetworkObject.IsSpawned)
                {
                    NetworkObject.Despawn(true);
                }
            }
        }
    }
}