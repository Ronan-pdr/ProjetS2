using UnityEngine;

namespace Script.DossierPoint
{
    public class CrossManager : MonoBehaviour
    {
        public static CrossManager Instance;
        private Point[] crossPoints;
    
        private void Awake()
        {
            Instance = this;
            crossPoints = GetComponentsInChildren<Point>();
        }
    
        //Getter
        public int GetNumberPoint() => crossPoints.Length;
        
        public (Vector3, int) GetRandomPosition(int previousIndex)
        {
            int len = crossPoints.Length;
            int max = len;
            if (previousIndex == -1) //Si bot vient d'aparaître
                previousIndex = len;
            else
                max -= 1;
                
    
            int index = Random.Range(0, max);
            if (index >= previousIndex)
                index++;
            
            return (crossPoints[index].transform.position, index);
        }
    
        public Vector3 GetPosition(int index)
        {
            if (index >= crossPoints.Length)
            {
                Debug.Log("GetPostion cross manager : index out of range");
                return Vector3.zero;
            }
    
            return crossPoints[index].transform.position;
        }
    
        public Vector3[] GetListPosition()
        {
            int len = GetNumberPoint();
            Vector3[] positions = new Vector3[len];
            for (int i = 0; i < len; i++)
            {
                positions[i] = crossPoints[i].transform.position;
            }
    
            return positions;
        }
    }
}