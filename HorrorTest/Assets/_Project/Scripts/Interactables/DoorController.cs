using UnityEngine;
using DG.Tweening; // DOTweenを使う宣言

public class DoorController : MonoBehaviour
{
    [Header("設定")]
    public float openAngle = 90f; // 開く角度
    public float duration = 1.0f; // かかる時間（秒）
    public Ease moveEase = Ease.OutQuad; // 動きのタイプ（最初速くて最後ゆっくり）

    private bool isOpen = false;
    private bool isMoving = false; // 動いている最中か？


    // 外部（ボタンなど）からも呼べるようにpublicにする
    public void ToggleDoor()
    {
        // 動いている最中は操作を受け付けない（連打防止）
        if (isMoving) return;

        isMoving = true;
        isOpen = !isOpen;

        // 目標の角度を決める
        Vector3 targetAngle = isOpen ? new Vector3(0, openAngle, 0) : new Vector3(0, 0, 0);

        // DOTweenで回転させる
        transform.DOLocalRotate(targetAngle, duration)
            .SetEase(moveEase)
            .OnComplete(() => {
                isMoving = false; // 動き終わったらフラグを下ろす
            });
    }
}