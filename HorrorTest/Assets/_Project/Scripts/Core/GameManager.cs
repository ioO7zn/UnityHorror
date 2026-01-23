using UnityEngine;

public class GameManager : MonoBehaviour
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
}