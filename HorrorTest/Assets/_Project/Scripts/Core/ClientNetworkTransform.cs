using Unity.Netcode.Components;
using UnityEngine;

// サーバー権威型のTransform同期クラス
[DisallowMultipleComponent]
public class ServerNetworkTransform : NetworkTransform
{
    // サーバーが権限を持つ：true
    protected override bool OnIsServerAuthoritative()
    {
        return true;
    }
}