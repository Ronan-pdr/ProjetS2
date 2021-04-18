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
        protected BotManager BotManager; // instancié lorsque le bot est créé dans son BotManager
        
        //Rotation
        protected float rotationSpeed;
        protected float AmountRotation;
    
        // Getter

        // Setter
        public void SetOwnBotManager(BotManager value)
        {
            BotManager = value;
        }

        // cette fonction indique si un bot est contrôlé par ton ordinateur
        public bool IsMyBot()
        {
            return BotManager != null;
        }

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
            changedProps.TryGetValue("PointDeVieBot", out object life);

            if (life != null)
            {
                string deuxInfos = (string) life;
                int len = deuxInfos.Length;
                string nameTarget = deuxInfos.Substring(0, len - 3);

                if (name == nameTarget) // parce que chaque joueur contrôle plusieurs bot, il faut donc faire une deuxième vérification
                {
                    int vie = int.Parse(deuxInfos.Substring(len - 3, 3));
                            
                    CurrentHealth = vie;
                }
            }
        }
    }
}