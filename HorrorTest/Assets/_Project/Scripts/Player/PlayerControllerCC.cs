using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerCC : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float walkSpeed = 5f;      // 通常速度
    [SerializeField] private float sprintSpeed = 8f;    // ダッシュ速度
    [SerializeField] private float gravity = -9.81f;

    [Header("視点設定（Look）")]
    [SerializeField] private Transform playerCamera; 
    [SerializeField] private float mouseSensitivity = 10f; 
    [SerializeField] private float lookXLimit = 80f; 

    // 内部変数
    private CharacterController _controller;
    private InputActions _inputActions; // 生成されたクラス
    private Vector2 _moveInput;
    private Vector2 _lookInput; 
    private Vector3 _velocity;
    private bool _isGrounded;
    private float _xRotation = 0f; 
    private float _currentSpeed; // 現在の速度を保持する変数

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        // InputActionsのインスタンスを作成
        _inputActions = new InputActions();
    }


    //このスクリプトが有効か無効か
    private void OnEnable() => _inputActions.Enable();
    private void OnDisable() => _inputActions.Disable();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // 1. 入力を取得
        _moveInput = _inputActions.Player.Move.ReadValue<Vector2>();
        _lookInput = _inputActions.Player.Look.ReadValue<Vector2>();


        // 1. スプリントボタンが押されている
        // 2. かつ、スティック/キー入力が「前方向（y > 0）」である
        if (_inputActions.Player.Sprint.IsPressed() && _moveInput.y > 0)
        {
            _currentSpeed = sprintSpeed;
        }
        else
        {
            _currentSpeed = walkSpeed;
        }
        

        _isGrounded = _controller.isGrounded;

        // ------------------------------
        // 視点操作（Look）の処理
        // ------------------------------
        transform.Rotate(Vector3.up * _lookInput.x * mouseSensitivity * Time.deltaTime);

        _xRotation -= _lookInput.y * mouseSensitivity * Time.deltaTime;
        _xRotation = Mathf.Clamp(_xRotation, -lookXLimit, lookXLimit);

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }

        // ------------------------------
        // 移動操作（Move）の処理
        // ------------------------------
        if (_controller == null || !_controller.enabled) 
        {
            return; 
        }

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        
        // ★ _currentSpeed を使って移動
        _controller.Move(move * _currentSpeed * Time.deltaTime);

        // 重力
        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}