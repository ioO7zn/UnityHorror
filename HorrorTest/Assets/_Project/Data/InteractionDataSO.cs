using UnityEngine;

[CreateAssetMenu(fileName = "NewInteractionData", menuName = "HorrorGame/InteractionData")]

public class InteractionDataSO : ScriptableObject
{
    [Header("表示設定")]
    public string promptText;      // 「隠れる」「開ける」などの文字
    public string subPromptText;   // 「出る」「閉める」など、状態が変わった時の文字
    
    [Header("演出設定")]
    public AudioClip interactSound; // ついでに音も設定できるようにすると便利！
}