using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem; 
using UnityEngine.Events;
using Unity.Netcode;
using Unity.Netcode.Components; // NetworkTransform 用
using UnityEngine.SceneManagement; 

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkTransform))] // NetworkTransform 必須
public class PlayerControllerCC : CharacterBase
{
    private const string LobbySceneName = "LobbyScene";

    [Header("プレイヤー専用参照")]
    [SerializeField] private Transform _cameraRoot;
    [SerializeField] private float _mouseSensitivity = 0.1f;
    [SerializeField] private float _lookXLimit = 80f;

    protected PlayerInteract _interact;
    protected Vector2 _moveInput;
    protected Vector2 _lookInput;
    protected bool _isSprinting;
    private float _xRotation = 0f;
    public struct PlayerNetworkState : INetworkSerializable
    {
        public Vector2 Movement;
        public bool Sprinting;
        public float RawYaw;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Movement);
            serializer.SerializeValue(ref Sprinting);
            serializer.SerializeValue(ref RawYaw);
        }
    }

    private NetworkTransform _networkTransform;
    private PlayerNetworkState _latestState;
    private float _inputSendInterval = 0.016f; // 60Hz程度で送信
    private float _inputSendTimer = 0f;
    private float _localYaw;

    private bool _isInputLocked = false;


    protected override void Awake()
    {
        base.Awake();
        _interact = GetComponent<PlayerInteract>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        _networkTransform = GetComponent<NetworkTransform>();
        _latestState.RawYaw = transform.eulerAngles.y;
        _localYaw = transform.eulerAngles.y;

        if (_cameraRoot != null)
        {
            _cameraRoot.gameObject.SetActive(IsOwner);
        }

        // 純クライアントの他人プレイヤーだけ入力系を停止する。
        // サーバーでは IsOwner ではなくても移動処理が必要なので無効化しない。
        if (!IsOwner && !IsServer)
        {
            enabled = false;
            return;
        }

        if (IsOwner)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
            UpdateControlState();
        }
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted)
        {
            UpdateControlState();
        }
    }

    private void UpdateControlState()
    {
        if (!IsOwner) return;

        if (IsLobbyScene())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ResetGameplayInput();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    protected virtual void Update()
    {
        if (!IsGameplayActive()) return;

        if (IsOwner)
        {
            HandleOwnerInput();
        }
    }

    private void FixedUpdate()
    {
        if (!IsGameplayActive()) return;

        if (IsServer)
        {
            HandleServerMovement();
        }
    }

    private void HandleOwnerInput()
    {
        if (!IsOwner) return;
        if (_isInputLocked) return;
        if (!IsGameplayActive()) return;

        HandleLocalLook();

        _inputSendTimer += Time.deltaTime;
        if (_inputSendTimer >= _inputSendInterval)
        {
            _inputSendTimer = 0f;
            SubmitInputRpc(_moveInput, _isSprinting, _localYaw);
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void SubmitInputRpc(Vector2 move, bool sprint, float yaw)
    {
        _latestState.Movement = move;
        _latestState.Sprinting = sprint;
        _latestState.RawYaw = yaw;
    }

    private void HandleLocalLook()
    {
        _localYaw += _lookInput.x * _mouseSensitivity;
        _xRotation -= _lookInput.y * _mouseSensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -_lookXLimit, _lookXLimit);

        // オーナーはローカルで即座に横視点を反映する。
        transform.rotation = Quaternion.Euler(0f, _localYaw, 0f);

        if (_cameraRoot != null)
        {
            _cameraRoot.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }
    }

    private void HandleServerMovement()
    {
        if (!IsServer) return;

        transform.rotation = Quaternion.Euler(0f, _latestState.RawYaw, 0f);

        float targetSpeed = (_latestState.Sprinting && _latestState.Movement.y > 0) ? _sprintSpeed : _walkSpeed;
        Vector3 moveDir = transform.right * _latestState.Movement.x + transform.forward * _latestState.Movement.y;

        ApplyMovement(moveDir, targetSpeed, _latestState.Sprinting);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (IsOwner && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
        }
    }

    public void SetInputLock(bool isLocked)
    {
        _isInputLocked = isLocked;
    }

    private bool IsLobbyScene()
    {
        return SceneManager.GetActiveScene().name == LobbySceneName;
    }

    private bool IsGameplayActive()
    {
        return !IsLobbyScene();
    }

    private void ResetGameplayInput()
    {
        _moveInput = Vector2.zero;
        _lookInput = Vector2.zero;
        _isSprinting = false;
        _inputSendTimer = 0f;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOwner || !IsGameplayActive()) return;
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (!IsOwner || !IsGameplayActive()) return;
        _lookInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (!IsOwner || !IsGameplayActive()) return;
        if (context.performed) _isSprinting = true;
        if (context.canceled) _isSprinting = false;
    }
}