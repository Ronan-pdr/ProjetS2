using UnityEngine;
using Photon.Realtime;
using Script.DossierPoint;
using Script.EntityPlayer;
using Script.Manager;
using Script.Tools;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Script.Bot
{
    public abstract class BotClass : Humanoide
    {
        // ------------ Attributs ------------
        
        protected BotManager BotManager; // instancié lorsque le bot est créé dans son BotManager
        
        //Rotation
        protected float rotationSpeed;
        protected float AmountRotation;
        
        // ------------ Getters ------------
        
        // cette fonction indique si un bot est contrôlé par ton ordinateur
        public bool IsMyBot()
        {
            return BotManager != null;
        }

        // ------------ Setter ------------
        public void SetOwnBotManager(BotManager value)
        {
            BotManager = value;
        }

        // ------------ Constructeurs ------------
        protected void AwakeBot()
        {
            AwakeHuman();
        }

        protected void StartBot()
        {
            SetRbTr();
        
            MaxHealth = 100;
            StartHuman(); // vie

            name = MasterManager.Instance.GetNameBot(Pv.Owner);

            if (BotManager == null) // cela veut dire que c'est pas cet ordinateur qui a créé ces bots ni qui les contrôle
                Tr.parent = MasterManager.Instance.GetDossierOtherBot(); // le parenter dans le dossier qui contient tous les bots controlés par les autres joueurs
            else
                Tr.parent = BotManager.transform; // le parenter dans ton dossier de botManager
        }

        // ------------ Méthodes ------------
        protected void UpdateBot()
        {
            UpdateHumanoide();
        }
        
        // Déplacement

        protected abstract void FiniDeTourner();

        protected void Tourner()
        {
            int sensRotation;
            if (AmountRotation >= 0)
                sensRotation = 1;
            else
                sensRotation = -1;

            float yRot = sensRotation * rotationSpeed * Time.deltaTime;

            if (SimpleMath.Abs(AmountRotation) < SimpleMath.Abs(yRot)) // Le cas où on a finis de tourner
            {
                Tr.Rotate(new Vector3(0, AmountRotation, 0));
                AmountRotation = 0;
                FiniDeTourner();
            }
            else
            {
                Tr.Rotate(new Vector3(0, yRot, 0));
                AmountRotation -= yRot;
            }
        }
        
        // GamePlay
        protected override void Die()
        {
            enabled = false;
            BotManager.Die(gameObject);
        }

        // réception des hash
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!Pv.Owner.Equals(targetPlayer) || !this) // si c'est pas toi la target, tu ne changes rien
                return;
        
            // point de vie -> TakeDamage (Humanoide)
            // Tout le monde doit faire ce changement (trop compliqué de retrouvé celui qui l'a déjà fait)
            if (changedProps.TryGetValue("PointDeVieBot", out object mes))
            {
                (string nameTarget, int vie) = DecodeFormatVieBot((string)mes);

                // parce que chaque joueur contrôle plusieurs bot, il faut donc faire une deuxième vérification
                if (name == nameTarget)
                {
                    CurrentHealth = vie;
                }
            }
        }

        // le format -> name[n char]Vie[3 char]
        public string EncodeFormatVieBot()
        {
            string mes = CurrentHealth.ToString();
            while (mes.Length < 3) // on formate la vie à trois charactères
            {
                mes = " " + mes;
            }

            return name + mes;
        }

        private (string nameTarget, int vie) DecodeFormatVieBot(string mes)
        {
            string deuxInfos = mes;
            int len = deuxInfos.Length;
            string nameTarget = deuxInfos.Substring(0, len - 3);
            
            int vie = int.Parse(deuxInfos.Substring(len - 3, 3));

            return (nameTarget, vie);
        }
    }
}