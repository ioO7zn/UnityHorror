using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("スロット設定")]
    [SerializeField] private List<Image> itemSlots;
    [SerializeField] private Sprite emptySlotSprite; // 空枠の画像（なければnullでOK）

    private PlayerInventory localInventory;

    private void Update()
    {
        // まだ自分のインベントリを見つけていなければ探し続ける
        if (localInventory == null)
        {
            TryBindInventory();
        }

        
    }

    private void TryBindInventory()
    {
        // ネットワークが動いていて、自分のキャラが存在するか確認
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient)
        {
            var playerObj = NetworkManager.Singleton.LocalClient?.PlayerObject;
            if (playerObj != null && playerObj.TryGetComponent<PlayerInventory>(out var inv))
            {
                localInventory = inv;
                localInventory.OnInventoryChanged += RefreshUI; // 監視スタート
                RefreshUI(); // 初回表示
            }
        }
    }

    private void RefreshUI()
    {
        if (localInventory == null) return;

        // 最新のアイテムリストを取得
        var items = localInventory.currentItems;

        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < items.Count)
            {
                // アイテムを持っている枠
                itemSlots[i].sprite = items[i].icon;
                itemSlots[i].color = Color.white;
                itemSlots[i].enabled = true;
            }
            else
            {
                // 空の枠
                itemSlots[i].sprite = emptySlotSprite;
                if (emptySlotSprite == null)
                {
                    itemSlots[i].enabled = false; // 画像がなければ非表示
                }
                else
                {
                    itemSlots[i].color = new Color(1, 1, 1, 0.5f); // 半透明にするなど
                    itemSlots[i].enabled = true;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (localInventory != null)
        {
            localInventory.OnInventoryChanged -= RefreshUI;
        }
    }
}