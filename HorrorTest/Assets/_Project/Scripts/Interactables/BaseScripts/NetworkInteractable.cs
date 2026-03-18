using UnityEngine;
using Unity.Netcode;

public class NetworkInteractable : InteractableBase
{
    public override void Interact()
    {
        if (!IsSpawned) 
        {
            Debug.LogWarning($"{gameObject.name} はまだネットワークにSpawnされていないため、インタラクトできません。");
            return; 
        }
        
        RequestInteractRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestInteractRpc(RpcParams rpcParams = default)
    {
        // 通信を送ってきたプレイヤーのIDを取得
        ulong senderId = rpcParams.Receive.SenderClientId;
        
        //全員に見た目を同期する「前」に、サーバー専用の処理を実行する
        OnServerInteract(senderId);

        ExecuteInteractRpc();
    }

    protected virtual void OnServerInteract(ulong clientId)
    {
        // デフォルトでは何もしない（ドアなどはそのままEveryoneへ流れる）
    }

    [Rpc(SendTo.Everyone)]
    private void ExecuteInteractRpc()
    {
        PerformVisualAction();
    }

    private void PerformVisualAction()
    {
        base.Interact();
    }
}