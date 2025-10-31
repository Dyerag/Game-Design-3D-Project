using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using UnityEngine;

public class AutoStartNetwork : MonoBehaviour
{
    void Start()
    {
#if UNITY_EDITOR
        // Editor: Multiplayer Play Mode
        if (CurrentPlayer.IsMainEditor)
        {
            Debug.Log("[AutoNetworkStart] Starting as Host (Main Editor Player)");
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            Debug.Log("[AutoNetworkStart] Starting as Client (Virtual Player)");
            NetworkManager.Singleton.StartClient();
        }
#else
        // Build: auto-start host for single-player build
        if (!NetworkManager.Singleton.IsListening)
        {
            Debug.Log("[AutoNetworkStart] Running in build, starting Host automatically");
            NetworkManager.Singleton.StartHost();
        }
#endif
    }
}