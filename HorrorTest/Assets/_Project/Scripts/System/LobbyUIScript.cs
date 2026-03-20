using UnityEngine;
using Unity.Netcode;

public class LobbyUIScript : MonoBehaviour
{
    // ボタンからこのメソッドを呼ぶように設定する
    public void OnClickReady()
    {
        // 1. 自分のプレイヤーオブジェクトを探す
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.LocalClient != null)
        {
            var myPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;

            if (myPlayer != null)
            {
                // 2. その中にある LobbyPlayerState を取得して RPC を実行！
                var state = myPlayer.GetComponent<LobbyPlayerState>();
                state.ToggleReadyServerRpc();
                
                Debug.Log("自分の準備完了をサーバーに送りました！");
            }
        }
    }
}