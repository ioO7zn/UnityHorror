using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem; 
using UnityEngine.Events;
using Unity.Netcode; // 追加
using UnityEngine.SceneManagement; // シーン判定に必要

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerCC : CharacterBase
{
    [Header("プレイヤー専用参照")]
    [SerializeField] private Transform _cameraRoot;
    [SerializeField] private float _mouseSensitivity = 0.1f;
    [SerializeField] private float _lookXLimit = 80f;

    protected PlayerInteract _interact;
    protected Vector2 _moveInput;
    protected Vector2 _lookInput;
    protected bool _isSprinting;
    private float _xRotation = 0f;

    private bool _isInputLocked = false;

    public void SetInputLock(bool isLocked)
    {
        _isInputLocked = isLocked;
    }

    protected override void Awake()
    {
        base.Awake();
        _interact = GetComponent<PlayerInteract>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            if (_cameraRoot != null) _cameraRoot.gameObject.SetActive(false);
            enabled = false;
            return;
        }

        // シーン切り替えイベントを登録
        NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;

        // 最初の状態をチェック（ロビーかゲームか）
        UpdateControlState();
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        // シーンロード完了時に状態を更新
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted)
        {
            UpdateControlState();
        }
    }

    private void UpdateControlState()
    {
        if (!IsOwner) return;

        // 現在のシーンがロビーかどうか判定
        bool isLobby = SceneManager.GetActiveScene().name == "LobbyScene";

        if (isLobby)
        {
            // ロビー：動かさない
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // 入力値をリセットして、Updateでの移動を止める
            _moveInput = Vector2.zero;
            _lookInput = Vector2.zero;
        }
        else
        {
            // ゲームシーン：動かせる
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    protected virtual void Update()
    {
        // ロビーなら処理を中断（これでキー入力を無視できる）
        if (!IsOwner || SceneManager.GetActiveScene().name == "LobbyScene") return;
        
        // 通常のゲーム中処理
        if (Cursor.lockState != CursorLockMode.Locked) return;

        HandleRotation();
        
        float targetSpeed = (_isSprinting && _moveInput.y > 0) ? _sprintSpeed : _walkSpeed;
        Vector3 moveDir = transform.right * _moveInput.x + transform.forward * _moveInput.y;

        ApplyMovement(moveDir, targetSpeed, _isSprinting);
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
        }
    }

    // --- 以下、HandleRotation や Input Events はそのまま ---
    private void HandleRotation()
    {
        transform.Rotate(Vector3.up * _lookInput.x * _mouseSensitivity);
        _xRotation -= _lookInput.y * _mouseSensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -_lookXLimit, _lookXLimit);
        if (_cameraRoot != null) _cameraRoot.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }

    public void OnMove(InputAction.CallbackContext context) => _moveInput = context.ReadValue<Vector2>();
    public void OnLook(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>();

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed) _isSprinting = true;
        if (context.canceled) _isSprinting = false;
    }
}