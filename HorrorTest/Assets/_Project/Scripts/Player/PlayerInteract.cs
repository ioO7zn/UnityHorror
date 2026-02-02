using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using Unity.Netcode;

public class PlayerInteract : NetworkBehaviour
{
    [Header("参照")]
    [SerializeField] private CinemachineCamera _playerCamera;
    [SerializeField] private UIManager _uiManager; // 直接アタッチするか自動取得する
    
    [Header("設定")]
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _interactLayer;

    [SerializeField] private InputActionReference _interactAction;

    private IInteractable currentTarget;
    private RaycastHit currentHit;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            // 自分以外ならUIごと非表示にする
            if (_uiManager != null && _uiManager.mainCanvas != null)
            {
                _uiManager.mainCanvas.gameObject.SetActive(false);
            }
            enabled = false;
            return;
        }

        // 自分の場合はUIを確実に表示
        if (_uiManager != null && _uiManager.mainCanvas != null)
        {
            _uiManager.mainCanvas.gameObject.SetActive(true);
            _uiManager.mainCanvas.enabled = true;
        }

        // アクション自動取得
        if (_interactAction == null)
        {
            var playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                var action = playerInput.actions.FindAction("Interact");
                if (action != null) _interactAction = InputActionReference.Create(action);
            }
        }
    }

    private void OnEnable()
    {
        if (IsOwner && _interactAction != null) _interactAction.action.Enable();
    }

    private void OnDisable()
    {
        if (IsOwner && _interactAction != null) _interactAction.action.Disable();
    }

    void Update()
    {
        // 念押しでIsOwnerチェック（これだけで混線は100%防げる）
        if (!IsOwner) return;

        UpdateRay();

        if (_uiManager != null)
        {
            // 自分のカメラの回転を渡す
            _uiManager.UpdateUI(currentTarget, currentHit, _playerCamera.transform.rotation);
        }

        HandleInput();
    }

    private void UpdateRay()
    {
        currentTarget = null;
        Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, _interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.CanInteract)
            {
                currentTarget = interactable;
                currentHit = hit;
            }
        }
    }

    private void HandleInput()
    {
        if (currentTarget != null && _interactAction != null && _interactAction.action.WasPressedThisFrame())
        {
            currentTarget.OnInteract();
        }
    }
}