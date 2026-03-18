using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem; // CallbackContext用
using UnityEngine.Events;

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

    protected override void Awake()
    {
        base.Awake(); // CharacterBaseの取得を呼ぶ
        _interact = GetComponent<PlayerInteract>();
    }

    public override void OnNetworkSpawn()
    {
        // 自分が操作するプレイヤー以外なら、このスクリプトとカメラを無効化
        if (!IsOwner)
        {
            if (_cameraRoot != null) _cameraRoot.gameObject.SetActive(false);
            enabled = false;
            return;
        }

        // オーナーのみマウスロック
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected virtual void Update()
    {
        if (!IsOwner || Cursor.lockState != CursorLockMode.Locked) return;

        HandleRotation();
        
        float targetSpeed = (_isSprinting && _moveInput.y > 0) ? _sprintSpeed : _walkSpeed;
        Vector3 moveDir = transform.right * _moveInput.x + transform.forward * _moveInput.y;

        // 肉体（Base）に命令を出す
        ApplyMovement(moveDir, targetSpeed, _isSprinting);
    }

    private void HandleRotation()
    {
        transform.Rotate(Vector3.up * _lookInput.x * _mouseSensitivity);
        _xRotation -= _lookInput.y * _mouseSensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -_lookXLimit, _lookXLimit);
        if (_cameraRoot != null) _cameraRoot.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }

    // --- Input System Events ---
    public void OnMove(InputAction.CallbackContext context) => _moveInput = context.ReadValue<Vector2>();
    public void OnLook(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>();

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed) _isSprinting = true;
        if (context.canceled) _isSprinting = false;
    }
}