using Photon.Pun;
using Script.Bar;
using Script.EntityPlayer;
using Script.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Menu
{
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
            MasterManager master = MasterManager.Instance;

            if (scene.buildIndex == 1) // We are in the bar
            {
                BarManager barManager = BarManager.Instance;
            
                Transform spawn = barManager.GetSpawn();   
            
                Chassé hunted = PhotonNetwork.Instantiate("PhotonPrefabs/Humanoide/Chassé",
                    spawn.position, spawn.rotation).GetComponent<Chassé>();
            
                barManager.NewHunted(hunted);
            }
            else if (master) // We are in the game scene
            {
                if (master.IsInMaintenance())
                {
                    // 
                }
                else
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        master.SettingsGame.Send();
                    }

                    PhotonNetwork.Instantiate("PhotonPrefabs/Manager/PlayerManager",
                        Vector3.zero, Quaternion.identity);
                }
            }
        }
    }
}
