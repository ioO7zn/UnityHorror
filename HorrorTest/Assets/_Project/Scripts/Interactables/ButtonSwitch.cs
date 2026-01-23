using UnityEngine;
using DG.Tweening; // 子供だけがDOTweenを知っていればいい

// InteractableBase を継承（コピー）する
public class ButtonSwitch : InteractableBase
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

    // override = 「親の機能に、自分の処理を付け足すよ！」
    public override void OnInteract()
    {
         // 1. まず自分独自の「見た目の処理」をする
        if (buttonMesh != null)
        {
            buttonMesh.DOKill();
            buttonMesh.localPosition = _initialPosition;
            buttonMesh.DOPunchPosition(new Vector3(0, 0, -pushDepth), duration, 1, 0);
        }
        base.OnInteract();
    }
}