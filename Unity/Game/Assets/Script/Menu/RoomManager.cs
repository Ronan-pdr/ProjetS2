using UnityEngine;
using Photon.Pun;
using Script.DossierPoint;
using Script.EntityPlayer;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    void Awake()
    {
        if (Instance) // checks if another RoomManager exists
        {
            Destroy(gameObject); // there can only be one
            return;
        }
        DontDestroyOnLoad(gameObject); // I am the only one...
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        
        if (scene.buildIndex > 0) // We are in the game scene
        {
            if (MasterManager.Instance.IsInMaintenance())
            {
                // 
            }
            else
            {
                PhotonNetwork.Instantiate("PhotonPrefabs/Manager/PlayerManager",
                                                Vector3.zero, Quaternion.identity);
            }
        }
    }
}
