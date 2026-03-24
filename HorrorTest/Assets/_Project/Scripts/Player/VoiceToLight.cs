using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(AudioSource))]
public class VoiceToLight : NetworkBehaviour
{
    public Light targetLight;       // 反応させたいLight
    public float sensitivity = 100f; // 感度調整
    public float lerpSpeed = 10f;    // 変化のなめらかさ
    public float minIntensity = 0.2f; // 最低限の明るさ（真っ暗防止）

    private AudioSource audioSource;
    private string deviceName;
    private bool microphoneReady;

    private bool ShouldSkipMicrophoneInitialization()
    {
#if UNITY_SERVER
        return true;
#else
        return IsServer && !IsClient;
#endif
    }

    void Start()
    {
        // 専用サーバーやバッチモードでは録音デバイスに触らない
        if (ShouldSkipMicrophoneInitialization())
        {
            enabled = false;
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            enabled = false;
            return;
        }
        
        // マイクデバイスの確認
        if (Microphone.devices.Length > 0)
        {
            deviceName = Microphone.devices[0];
            // マイク入力をAudioClipとしてループ再生（遅延を最小限にするため1秒のバッファ）
            audioSource.clip = Microphone.Start(deviceName, true, 1, 44100);
            audioSource.loop = true;
            
            // マイクの準備ができるまで待機
            while (!(Microphone.GetPosition(deviceName) > 0)) { }
            audioSource.Play();
            microphoneReady = true;
        }
        else
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (!microphoneReady || targetLight == null)
        {
            return;
        }

        float loudness = GetLoudnessFromAudio() * sensitivity;
        
        // Lightの強度をなめらかに変化させる
        targetLight.intensity = Mathf.Lerp(targetLight.intensity, minIntensity + loudness, Time.deltaTime * lerpSpeed);
    }

    float GetLoudnessFromAudio()
    {
        if (!microphoneReady || string.IsNullOrEmpty(deviceName) || audioSource == null || audioSource.clip == null)
        {
            return 0f;
        }

        int sampleCount = 128; // 解析するサンプル数
        float[] waveData = new float[sampleCount];
        int micPosition = Microphone.GetPosition(deviceName) - (sampleCount + 1);
        
        if (micPosition < 0) return 0;

        audioSource.clip.GetData(waveData, micPosition);

        float totalLoudness = 0;
        foreach (var sample in waveData)
        {
            totalLoudness += Mathf.Abs(sample);
        }
        return totalLoudness / sampleCount;
    }

    void OnDisable()
    {
        if (!string.IsNullOrEmpty(deviceName) && Microphone.IsRecording(deviceName))
        {
            Microphone.End(deviceName);
        }

        microphoneReady = false;
    }
}