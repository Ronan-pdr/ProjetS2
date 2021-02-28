using UnityEngine;

namespace Script.IA
{
    public class BotCheckColission : MonoBehaviour
    {
        private BotRectiligne bot;
        
        private void Awake()
        {
            bot = GetComponent<BotRectiligne>();
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject == bot.gameObject)
                return;
        
            bot.FindAmountRotation();
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject == bot.gameObject)
                return;
        
            bot.FindAmountRotation();
        }
    }
}