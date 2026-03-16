using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public abstract class CharacterBase : NetworkBehaviour
{
    [Header("共通ステータス")]
    public NetworkVariable<int> Health = new NetworkVariable<int>(100);
    [SerializeField] protected float _walkSpeed = 5f;
    [SerializeField] protected float _sprintSpeed = 8f;
    [SerializeField] protected float _gravity = -9.81f;

    [Header("共通参照")]
    protected CharacterController _controller;
    protected Animator _animator;
    protected Vector3 _velocity;

    protected virtual void Awake()
    {
        _controller = GetComponent<CharacterController>();
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
    }

    // ★重要: 物理移動の実行部分だけをメソッド化
    // これを呼べば人間でもAIでも「歩く」という物理現象が起きる
    protected void ApplyMovement(Vector3 moveDir, float targetSpeed, bool isSprinting)
    {
        // 接地判定
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        // 1. 水平移動
        _controller.Move(moveDir * targetSpeed * Time.deltaTime);

        // 2. 垂直移動（重力）
        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);

        // 3. アニメーション（肉体が動いた結果として更新）
        UpdateAnimation(moveDir.magnitude, isSprinting);
    }

    private void UpdateAnimation(float magnitude, bool isSprinting)
    {
        if (_animator == null) return;
        float speedParam = magnitude > 0.1f ? (isSprinting ? 2.0f : 1.0f) : 0f;
        _animator.SetFloat("Speed", speedParam, 0.1f, Time.deltaTime);
    }

    // 全キャラ共通のダメージ処理
    public virtual void TakeDamage(int amount)
    {
        if (IsServer) Health.Value -= amount;
    }
}