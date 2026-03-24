using Unity.Netcode.Components;

// プレイヤー移動がサーバー権威なので、アニメーション同期もサーバー権威に合わせる。
public class OwnerNetworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return true;
    }
}