using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private UIManager uIManager;
    [Header("設定")]
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private InputActionReference _interactAction;


    private IInteractable currentTarget;  // 視線の先にあるもの
    private RaycastHit currentHit;

    private void OnEnable()
    {
        if (_interactAction != null) _interactAction.action.Enable();
    }

    private void OnDisable()
    {
        if (_interactAction != null) _interactAction.action.Disable();
    }

    void Update()
    {
        UpdateRay();
        uIManager.UpdateUI(currentTarget, currentHit); //UIに渡す
        HandleInput();

    }

    

    private void UpdateRay()
    {
        currentTarget = null;

        Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);


        if(Physics.Raycast(ray, out RaycastHit hit, _interactDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if(interactable != null && interactable.CanInteract)
            {
                currentTarget = interactable;
                currentHit = hit; //UI用にヒット情報をキャッシュ
            }
        }
    }

    private void HandleInput()
    {
        if(currentTarget != null && _interactAction.action.WasPressedThisFrame())
        {
            currentTarget.OnInteract();
        }
    }

}