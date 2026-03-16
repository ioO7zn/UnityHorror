using UnityEngine;
using UnityEngine.InputSystem;

public class SurvivorController : PlayerControllerCC
{
    // Input Systemから呼ばれる「窓口」
    public void OnInteract(InputAction.CallbackContext context)
    {
        // 1. 自分が操作している時だけ
        if (!IsOwner) return;

        // 2. ボタンが「押された瞬間」だけ
        if (context.started)
        {
            // 3. PlayerInteractコンポーネントに実行を依頼
            _interact.DoInteract();
        }
    }
}