using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem; // これを追加

namespace UnityChan
{
    [RequireComponent(typeof(Animator))]
    public class IdleChanger : MonoBehaviour
    {
        private Animator anim;
        private AnimatorStateInfo currentState;
        private AnimatorStateInfo previousState;
        public bool _random = false;
        public float _threshold = 0.5f;
        public float _interval = 10f;

        void Start()
        {
            anim = GetComponent<Animator>();
            currentState = anim.GetCurrentAnimatorStateInfo(0);
            previousState = currentState;
            StartCoroutine("RandomChange");
        }

        void Update()
        {
            // --- ここを新しい Input System に書き換え ---
            var keyboard = Keyboard.current;
            if (keyboard == null) return; // キーボードが接続されていない場合は何もしない

            // ↑キー または スペースキー
            if (keyboard.upArrowKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame)
            {
                anim.SetBool("Next", true);
            }

            // ↓キー
            if (keyboard.downArrowKey.wasPressedThisFrame)
            {
                anim.SetBool("Back", true);
            }
            // ------------------------------------------

            // "Next"フラグの処理
            if (anim.GetBool("Next"))
            {
                currentState = anim.GetCurrentAnimatorStateInfo(0);
                if (previousState.fullPathHash != currentState.fullPathHash)
                {
                    anim.SetBool("Next", false);
                    previousState = currentState;
                }
            }

            // "Back"フラグの処理
            if (anim.GetBool("Back"))
            {
                currentState = anim.GetCurrentAnimatorStateInfo(0);
                if (previousState.fullPathHash != currentState.fullPathHash)
                {
                    anim.SetBool("Back", false);
                    previousState = currentState;
                }
            }
        }

        // OnGUIはそのままでも動作しますが、
        // 現代のUnityではUIシステム(Canvas)を使うのが一般的です。
        void OnGUI()
        {
            GUI.Box(new Rect(Screen.width - 110, 10, 100, 90), "Change Motion");
            if (GUI.Button(new Rect(Screen.width - 100, 40, 80, 20), "Next"))
                anim.SetBool("Next", true);
            if (GUI.Button(new Rect(Screen.width - 100, 70, 80, 20), "Back"))
                anim.SetBool("Back", true);
        }

        IEnumerator RandomChange()
        {
            while (true)
            {
                if (_random)
                {
                    float _seed = Random.Range(0.0f, 1.0f);
                    if (_seed < _threshold)
                    {
                        anim.SetBool("Back", true);
                    }
                    else if (_seed >= _threshold)
                    {
                        anim.SetBool("Next", true);
                    }
                }
                yield return new WaitForSeconds(_interval);
            }
        }
    }
}