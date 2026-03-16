using UnityEngine;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine.InputSystem; // CallbackContext用
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerCC : NetworkBehaviour
{
    [Header("参照 (Prefab内で完結)")]
    [SerializeField] private Transform _cameraRoot;
    [SerializeField] private Animator _animator; // <-- 1. 宣言を追加
    
    [Header("移動設定")]
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 8f;
    [SerializeField] private float _gravity = -9.81f;

    [Header("視点設定")]
    [SerializeField] private float _mouseSensitivity = 0.1f;
    [SerializeField] private float _lookXLimit = 80f;

    [Header("外部通知 (UI等へ)")]
    public UnityEvent<int> OnSlotSelected;

    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private bool _isSprinting;
    
    private float _xRotation = 0f;
    private Vector3 _velocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        // <-- 2. 自動的に子オブジェクトからAnimatorを探す設定を追加
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        // 自分が操作するプレイヤー以外なら、このスクリプト自体を停止
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



    void Update()
    {
        // OnNetworkSpawnで制限済みのため、常にIsOwner
        if (Cursor.lockState != CursorLockMode.Locked) return;
        

        HandleRotation();
        HandleMovement();
    }

    // --- Unityの Player Input (Events) から紐付けるメソッド群 ---

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed) _isSprinting = true;
        if (context.canceled) _isSprinting = false;
    }

    public void OnSlot(InputAction.CallbackContext context)
    {
        // 1~4などのキー入力を想定。context.control.name等からIndexを抽出するか、
        // 単純にActionを分けてインスペクターで 0, 1, 2... と引数を指定する
        if (context.started)
        {
            // インスペクター側で引数(int)を直接指定する場合は不要だが
            // 汎用的に使うならここで処理を分岐
        }
    }

    // --- 内部ロジック ---

    private void HandleRotation()
    {
        // 左右：本体を回転
        transform.Rotate(Vector3.up * _lookInput.x * _mouseSensitivity);

        // 上下：カメラ（Cinemachine）を回転
        _xRotation -= _lookInput.y * _mouseSensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -_lookXLimit, _lookXLimit);
        
        if (_cameraRoot != null)
        {
            _cameraRoot.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }
    }

    private void HandleMovement()
    {
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        float currentSpeed = (_isSprinting && _moveInput.y > 0) ? _sprintSpeed : _walkSpeed;
        Vector3 moveDir = transform.right * _moveInput.x + transform.forward * _moveInput.y;

        _controller.Move(moveDir * currentSpeed * Time.deltaTime);

        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        // --- 3. ここに Animator への橋渡しを追加 ---
        if (_animator != null)
        {
            // 入力の絶対量（0～1）をスピードとして渡す
            // これにより、歩き・走りのアニメーションが自動で切り替わる
            float inputMagnitude = _moveInput.magnitude;
            
            // もし「走っている」なら値を少し大きくして走るモーションを強調しても良い
            float animationSpeed = _isSprinting ? inputMagnitude * 2.0f : inputMagnitude;
            
            _animator.SetFloat("Speed", animationSpeed);
        }

        
    }
}