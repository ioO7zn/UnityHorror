using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class HorrorFPSController : MonoBehaviour
{
    [Header("移動設定")]
    public float walkSpeed = 5f;
    public float gravity = -9.81f;

    [Header("視点設定")]
    public Camera playerCamera;
    public float lookSpeed = 0.1f; // マウス感度
    public float keyRotateSpeed = 150f; // ★キーボードでの回転速度
    public float lookXLimit = 80f;

    [Header("懐中電灯")]
    public Light flashlight;

    [Header("足音と揺れ")]
    public AudioClip[] footstepSounds;
    public float stepInterval = 0.5f;
    public float bobbingAmount = 0.05f;

    private CharacterController characterController;
    private AudioSource audioSource;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    
    // 足音用
    private float stepCycle = 0f;
    private float defaultYPos = 0;
    private float timer = 0;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if(playerCamera != null)
            defaultYPos = playerCamera.transform.localPosition.y;
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null) return;

        // ---------------------------------------------------------
        // 1. 移動 (WASD)
        // ---------------------------------------------------------
        float curSpeedX = 0;
        float curSpeedY = 0;

        if (Keyboard.current.wKey.isPressed) curSpeedX = walkSpeed;
        if (Keyboard.current.sKey.isPressed) curSpeedX = -walkSpeed;
        if (Keyboard.current.dKey.isPressed) curSpeedY = walkSpeed;
        if (Keyboard.current.aKey.isPressed) curSpeedY = -walkSpeed;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (characterController.isGrounded)
        {
            moveDirection.y = -2f;
        }
        else
        {
            moveDirection.y = movementDirectionY + (gravity * Time.deltaTime);
        }

        characterController.Move(moveDirection * Time.deltaTime);

        // ---------------------------------------------------------
        // 2. 視点移動 (マウス + Q/Eキー)
        // ---------------------------------------------------------
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        
        // ★ここを追加：QとEの入力チェック
        float keyRotation = 0f;
        if (Keyboard.current.qKey.isPressed) keyRotation -= 1f; // 左へ
        if (Keyboard.current.eKey.isPressed) keyRotation += 1f; // 右へ

        // 上下回転（マウスのみ）
        rotationX += -mouseDelta.y * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        
        if(playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }

        // 左右回転（マウス + キーボード）
        // マウスの動きと、キーボードの持続的な回転を合算します
        float mouseRotationY = mouseDelta.x * lookSpeed;
        float keyRotationY = keyRotation * keyRotateSpeed * Time.deltaTime;

        transform.rotation *= Quaternion.Euler(0, mouseRotationY + keyRotationY, 0);

        // ---------------------------------------------------------
        // 3. 懐中電灯 (Fキー)
        // ---------------------------------------------------------
        if (Keyboard.current.fKey.wasPressedThisFrame && flashlight != null)
        {
            flashlight.enabled = !flashlight.enabled;
        }

        // ---------------------------------------------------------
        // 4. 足音と画面揺れ
        // ---------------------------------------------------------
        HandleHeadBobAndFootsteps(curSpeedX, curSpeedY);
    }

    void HandleHeadBobAndFootsteps(float speedX, float speedY)
    {
        // キー回転中も足踏みさせたい場合は条件に追加できますが、今回は移動時のみにします
        if ((Mathf.Abs(speedX) > 0.1f || Mathf.Abs(speedY) > 0.1f) && characterController.isGrounded)
        {
            timer += Time.deltaTime * walkSpeed;
            
            if (playerCamera != null)
            {
                float newY = defaultYPos + Mathf.Sin(timer * 2) * bobbingAmount;
                playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, newY, playerCamera.transform.localPosition.z);
            }

            stepCycle += (characterController.velocity.magnitude * Time.deltaTime);
            if (stepCycle > stepInterval)
            {
                PlayFootstep();
                stepCycle = 0;
            }
        }
        else
        {
            timer = 0;
            if (playerCamera != null)
            {
                Vector3 newPos = playerCamera.transform.localPosition;
                newPos.y = Mathf.Lerp(newPos.y, defaultYPos, Time.deltaTime * 5);
                playerCamera.transform.localPosition = newPos;
            }
        }
    }

    void PlayFootstep()
    {
        if (footstepSounds != null && footstepSounds.Length > 0 && audioSource != null)
        {
            int n = Random.Range(1, footstepSounds.Length);
            audioSource.clip = footstepSounds[n];
            audioSource.PlayOneShot(audioSource.clip);
            footstepSounds[n] = footstepSounds[0];
            footstepSounds[0] = audioSource.clip;
        }
    }
}