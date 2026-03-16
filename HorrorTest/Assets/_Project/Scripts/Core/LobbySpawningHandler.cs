using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbySpawningHandler : MonoBehaviour
{
    [Header("立ち位置のリスト（入室順）")]
    [SerializeField] private Transform[] _spawnPoints;

    private void Start()
    {
        // プレイヤーが生成される（Spawnされる）イベントを購読する
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        
        // すでに接続済みのプレイヤー（ホスト自身など）がいれば配置する
        InitialPositioning();
    }

    private void OnDestroy()
    {
        // 忘れずにイベントを解除
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        // 新しいプレイヤーが入ってきたら配置を実行
        PositionPlayer(clientId);
    }

    private void InitialPositioning()
    {
        // シーン移動直後に、自分を含め既に存在するプレイヤーを配置
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            PositionPlayer(client.ClientId);
        }
    }

    private void PositionPlayer(ulong clientId)
    {
        // 1. そのIDのプレイヤーオブジェクトを探す
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
        {
            var playerObject = networkClient.PlayerObject;
            if (playerObject != null)
            {
                // 2. IDに基づいてスポーン地点を選択 (0番なら地点0、1番なら地点1...)
                int index = (int)clientId % _spawnPoints.Length;
                Transform targetPoint = _spawnPoints[index];

                // 3. 移動させる
                playerObject.transform.position = targetPoint.position;
                playerObject.transform.rotation = targetPoint.rotation;
                
                Debug.Log($"Player {clientId} を地点 {index} に配置しました");
            }
        }
    }
}