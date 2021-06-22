using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Script.DossierPoint;
using Script.Zone;
using UnityEngine;

namespace Script.Manager
{
    [CreateAssetMenu(menuName = "Settings/Game")]
    public class SettingsGame : ScriptableObject
    {
        // ------------ SerializeField ------------

        [Header("Default Value")]
        [SerializeField] private ZoneManager.EnumZone zone;
        [SerializeField] private int nChasseur;
        [SerializeField] private int timeMax;
        
        // ------------ Getter ------------

        public ZoneManager.EnumZone Zone => zone;
        
        public int NChasseur => nChasseur;
        
        public int TimeMax => timeMax;
        
        // ------------ Setter ------------

        public void SetZone(ZoneManager.EnumZone value)
        {
            zone = value;
        }
        
        public void SetNbChasseur(int value)
        {
            nChasseur = value;
        }
        
        public void SetTimeMax(int value)
        {
            timeMax = value;
        }

        // ------------ Mutijoueur ------------

        public void Send()
        {
            Debug.Log("send");
            
            // envoi des infos au concerné(e)
            Hashtable hash = new Hashtable();
            hash.Add("SettingsGame", EncodeInfos());

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                player.SetCustomProperties(hash);
            }
        }
        
        public void Receive(string hash)
        {
            Debug.Log("set");
            DecodeInfos(hash);
            
            ZoneManager.Instance.SetZone();

            // update du spawn manager
            SpawnManager.Instance.RecupSpawns();
            
            // le master manager peut faire son taff :
            MasterManager master = MasterManager.Instance;
                
            // - envoyer les infos aux humanoide pour spawn
            master.SendInfoPlayer();
            master.SendInfoBot();
            
            // - récupérer le temps max
            master.SetTimeMax(timeMax);
        }

        private string EncodeInfos()
        {
            return $"{(int) zone};{timeMax}";
        }

        private void DecodeInfos(string s)
        {
            string[] infos = s.Split(';');

            if (int.TryParse(infos[0], out int z))
            {
                zone = (ZoneManager.EnumZone)z;
            }
            else
            {
                throw new Exception();
            }

            if (int.TryParse(infos[1], out int t))
            {
                timeMax = t;
            }
            else
            {
                throw new Exception();
            }
        }
    }
}