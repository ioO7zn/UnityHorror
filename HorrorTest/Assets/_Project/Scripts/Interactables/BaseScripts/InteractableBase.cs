using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Unity.Netcode;

public abstract class InteractableBase : NetworkBehaviour, IInteractable
{
    [Header("基本設定 (親クラス)")]
    [SerializeField] protected string textMessage = "None";
    
    [SerializeField] protected InputActionReference interactAction;

    [Header("イベント")]
    [SerializeField] protected UnityEvent onInteract;

    private bool _isBusy = false;

    public virtual void Interact()
    {
        if (IsBusy) return;
        onInteract?.Invoke();
    }

    
    public string GetInteractText()
    {
        // キー名を取得
        string keyName = interactAction != null ? interactAction.action.GetBindingDisplayString() : "None";

        string message = textMessage;

        return $"[{keyName}] {message}";
    }

    public InputAction GetInteractAction()
    {
        return interactAction.action;
    }


    //触れるか触れないかを返す
    public bool CanInteract => !_isBusy;

    protected bool IsBusy
    {
        get => _isBusy;
        set => _isBusy = value;
    }


}