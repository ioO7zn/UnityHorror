using Unity.Netcode.Components;
using UnityEngine;

// クライアント(Owner)にTransformの決定権を譲渡するクラス
[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkTransform
{
    // サーバーが権限を持つか？に「false(いいえ)」と答えることで、
    // クライアント主導の同期が可能になります
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}