
using UnityEngine;

namespace Script.Bot
{
    public class FuyardCheckColission : MonoBehaviour
    {
        private Fuyard bot;
        
        private void Awake()
        {
            bot = GetComponent<Fuyard>();
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (!bot.IsMyBot()) // Ton ordi contrôle seulement tes bots
                return;
        
            if (other.gameObject == bot.gameObject) // si c'est son propre corps qu'il a percuté
                return;
        }

        private void OnCollisionExit(Collision other)
        {
            if (!bot.IsMyBot()) // Ton ordi contrôle seulement tes bots
                return;
            
            if (other.gameObject == bot.gameObject) // si c'est son propre corps qu'il a percuté
                return;
        }
    }
}