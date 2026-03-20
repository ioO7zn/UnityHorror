using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System.Collections;

public class LobbySpawningHandler : MonoBehaviour
{
    [Header("スポーン地点の親オブジェクト（子に地点を並べる）")]
    [SerializeField] private Transform _spawnPointsParent;
    private Transform[] _spawnPoints;

    private void Start()
    {
        // インスペクターで指定した親の子要素を地点として取得
        if (_spawnPointsParent != null)
        {
            _spawnPoints = new Transform[_spawnPointsParent.childCount];
            for (int i = 0; i < _spawnPointsParent.childCount; i++)
            {
                _spawnPoints[i] = _spawnPointsParent.GetChild(i);
            }
        }

        if (!NetworkManager.Singleton.IsServer) return;

        // 1. これから入ってくる人のためにイベント登録
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        // 2. 【重要】すでに接続されている人（ホスト自身）を今すぐ配置
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            OnClientConnected(client.ClientId);
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // サーバー側で配置を実行
        StartCoroutine(DelayedPositioning(clientId));
    }

    private IEnumerator DelayedPositioning(ulong clientId)
    {
        // オブジェクトが生成され、NetworkObjectとして認識されるまで待つ
        yield return new WaitUntil(() => 
            NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId) && 
            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject != null
        );

        var playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        
        // 何番目の接続者か計算
        int index = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.ClientId == clientId) break;
            index++;
        }

        if (_spawnPoints != null && _spawnPoints.Length > index)
        {
            Transform target = _spawnPoints[index];

            // CharacterControllerの干渉防止
            var cc = playerObject.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // 物理的な位置移動
            playerObject.transform.position = target.position;
            playerObject.transform.rotation = target.rotation;

            // NetworkTransformへの強制ワープ通知（(0,0,0)戻り防止）
            var nt = playerObject.GetComponent<NetworkTransform>();
            if (nt != null)
            {
                nt.Teleport(target.position, target.rotation, playerObject.transform.localScale);
            }

            if (cc != null) cc.enabled = true;

            Debug.Log($"[Lobby] Player {clientId} を地点 {index} に配置しました。");
        }
    }
}