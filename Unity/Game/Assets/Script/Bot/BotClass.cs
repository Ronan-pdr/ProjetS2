using System;
using System.Collections.Generic;
using Photon.Pun;
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
        // ------------ Enum ------------
        protected enum Running
        {
            Arret,
            Marche,
            Course
        }

        protected Running running = Running.Arret;
        
        // ------------ Attributs ------------
        
        protected BotManager BotManager; // instancié lorsque le bot est créé dans son BotManager
        
        //Rotation
        protected float RotationSpeed = 400;
        protected float AmountRotation;
        
        // variables relatives à la caméra artificiel des bots
        private static float AngleCamera = 80; // le degré pour la vision périphérique
        private static Vector3 PositionCamera = new Vector3(0, 1.4f, 0.3f); // correspond à la distance séparant le "cameraHolder" de la "camera" de type "Camera"

        //Le bot va recalculer automatiquement sa trajectoire au bout de 'ecartTime'
        protected float LastCalculRotation; //cette variable contient le dernier moment durant lequel le bot à recalculer sa trajectoire

        // Vitesse
        protected float TranquilleVitesse = WalkSpeed;
        protected float PleineVitesse = SprintSpeed;
        
        // quand il est bloqué
        private (float time, Vector3 position) block;

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
        protected abstract void AwakeBot();
        
        protected void Awake()
        {
            AwakeHuman();
            AwakeBot();
        }

        protected abstract void StartBot();

        private void Start()
        {
            MaxHealth = 100;
            StartHuman();
            StartBot();
            
            // son nom (qui sera unique)
            name = BotManager.Instance.GetNameBot(this, Pv.Owner);

            // le parenter
            if (BotManager == null) // cela veut dire que c'est pas cet ordinateur qui a créé ces bots ni qui les contrôle
            {
                // le parenter dans le dossier qui contient tous les bots controlés par les autres joueurs
                Tr.parent = MasterManager.Instance.GetDossierOtherBot();
            }
            else
            {
                Tr.parent = BotManager.transform; // le parenter dans ton dossier de botManager
            }
        }

        // ------------ Update ------------
        
        // Upadte
        protected abstract void UpdateBot();

        private void Update()
        {
            if (!IsMyBot())
                return;
            
            UpdateBot();
            UpdateHumanoide();

            SetSpeed();
            ManageBlock();
        }

        protected void FixedUpdate()
        {
            if (IsMyBot())
            {
                MoveEntity();
            }
        }
        
        // ------------ Méthodes ------------

        // Rotation
        protected void GestionRotation(Vector3 dest, float periodeCalculRotation)
        {
            // il recalcule sa rotation tous les 0.5f
            if (Time.time - LastCalculRotation > periodeCalculRotation)
            {
                CalculeRotation(dest);
            }

            Tourner();
        }
        
        protected void GestionRotation(Vector3 dest)
        {
            GestionRotation(dest, 0.5f);
        }
        
        protected void CalculeRotation(Vector3 dest)
        {
            AmountRotation = Calcul.Angle(Tr.eulerAngles.y, Tr.position, dest, Calcul.Coord.Y);
            LastCalculRotation = Time.time;
        }

        protected void Tourner()
        {
            if (SimpleMath.Abs(AmountRotation) <= 0)
                return;
            
            int sensRotation;
            if (AmountRotation >= 0)
                sensRotation = 1;
            else
                sensRotation = -1;

            float yRot = sensRotation * GetAmountYRot() * RotationSpeed * Time.deltaTime;

            if (SimpleMath.Abs(AmountRotation) < SimpleMath.Abs(yRot)) // Le cas où on a finis de tourner
            {
                Tr.Rotate(new Vector3(0, AmountRotation, 0));
                AmountRotation = 0;
            }
            else
            {
                Tr.Rotate(new Vector3(0, yRot, 0));
                AmountRotation -= yRot;
            }
        }

        private float GetAmountYRot()
        {
            float absMoveAmount = SimpleMath.Abs(AmountRotation);
            if (absMoveAmount < 5)
                return 0.7f;
            if (absMoveAmount < 30)
                return 0.8f;
            if (absMoveAmount < 60)
                return 0.9f;
            if (absMoveAmount < 90)
                return 1.1f;
            if (absMoveAmount < 120)
                return 1.3f;
            if (absMoveAmount < 150)
                return 1.5f;
            
            return 1.7f;
        }
        
        // vitesse
        private void SetSpeed()
        {
            if (running == Running.Arret)
            {
                MoveAmount = Vector3.zero;
            }
            else if (AmountRotation > 120)
            {
                // ralenti pour le virage
                SetMoveAmount(Vector3.forward, 0.5f);
                //ActiverAnimation("Avant");
            }
            else if (AmountRotation > 60)
            {
                // ralenti pour le virage
                SetMoveAmount(Vector3.forward, 1f);
                //ActiverAnimation("Avant");
            }
            else if (running == Running.Marche)
            {
                // marche
                SetMoveAmount(Vector3.forward, TranquilleVitesse);
                //ActiverAnimation("Avant");
            }
            else if (running == Running.Course)
            {
                // court
                SetMoveAmount(Vector3.forward, PleineVitesse);
                //ActiverAnimation("Course");
            }
        }

        // Destination
        protected bool IsArrivé(Vector3 dest) => IsArrivé(dest, 0.4f);

        protected bool IsArrivé(Vector3 dest, float ecart)
        {
            return Calcul.Distance(Tr.position, dest, Calcul.Coord.Y) < ecart;
        }

        // Vision
        protected List<PlayerClass> GetPlayerInMyVision(TypePlayer typePlayer)
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
            
            List<PlayerClass> playersInVision = new List<PlayerClass>();

            for (int i = 0; i < l; i++)
            {
                PlayerClass player = getPlayer(i);
                if (IsInMyVision(player))
                {
                    playersInVision.Add(player);
                }
            }

            return playersInVision;
        }

        private bool IsInMyVision(PlayerClass player)
        {
            Vector3 positionCamera = Tr.position + Tr.TransformDirection(PositionCamera);
            Vector3 posPlayer = player.transform.position;
            
            float angleY = Calcul.Angle(Tr.eulerAngles.y, positionCamera,
                posPlayer, Calcul.Coord.Y);

            if (SimpleMath.Abs(angleY) < AngleCamera) // le chasseur est dans le champs de vision du bot ?
            {
                Ray ray = new Ray(positionCamera, Calcul.Diff(posPlayer, positionCamera));

                if (Physics.Raycast(ray, out RaycastHit hit)) // y'a t'il aucun obstacle entre le chasseur et le bot ?
                {
                    if (hit.collider.GetComponent<PlayerClass>() == player) // si l'obstacle est le joueur alors le bot "VOIT" le joueur
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        // bloquer
        private void ManageBlock()
        {
            // vérifier qu'il n'est pas bloqué
            if (SimpleMath.IsEncadré(block.position, Tr.position))
            {
                // s'il semble bloquer à une position
                if (Time.time - block.time > 2 && running != Running.Arret)
                {
                    // et que ça fait longtemps
                    WhenBlock();
                    SetBlock();
                }
            }
            else
            {
                SetBlock();
            }
        }

        private void SetBlock()
        {
            block.time = Time.time;
            block.position = Tr.position;
        }

        protected abstract void WhenBlock();
        
        // GamePlay
        protected override void Die()
        {
            enabled = false;
            BotManager.Die(this);
            
            // détruire l'objet
            PhotonNetwork.Destroy(gameObject);
        }

        // ------------ Mulitijoueur ------------
        
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