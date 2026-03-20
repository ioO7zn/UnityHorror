using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
public class LobbyManager : NetworkBehaviour
{
    public void CheckAllPlayersReady()
    {
        if (!IsServer) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject.TryGetComponent<LobbyPlayerState>(out var state))
            {
                if (!state.IsReady.Value) return; // 一人でも準備中なら中断
            }
        }

        // 全員完了！シーン移動を開始
        Debug.Log("全員準備完了！ゲームを開始します。");
        NetworkManager.Singleton.SceneManager.LoadScene("MapScene", LoadSceneMode.Single);
    }
}