using Unity.Netcode.Components;

// これを貼るだけで「操作している人」のアニメーションが全員に同期されるようになります
public class OwnerNetworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}