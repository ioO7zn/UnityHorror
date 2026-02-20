using UnityEngine;
using DG.Tweening;

public class MugController : NetworkInteractable
{
    [Header("マグカップの設定")]
    [SerializeField] private float rotateAngle = 90f;
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private Ease moveEase = Ease.OutQuad;
    

    /*
    protected override void PerformVisualAction()
    {
        //RotateMug();
        base.PerformVisualAction();
    }
    */

    public void RotateMug()
    {
        // 親クラスのIsBusyをtrueにする（これでUIが非表示になる）
        IsBusy = true;

        transform.DOLocalRotate(new Vector3(0, rotateAngle, 0), duration)
            .SetRelative(true)
            .SetEase(moveEase)
            .OnComplete(() => 
            {
                // 終わったらIsBusyをfalseに戻す（これでUIが再表示される）
                IsBusy = false; 
            });
    }

    // もし「一度回したら二度と触れない」ようにしたい場合は、
    // OnCompleteの中で false に戻さないだけでOKです（親の _isOnce 設定でも制御可能）
}