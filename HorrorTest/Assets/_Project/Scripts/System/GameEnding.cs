using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameEnding : MonoBehaviour
{
    [Header("画面を覆う白い画像")]
    public Image whiteFadePanel;

    [Header("クリア画面のUI（最初は隠しておくやつ）")]
    public GameObject clearScreenUI;

    [Header("フェードアウトの速さ")]
    public float fadeSpeed = 0.5f;

    [Header("プレイヤー")]
    public PlayerControllerCC playerScript;

    private bool isEnding = false;
    private bool isCleared = false;

    void OnTriggerEnter(Collider other)
    {
        // プレイヤーがゴールに触れた瞬間
        if (other.CompareTag("Player") && !isEnding)
        {
            isEnding = true;
            StopPlayerAction(other.gameObject);
        }
    }

    private void StopPlayerAction(GameObject player)
    {
        // 1. 移動を物理的に止める
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // 2. 視点移動と移動スクリプトを止める
        if (playerScript != null) playerScript.enabled = false;

        // 3. Eキーなどのインタラクトを止める（ロッカーやドアに触れなくする）
        PlayerInteract interact = player.GetComponent<PlayerInteract>();
        if (interact != null)
        {
            interact.enabled = false;
            // 画面に出ていた「[E] 操作する」などのUIも消す
            //if (interact.interactMessageObj != null) interact.interactMessageObj.SetActive(false);
        }

        // 4. マウスカーソルを自由に動かせるようにする
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // 1. エンディング演出中（フェード中）
        if (isEnding && !isCleared)
        {
            Color currentColor = whiteFadePanel.color;
            // 0(透明)から1(真っ白)へなめらかに変化させる
            currentColor.a = Mathf.MoveTowards(currentColor.a, 1f, fadeSpeed * Time.deltaTime);
            whiteFadePanel.color = currentColor;

            if (currentColor.a >= 1.0f)
            {
                ShowClearScreen();
            }
        }

        // 2. 完全に白くなってクリア画面が出た後
        if (isCleared)
        {
            // 何かキーが押されたらタイトルへ
            if (AnyInputDown())
            {
                SceneManager.LoadScene("TitleScene");
            }
        }
    }

    private bool AnyInputDown()
    {
        // Input Systemを使ってあらゆるデバイスの入力を検知
        return  (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) ||
                (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame);
    }

    void ShowClearScreen()
    {
        isCleared = true;
        if (clearScreenUI != null)
        {
            clearScreenUI.SetActive(true);
        }
    }
}