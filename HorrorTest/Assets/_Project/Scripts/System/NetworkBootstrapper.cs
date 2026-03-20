using UnityEngine;
using Unity.Netcode;

public class NetworkBootstrapper : MonoBehaviour
{
    void Start()
    {
        // 専用サーバービルドとして実行された場合、自動でサーバーを開始
        #if UNITY_SERVER
                Debug.Log("--- サーバー専用モードで起動中 ---");
                NetworkManager.Singleton.StartServer();
        #else
                // エディタや普通のビルドなら、手動で Client ボタンを押すか、
                // テスト用に自動で Client を開始させる
                Debug.Log("--- クライアントモードで起動中 ---");
                // 自動で繋ぎたいなら: NetworkManager.Singleton.StartClient();
        #endif
    }
}