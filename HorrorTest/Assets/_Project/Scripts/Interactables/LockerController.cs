using UnityEngine;
using DG.Tweening;
using System.Collections;

public class LockerController : InteractableBase
{
    [Header("ロッカー設定")]
    [SerializeField] private Transform _insidePoint;  // ロッカーの中の座標
    [SerializeField] private Transform _outsidePoint; // ロッカーの外の座標
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField] private Ease _moveEase = Ease.InOutQuad;

    private bool _isInside = false; // 現在中にいるか

    public override void OnInteract()
    {
        if (!_isInside)
        {
            StartCoroutine(EnterLocker());
        }
        else
        {
            StartCoroutine(ExitLocker());
        }
        base.OnInteract();
    }
    

    private IEnumerator EnterLocker()
    {
        IsBusy = true; // 親クラスの窓口を通じてロックをかける（UIが消える）

        // 1. プレイヤーを中に移動（ここでは簡易的にCamera.mainを移動させています）
        // 本来はPlayerのTransformを渡して移動させるのが理想的です
        Camera.main.transform.DOMove(_insidePoint.position, _moveDuration).SetEase(_moveEase);
        yield return Camera.main.transform.DORotate(_insidePoint.eulerAngles, _moveDuration).WaitForCompletion();

        _isInside = true;
        IsBusy = false; // 終わったのでロック解除（「出る」というUIが出るようになる）
    }

    private IEnumerator ExitLocker()
    {
        IsBusy = true;

        // 1. プレイヤーを外に移動
        Camera.main.transform.DOMove(_outsidePoint.position, _moveDuration).SetEase(_moveEase);
        yield return Camera.main.transform.DORotate(_outsidePoint.eulerAngles, _moveDuration).WaitForCompletion();

        _isInside = false;
        IsBusy = false;
    }
}