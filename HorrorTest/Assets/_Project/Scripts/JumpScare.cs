using UnityEngine;
using Unity.Cinemachine; // ★ここ重要！
using System.Collections;

public class JumpScare : MonoBehaviour
{
    [Header("切り替えるカメラ")]
    public CinemachineCamera scareCamera;


    [Header("何秒間見せる？")]
    public float duration = 2.0f;

    private bool hasPlayed = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;

        if (other.CompareTag("Player"))
        {
            hasPlayed = true;
            StartCoroutine(PlayScare());
        }
    }

    IEnumerator PlayScare()
    {

        // 2. 怖いカメラの優先度を上げて、強制的に切り替える！
        scareCamera.Priority = 20; 

        // 3. 指定した秒数待つ
        yield return new WaitForSeconds(duration);

        // 4. 元に戻す
        scareCamera.Priority = 0;
        
        Debug.Log("演出終了");
    }
}