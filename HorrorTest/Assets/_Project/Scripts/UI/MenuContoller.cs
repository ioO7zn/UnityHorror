using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MenuController : NetworkBehaviour
{
    private GameObject _menuCanvas; 
    private bool _isMenuOpen = false;
    private PlayerControllerCC _player;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _player = GetComponent<PlayerControllerCC>();

            _menuCanvas = GameObject.Find("MenuCanvas");

            if (_menuCanvas != null)
            {
                _menuCanvas.SetActive(false); 
            }
            else
            {
                Debug.LogError("ヒエラルキーに 'MenuCanvas' が見つかりません！名前が一致しているか確認してください。");
            }
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
        }
        else
        {
            this.enabled = false;
        }
    }


    public void ToggleMenu()
    {
        if (!IsOwner || SceneManager.GetActiveScene().name == "LobbyScene") return;
        if (_menuCanvas == null) return;

        _isMenuOpen = !_isMenuOpen;
        
        _menuCanvas.SetActive(_isMenuOpen);

        Cursor.lockState = _isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = _isMenuOpen;

        if (_player != null)
        {
            _player.SetInputLock(_isMenuOpen);
        }
    }
}