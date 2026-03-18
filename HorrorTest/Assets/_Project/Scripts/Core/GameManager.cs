using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    // どこからでもアクセスできる「自分自身」の分身
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // もし既に誰か（別のGameManager）がいたら、自分は消える（重複防止）
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // 自分を登録する
        Instance = this;
        
        // シーン移動しても破壊されないようにする（必要なら）
        DontDestroyOnLoad(gameObject);
    }

    public void GameOver()
    {
        Debug.Log("ゲームオーバー処理");
    }

    // 勝利判定フラグ（サーバーのみ管理）
    private bool isGameFinished = false;

    // UI Managerの参照（全クライアントで持つ）
    [SerializeField] private UIManager uiManager;

    public override void OnNetworkSpawn()
    {
        // UI Managerの参照を取得（インスペクターでセットしても良い）
        if (uiManager == null) uiManager = Object.FindFirstObjectByType<UIManager>();
    }

    // サーバーが勝利を確定させ、全員に通知するRPC
    [Rpc(SendTo.Everyone)] // 全員に送る
    public void BroadcastVictoryRpc(ulong winnerClientId)
    {
        // クライアント側の処理
        if (uiManager != null)
        {
            //uiManager.ShowVictoryScreen(winnerClientId);
        }
    }
}