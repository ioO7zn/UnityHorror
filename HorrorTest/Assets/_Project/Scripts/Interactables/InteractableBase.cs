using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Unity.Netcode;

public abstract class InteractableBase : NetworkBehaviour, IInteractable
{
    [Header("基本設定 (親クラス)")]
    [SerializeField] protected string textMessage = "None_SO";
    
    [SerializeField] protected InputActionReference interactAction;

    [Header("イベント")]
    [SerializeField] protected UnityEvent onInteract;

    [Header("設定データ (SO)")]
    [SerializeField] private InteractionDataSO _data;

    private bool _isBusy = false;

    public virtual void OnInteract()
    {
        if (IsBusy) return;
        onInteract?.Invoke();
    }

    
    public string GetInteractText()
    {
        // キー名を取得
        string keyName = interactAction != null ? interactAction.action.GetBindingDisplayString() : "None";

        // SOがあればその文字を、なければ直打ちのtextMessageを使う
        string message = (_data != null) ? _data.promptText : textMessage;

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