using UnityEngine;
using Unity.Netcode;


public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _survivorPrefab;
    [SerializeField] private GameObject _hunterPrefab;

    public override void OnNetworkSpawn()
    {
        // サーバーだけが生成権限を持つ
        if (!IsServer) return;

        // 新しいクライアントが接続したときに呼ばれるイベントを登録
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
    }

    private void SpawnPlayer(ulong clientId)
    {
        // ここでロジックを決める（例：最初の1人はハンター、それ以外はサバイバー）
        GameObject prefabToSpawn = (clientId == 1) ? _hunterPrefab : _survivorPrefab;

        // インスタンス化
        GameObject playerInstance = Instantiate(prefabToSpawn);

        // 重要：NetworkObjectとしてスポーンさせ、所有権(clientId)を渡す
        var networkObject = playerInstance.GetComponent<NetworkObject>();
        networkObject.SpawnAsPlayerObject(clientId);
    }
}