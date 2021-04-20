using System;
using System.Collections.Generic;
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
        
        // variables relatives à la caméra artificiel des bots
        private static float AngleCamera = 80; // le degré pour la vision périphérique
        private static Vector3 PositionCamera = new Vector3(0, 1.4f, 0.3f); // correspond à la distance séparant le "cameraHolder" de la "camera" de type "Camera"

        //Le bot va recalculer automatiquement sa trajectoire au bout de 'ecartTime'
        protected float LastCalculRotation; //cette variable contient le dernier moment durant lequel le bot à recalculer sa trajectoire

        
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

            // son nom (qui sera unique)
            name = MasterManager.Instance.GetNameBot(Pv.Owner);

            // le parenter
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

        protected void FixedUpdateBot()
        {
            if (IsMyBot())
            {
                MoveEntity();
            }
        }
        
        // Déplacement

        protected abstract void FiniDeTourner();

        protected void CalculeRotation(Vector3 dest)
        {
            AmountRotation = Calcul.Angle(Tr.eulerAngles.y, Tr.position, dest, Calcul.Coord.Y);
            LastCalculRotation = Time.time;
        }

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

        protected bool IsArrivé(Vector3 dest)
        {
            return Calcul.Distance(Tr.position, dest, Calcul.Coord.Y) < 0.5f;
        }
        
        protected List<PlayerClass> IsPlayerInMyVision(TypePlayer typePlayer)
        {
            Func<int, PlayerClass> getPlayer;
            int l;

            // récupérer la fonction pour récupérer le bon type de joueur et
            // le nombre de ce type de joueur
            switch (typePlayer)
            {
                case TypePlayer.Player:
                    getPlayer = master.GetPlayer;
                    l = master.GetNbPlayer();
                    break;
                case TypePlayer.Chasseur:
                    getPlayer = master.GetChasseur;
                    l = master.GetNbChasseur();
                    break;
                case TypePlayer.Chassé:
                    getPlayer = master.GetChassé;
                    l = master.GetNbChassé();
                    break;
                default:
                    throw new Exception($"Le cas {typePlayer} n'est pas encore géré");
            }
            
            Vector3 positionCamera = Tr.position + Tr.TransformDirection(PositionCamera);
            List<PlayerClass> playerInVision = new List<PlayerClass>();

            for (int i = 0; i < l; i++)
            {
                Vector3 posPlayer = getPlayer(i).transform.position;

                float angleY = Calcul.Angle(Tr.eulerAngles.y, positionCamera,
                    posPlayer, Calcul.Coord.Y);

                if (SimpleMath.Abs(angleY) < AngleCamera) // le chasseur est dans le champs de vision du bot ?
                {
                    Ray ray = new Ray(positionCamera, Calcul.Diff(posPlayer, positionCamera));

                    if (Physics.Raycast(ray, out RaycastHit hit)) // y'a t'il aucun obstacle entre le chasseur et le bot ?
                    {
                        if (hit.collider.GetComponent<PlayerClass>()) // si l'obstacle est le joueur alors le bot "VOIT" le joueur
                        {
                            playerInVision.Add(hit.collider.GetComponent<PlayerClass>());
                        }
                    }
                }
            }

            return playerInVision;
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