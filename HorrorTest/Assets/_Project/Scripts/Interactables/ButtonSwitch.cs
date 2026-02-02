using UnityEngine;
using DG.Tweening; // 子供だけがDOTweenを知っていればいい

// InteractableBase を継承（コピー）する
public class ButtonSwitch : NetworkInteractable
{
    [Header("見た目の設定 (子クラス)")]
    [SerializeField] private Transform buttonMesh;
    [SerializeField] private float pushDepth = 0.05f;
    [SerializeField] private float duration = 0.1f;

    private Vector3 _initialPosition;

    private void Start()
    {
        if (buttonMesh != null) _initialPosition = buttonMesh.localPosition;
    }


    protected override void PerformVisualAction()
    {
        // 全員の画面でボタンが凹む
        if (buttonMesh != null)
        {
            buttonMesh.DOKill();
            buttonMesh.localPosition = _initialPosition;
            buttonMesh.DOPunchPosition(new Vector3(0, 0, -pushDepth), duration, 1, 0);
        }

        // 全員の画面でUnityEvent（ドア開閉など）が動く
        base.PerformVisualAction();
    }
}