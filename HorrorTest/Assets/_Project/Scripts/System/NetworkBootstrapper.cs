using UnityEngine;
using Unity.Netcode;

public class NetworkBootstrapper : MonoBehaviour
{
    void Start()
    {
        #if UNITY_SERVER
            Debug.Log("--- 専用サーバーモードで起動中 ---");
            if (!NetworkManager.Singleton.StartServer())
            {
                Debug.LogError("専用サーバーの起動に失敗しました");
            }
        #else
            Debug.Log("--- クライアント/ホストモードで起動中 ---");
            // 通常起動では UI から StartHost/StartClient で接続を制御
            // テスト用かつ自動接続したい場合はコメント解除
            //NetworkManager.Singleton.StartClient();
        #endif
    }
}