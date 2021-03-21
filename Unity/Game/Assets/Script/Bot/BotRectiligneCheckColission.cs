using UnityEngine;

namespace Script.Bot
{
    public class BotRectiligneCheckColission : MonoBehaviour
    {
        private BotRectiligne bot;
        
        private void Awake()
        {
            bot = GetComponent<BotRectiligne>();
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (!bot.IsMyBot()) // Ton ordi contrôle seulement tes bots
                return;
        
            if (other.gameObject == bot.gameObject) // si c'est son propre corps qu'il a percuté
                return;
            
            if (bot.GetEtat() == 0) // recalcule seulement quand il avance
            {
                bot.FindAmountRotation();
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (!bot.IsMyBot()) // Ton ordi contrôle seulement tes bots
                return;
            
            if (other.gameObject == bot.gameObject) // si c'est son propre corps qu'il a percuté
                return;

            if (bot.GetEtat() == 0) // recalcule seulement quand il avance
            {
                bot.FindAmountRotation();
            }
        }
    }
}