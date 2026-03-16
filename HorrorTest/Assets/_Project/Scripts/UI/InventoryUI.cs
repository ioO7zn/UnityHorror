using UnityEngine;
using UnityEngine.UI; // Imageコンポーネント用
using Unity.Netcode;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("スロット設定")]
    // Unityエディタ上で、作成済みのImage（スロット）を順番にドラッグ＆ドロップしてください
    [SerializeField] private List<Image> itemSlots = new List<Image>();
    
    [Header("デフォルト設定")]
    [SerializeField] private Sprite emptySlotSprite; // 空の時に表示する画像（透明な枠など）

    private PlayerInventory targetInventory;

    private void Update()
    {
        if (targetInventory == null) TryFindLocalPlayer();
    }

    private void TryFindLocalPlayer()
    {
        if (NetworkManager.Singleton?.LocalClient?.PlayerObject != null)
        {
            if (NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent<PlayerInventory>(out var inv))
            {
                targetInventory = inv;
                targetInventory.OnInventoryChanged += RefreshUI;
                RefreshUI();
            }
        }
    }

    private void RefreshUI()
    {
        if (targetInventory == null) return;

        var items = targetInventory.GetItemList();

        // 全スロットを一旦リセット、またはアイテムを割り当て
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < items.Count)
            {
                // アイテムがある場合：画像を表示して、色を不透明にする
                itemSlots[i].sprite = items[i].icon;
                itemSlots[i].color = Color.white; 
                itemSlots[i].enabled = true; // 画像がセットされている時だけ有効にする
            }
            else
            {
                // アイテムがない場合：空の画像にするか、非表示にする
                itemSlots[i].sprite = emptySlotSprite;
                // もし空枠の画像がないなら、enabled = false で消してもOK
                if (emptySlotSprite == null) itemSlots[i].enabled = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (targetInventory != null)
            targetInventory.OnInventoryChanged -= RefreshUI;
    }
}