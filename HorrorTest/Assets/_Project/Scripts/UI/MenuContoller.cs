using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private GameObject menuUI;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private bool _isMenuOpen = true; // 最初は表示
    private InputActions _inputActions;

    private void Awake()
    {
        _inputActions = new InputActions();
        
        // ESCキーでメニュー切り替え（参加後のみ有効にしたい場合は後述のロジック）
        _inputActions.Player.Menu.performed += _ => HandleMenuInput();

        // ボタンに機能を登録
        hostButton.onClick.AddListener(OnHostButtonClicked);
        clientButton.onClick.AddListener(OnClientButtonClicked);
    }

    private void Start()
    {
        // 最初はメニューを表示し、カーソルを自由にする
        OpenMenu();
    }

    private void OnEnable() => _inputActions.Enable();
    private void OnDisable() => _inputActions.Disable();

    // ESCキーが押された時の処理
    private void HandleMenuInput()
    {
        // ネットワークが動いていない（まだ参加していない）ときは、ESCで閉じさせない
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) return;

        if (_isMenuOpen) CloseMenu();
        else OpenMenu();
    }

    private void OnHostButtonClicked()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            CloseMenu();
        }
    }

    private void OnClientButtonClicked()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        _isMenuOpen = true;
        menuUI.SetActive(true);
        SetCursorState(false);
    }

    public void CloseMenu()
    {
        _isMenuOpen = false;
        menuUI.SetActive(false);
        SetCursorState(true);
    }

    private void SetCursorState(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }
}