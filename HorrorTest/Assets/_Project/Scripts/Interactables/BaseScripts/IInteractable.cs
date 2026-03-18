using UnityEngine.InputSystem;

public interface IInteractable
{
    //実行する
    void Interact();
    // UIに表示したい文字を返す
    string GetInteractText();

    //アクションそのものを返すようにする
    InputAction GetInteractAction();

    bool CanInteract { get; }
}