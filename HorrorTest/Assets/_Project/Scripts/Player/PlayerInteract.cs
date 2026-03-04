using UnityEngine;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine.InputSystem; // CallbackContext用

public class PlayerInteract : NetworkBehaviour
{
    [Header("参照 (Prefab内で完結)")]
    [SerializeField] private CinemachineCamera _playerCamera;
    
    [Header("設定")]
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _interactLayer;

    private IInteractable _currentTarget;
    private RaycastHit _currentHit;

    public override void OnNetworkSpawn()
    {
        // 自分が操作するプレイヤー以外なら、このスクリプト自体を停止
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        // UIの初期状態などはUIManager側で管理するため、ここでは何もしなくてOK
    }

    void Update()
    {
        // OnNetworkSpawnで制限しているので、ここは常にIsOwner
        UpdateRay();

        // UIManagerに報告（回転や表示判断はUI側で自律的に行う）
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateUI(_currentTarget, _currentHit);
        }
    }

    private void UpdateRay()
    {
        _currentTarget = null;
        
        // カメラの正面へレイを飛ばす
        Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, _interactLayer))
        {
            // インターフェースの取得は TryGetComponent が安全で効率的
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                if (interactable.CanInteract)
                {
                    _currentTarget = interactable;
                    _currentHit = hit;
                }
            }
        }
    }

    /// <summary>
    /// Player Input コンポーネントの Events -> Interact から 
    /// Dynamic (InputAction.CallbackContext) で繋ぐ
    /// </summary>
    public void OnInteract(InputAction.CallbackContext context)
    {
        // ボタンが押された瞬間のみ実行
        if (context.started && _currentTarget != null)
        {
            _currentTarget.Interact();
            Debug.Log($"{_currentHit.collider.name} とインタラクトした！");
        }
    }
}