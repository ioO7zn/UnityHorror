using UnityEngine;
using Unity.Netcode;

public class NetworkInteractable : InteractableBase
{
    public override void OnInteract()
    {
        // 【重要】Spawnされていない（ネットワーク同期が始まっていない）ならRPCを送らない
        if (!IsSpawned) 
        {
            Debug.LogWarning($"{gameObject.name} はまだネットワークにSpawnされていないため、インタラクトできません。");
            return; 
        }

        RequestInteractRpc();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestInteractRpc()
    {
        ExecuteInteractRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void ExecuteInteractRpc()
    {
        PerformVisualAction();
    }

    protected virtual void PerformVisualAction()
    {
        base.OnInteract();
    }
}