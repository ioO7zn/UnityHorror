using Unity.Netcode;
using UnityEngine;

public class LobbyPlayerState : NetworkBehaviour
{
    // 準備完了フラグ
    public NetworkVariable<bool> IsReady = new NetworkVariable<bool>(false);
    
    // サーバーが決める役割（0: 未定, 1: サバイバー, 2: ハンター）
    public NetworkVariable<int> PlayerRole = new NetworkVariable<int>(0);

    [Header("見た目の切り替え用")]
    [SerializeField] private GameObject soulModel;     // 魂（球体）
    [SerializeField] private GameObject humanModel;    // 人間モデルの親

    public override void OnNetworkSpawn()
    {
        // シーンが切り替わった時に見た目を更新する設定
        NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
        RefreshAppearance();
    }

    // 準備完了ボタンから呼ばれるRPC
    [Rpc(SendTo.Server)]
    public void ToggleReadyServerRpc()
    {
        IsReady.Value = !IsReady.Value;
        // サーバーに「全員チェックして！」と頼む
        Object.FindFirstObjectByType<LobbyManager>().CheckAllPlayersReady();
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted)
        {
            RefreshAppearance();
        }
    }

    public void RefreshAppearance()
    {
        // シーン名が "Lobby" なら魂、それ以外なら人間を表示
        bool isLobby = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "LobbyScene";
        soulModel.SetActive(isLobby);
        humanModel.SetActive(!isLobby);
    }
}