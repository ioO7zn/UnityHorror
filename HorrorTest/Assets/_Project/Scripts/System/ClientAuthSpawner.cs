using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System.Collections;
using System.Linq; // ←【追加】並べ替え（ソート）に必要

public class ClientAuthSpawner : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            StartCoroutine(SetMyPositionRoutine());
        }
    }

    private IEnumerator SetMyPositionRoutine()
    {
        // 念のため少しだけ長めに待機
        yield return new WaitForSeconds(0.2f);

        int myIndex = (int)OwnerClientId;

        // 1. タグで全て取得
        GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");

        if (spawnObjects.Length > 0)
        {
            // 2. 【最重要】オブジェクトの名前順（A→Z, 1→9）に綺麗に並べ替える
            var spawnPoints = spawnObjects.OrderBy(go => go.name).ToArray();

            int targetIndex = myIndex % spawnPoints.Length;
            Transform target = spawnPoints[targetIndex].transform;

            // キャラコンの無効化
            var cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // ワープ実行
            transform.position = target.position;
            transform.rotation = target.rotation;

            var nt = GetComponent<NetworkTransform>();
            if (nt != null)
            {
                nt.Teleport(target.position, target.rotation, transform.localScale);
            }

            yield return new WaitForSeconds(0.1f);
            if (cc != null) cc.enabled = true;

            Debug.Log($"[ClientAuth] Player {myIndex} は {target.name} ({target.position}) に着地しました！");
        }
        else
        {
            Debug.LogError("SpawnPointが見つかりません！");
        }
    }
}