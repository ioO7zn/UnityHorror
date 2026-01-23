using UnityEngine;
using UnityEngine.SceneManagement;
// using UnityEngine.UI; // Sliderを使わなくなったので不要ですが、消さなくてもエラーにはなりません
using System.Collections;

public class TitleManager : MonoBehaviour
{
    [Header("UI設定")]
    public GameObject loadingScreen; // ロード画面のパネル全体（背景の黒幕など）
    
    [Header("回転させるキューブ")]
    public GameObject loadingCube;   // ★追加：右下で回るオブジェクト（Imageなど）
    public float rotateSpeed = 300f; // ★追加：回転の速さ

    void Start()
    {
        // カーソル設定（前回と同じ）
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // 最初はロード画面を隠しておく
        if(loadingScreen != null) loadingScreen.SetActive(false);
    }

    public void OnStartButtonClicked()
    {
        StartCoroutine(LoadSceneAsync("MapScene")); // ※シーン名は自分の環境に合わせてね
    }

    // ★追加：終了ボタン用
    public void OnExitButtonClicked()
    {
        Debug.Log("ゲームを終了します");

        #if UNITY_EDITOR
            // Unityエディタで再生中の場合、再生を停止する
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // 本番のアプリ（exe）の場合、アプリを閉じる
            Application.Quit();
        #endif
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        // 1. ロード画面（とキューブ）を表示する
        if(loadingScreen != null) loadingScreen.SetActive(true);

        // 2. 非同期でシーン読み込みを開始
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // 3. 読み込みが終わるまでループ
        while (!operation.isDone)
        {
            // ★ここを変更！
            // 進捗バーの計算はやめて、単にキューブを回す
            if (loadingCube != null)
            {
                // Z軸（手前奥の軸）を中心に時計回りに回転させる
                // ※もし3Dの箱をY軸で回したいなら Vector3.up に変えてください
                loadingCube.transform.Rotate(0, 0, -rotateSpeed * Time.deltaTime);
            }

            yield return null; // 1フレーム待つ
        }
    }


}