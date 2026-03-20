using UnityEngine;
using Unity.Netcode;

public class MenuController : NetworkBehaviour
{
    [SerializeField] private GameObject _menuPanel;
    private bool _isMenuOpen = false;

    // 自分のプレイヤーコントローラーへの参照
    private PlayerControllerCC _playerController;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _playerController = GetComponent<PlayerControllerCC>();
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        _isMenuOpen = !_isMenuOpen;

        // UIとカーソルの制御
        if (_menuPanel != null) _menuPanel.SetActive(_isMenuOpen);
        
        Cursor.lockState = _isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isMenuOpen;

        // --- 重要: プレイヤーの入力を止める/再開する ---
        if (_playerController != null)
        {
            _playerController.SetInputLock(_isMenuOpen);
        }
    }
}