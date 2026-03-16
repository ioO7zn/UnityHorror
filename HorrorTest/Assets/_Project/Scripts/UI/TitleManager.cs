using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode; // Netcodeを使うために追加

public class TitleManager : MonoBehaviour
{
    [Header("UI設定")]
    public GameObject loadingScreen; 
    
    [Header("回転させるキューブ")]
    public GameObject loadingCube;   
    public float rotateSpeed = 300f; 

    private bool _isLoading = false;

    void Start()
    {
        // カーソル設定
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        if(loadingScreen != null) loadingScreen.SetActive(false);
    }

    void Update()
    {
        // ロード画面が表示されている間、キューブを回し続ける
        if (_isLoading && loadingCube != null)
        {
            loadingCube.transform.Rotate(0, 0, -rotateSpeed * Time.deltaTime);
        }
    }

    // --- Host（部屋を作る）ボタンにアサイン ---
    public void OnHostButtonClicked()
    {
        if (_isLoading) return;
        _isLoading = true;

        if(loadingScreen != null) loadingScreen.SetActive(true);

        // 1. まずHostとして起動
        if (NetworkManager.Singleton.StartHost())
        {
            // 2. Netcode専用のシーンマネージャーでシーンを切り替える
            // これにより、接続してくるClientも自動的にこのシーンへ飛ばされます
            NetworkManager.Singleton.SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
        }
        else
        {
            _isLoading = false;
            if(loadingScreen != null) loadingScreen.SetActive(false);
            Debug.LogError("ホストの起動に失敗しました");
        }
    }

    // --- Client（参加する）ボタンにアサイン ---
    public void OnClientButtonClicked()
    {
        if (_isLoading) return;
        _isLoading = true;

        if(loadingScreen != null) loadingScreen.SetActive(true);

        // クライアントとして起動。接続が成功すればサーバーが呼んだシーンへ自動で飛ばされます。
        if (!NetworkManager.Singleton.StartClient())
        {
            _isLoading = false;
            if(loadingScreen != null) loadingScreen.SetActive(false);
            Debug.LogError("クライアントの起動に失敗しました");
        }
    }

    public void OnExitButtonClicked()
    {
        Debug.Log("ゲームを終了します");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}