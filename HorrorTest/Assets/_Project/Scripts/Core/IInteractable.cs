using UnityEngine.InputSystem;

public interface IInteractable
{
    // 「調べられた時の動作」を約束する
    void OnInteract();
    
    // UIに表示する文字（例："ロッカーに隠れる"）
    string GetInteractText();

    //アクションそのものを返すようにする
    InputAction GetInteractAction();

    bool CanInteract { get; }
}