using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Interact用UI")]
    [SerializeField] private GameObject _uiRoot; // 
    [SerializeField] private TextMeshProUGUI _text;

    [Header("UI")]
    [SerializeField] private GameObject _mainHUD;
    [SerializeField] private Slider _hpSlider;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _uiRoot.SetActive(false);
    }

    // プレイヤーは「何を」「どこに」だけ伝えればいい
    public void UpdateUI(IInteractable target, RaycastHit? hit)
    {
        if (target == null || hit == null)
        {
            _uiRoot.SetActive(false);
            return;
        }

        _uiRoot.SetActive(true);
        _text.text = target.GetInteractText();

        // 当たった場所の少し上にUIを出す
        _uiRoot.transform.position = hit.Value.point + Vector3.up * 0.5f;

        // 【自律ポイント】カメラの方を向く（ビルボード）
        if (Camera.main != null)
        {
            // UIの正面をカメラに向ける（向きが逆なら - をつける）
            _uiRoot.transform.rotation = Camera.main.transform.rotation;
        }
    }
}