using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject interactTextPrefab;
    private GameObject interactText;
    
    // ★追加：コンポーネントを保存する箱（どんなTMPでも入る型にする）
    private TMP_Text _textComponent;

    void Start()
    {
        if (interactTextPrefab == null) return;
        interactText = Instantiate(interactTextPrefab);
        
        // ★追加：生成時に一度だけコンポーネントを探して保存する
        _textComponent = interactText.GetComponentInChildren<TMP_Text>();
        
        interactText.SetActive(false);
    }

    public void UpdateUI(IInteractable target, RaycastHit hit)
    {
        if (interactText == null || _textComponent == null) return;

        if (target == null || !target.CanInteract)
        {
            interactText.SetActive(false);
            return;
        }

        interactText.SetActive(true);
        interactText.transform.position = hit.point;

        //カメラ正面
        interactText.transform.rotation = Camera.main.transform.rotation;

        _textComponent.text = target.GetInteractText();
    }
}