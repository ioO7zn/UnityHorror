using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI設定")]
    public GameObject interactTextPrefab;
    public Canvas mainCanvas;
    
    private GameObject interactText;
    private TMP_Text _textComponent;

    void Start()
    {
        if (interactTextPrefab == null) return;

        // 生成
        interactText = Instantiate(interactTextPrefab, mainCanvas.transform);
        _textComponent = interactText.GetComponentInChildren<TMP_Text>();
        
        // 【重要】名前を変えて、ヒエラルキーで見つけやすくする
        interactText.name = "DEBUG_InteractText";
        
        // 初期状態は非表示
        interactText.SetActive(false);
    }

    public void UpdateUI(IInteractable target, RaycastHit hit, Quaternion cameraRotation)
    {
        // 参照チェック
        if (interactText == null)
        {
            Debug.LogError("interactTextが消えています！どこかで破壊された可能性があります。");
            return;
        }

        // 表示・非表示の判定をシンプルにする
        if (target == null || !target.CanInteract)
        {
            if (interactText.activeSelf) interactText.SetActive(false);
            return;
        }

        // ここまで来たら強制的に表示！
        if (!interactText.activeSelf) 
        {
            Debug.Log("<color=cyan>UIをSetActive(true)にしました！</color>");
            interactText.SetActive(true);
        }

        // 座標と回転
        interactText.transform.position = hit.point + (hit.normal * 0.1f);
        interactText.transform.rotation = cameraRotation;

        if (_textComponent != null)
        {
            _textComponent.text = target.GetInteractText();
        }
    }
}