using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartHost()
    {
        Unity.Netcode.NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        Unity.Netcode.NetworkManager.Singleton.StartClient();
    }

}
