using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode; // NGO（Netcode for GameObjects）を使用するために必須

/// <summary>
/// プレイヤーの所持品を管理するコンポーネント。
/// プレイヤーのPrefab（NetworkObject付き）にアタッチしてください。
/// </summary>
public class PlayerInventory : NetworkBehaviour
{
    // --- データの保持 ---
    // インスペクターから中身が見えるようにSerializeFieldにしています
    [SerializeField] private List<ItemData> items = new List<ItemData>();

    // --- 通知システム ---
    // UI側がこのイベントを購読することで、アイテムが増えた瞬間に画面を更新できます
    public event Action OnInventoryChanged;


    // --- メソッド（機能） ---

    /// <summary>
    /// アイテムをインベントリに追加する
    /// </summary>
    /// <param name="newItem">追加するアイテムのScriptableObject</param>
    public void AddItem(ItemData newItem)
    {
        // 【重要】マルチプレイのガード：
        // 自分のキャラクター（自身が操作しているインスタンス）のリストのみを更新します
        if (!IsOwner) return;

        if (newItem == null)
        {
            Debug.LogWarning("追加しようとしたアイテムデータが空です。");
            return;
        }

        items.Add(newItem);
        Debug.Log($"<color=cyan>[Inventory]</color> {newItem.itemName} を入手しました！");

        // UIなどの購読者に「変更があった」ことを通知
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// 特定のアイテムを持っているかチェックする（鍵の判定などに使用）
    /// </summary>
    public bool HasItem(ItemData targetData)
    {
        // 自分の所持リストの中に、指定されたSOが含まれているかを確認
        return items.Contains(targetData);
    }

    /// <summary>
    /// インベントリ内の全アイテムを取得（UI表示用）
    /// </summary>
    public List<ItemData> GetItemList()
    {
        return items;
    }

    /// <summary>
    /// アイテムを使用したり捨てたりして、リストから削除する
    /// </summary>
    public void RemoveItem(ItemData targetData)
    {
        if (!IsOwner) return;

        if (items.Contains(targetData))
        {
            items.Remove(targetData);
            OnInventoryChanged?.Invoke();
        }
    }
}