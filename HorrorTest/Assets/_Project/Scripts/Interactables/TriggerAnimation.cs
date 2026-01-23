using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
    [Header("動かしたいオブジェクト")]
    public Animator targetAnimator;
    public AnimationClip clipToPlay;
    public bool playOnce = true;
    private bool hasPlayed = false;

    void OnTriggerEnter(Collider other)
    {
        // 1. まず「何かがぶつかった」ことを報告
        Debug.Log($"何かが入った！ 名前: {other.gameObject.name} / タグ: {other.tag}");

        if (playOnce && hasPlayed) return;
        if (clipToPlay == null) 
        {
            Debug.LogError("エラー：アニメーションクリップがセットされていません！");
            return;
        }

        // 2. タグの判定
        if (other.CompareTag("Player"))
        {
            Debug.Log("プレイヤーだと確認できた！アニメ再生を試みます...");
            
            // 3. アニメーション再生命令
            if(targetAnimator != null)
            {
                targetAnimator.Play(clipToPlay.name);
                Debug.Log($"再生命令を出しました: {clipToPlay.name}");
                hasPlayed = true;
            }
            else
            {
                Debug.LogError("エラー：Animatorがセットされていません！");
            }
        }
        else
        {
            Debug.Log("プレイヤーじゃなかったので無視しました。");
        }
    }
}