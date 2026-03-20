using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ServerAutoLauncher : MonoBehaviour
{
    void Start()
    {
        // サーバー専用ビルドとして起動した場合
        if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
        {
            // 1. サーバーを開始
            NetworkManager.Singleton.StartServer();
            
            // 2. 自動でロビーシーンへ切り替える（NetworkSceneManagerを使用）
            // これにより、後から入ってくるクライアントも自動的にロビーに飛ばされます
            NetworkManager.Singleton.SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
            
            Debug.Log("Server: 自動でロビーシーンへ移動しました。");
        }
    }
}