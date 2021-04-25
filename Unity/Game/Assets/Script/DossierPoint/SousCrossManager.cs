using UnityEngine;

namespace Script.DossierPoint
{
    // C'est class sert juste à ranger convenablement les cross Points
    public class SousCrossManager : MonoBehaviour
    {
        // ------------ Attributs ------------

        private CrossPoint[] _crossPoints;
        
        // ------------ Getter ------------
        public CrossPoint[] CrossPoints => _crossPoints;
        
        // ------------ Constructeurs ------------
        private void Awake()
        {
            _crossPoints = GetComponentsInChildren<CrossPoint>();
        }
    }
}