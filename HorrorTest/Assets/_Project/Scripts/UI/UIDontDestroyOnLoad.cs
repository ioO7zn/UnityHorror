using UnityEngine;

public class UIDontDestroyOnLoad : MonoBehaviour
{
    void Awake()
{
    DontDestroyOnLoad(this.gameObject);
}
}
