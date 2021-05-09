using System;
using Script.EntityPlayer;
using Script.Manager;
using Script.Tools;
using UnityEngine;

namespace Script.Graph
{
    public class Line: Entity
    {
        // ------------ SerializeField ------------
        
        //[SerializeField] private Transform graphics;
        
        // ------------ Constructeur ------------

        public static void Create(Vector3 point1, Vector3 point2)
        {
            Line line = Instantiate(MasterManager.Instance.GetOriginalLine(), point1, Quaternion.identity);
            
            line.Const(point1, point2);
        }

        private void Const(Vector3 point1, Vector3 point2)
        {
            SetRbTr();

            Vector3 diff = Calcul.Diff(point2, point1);

            float rotX = -Calcul.BetterArctan(diff.y, Calcul.Distance(point1, point2, Calcul.Coord.Y));
            float rotY = Calcul.Angle(0, point1, point2, Calcul.Coord.Y);
            Tr.Rotate(0, rotY, 0);
            Tr.Rotate(rotX, 0, 0);

            float sizeZ = Calcul.Distance(point1, point2);
            
            Tr.localScale = new Vector3(1, 1, sizeZ);
            Tr.position += Tr.TransformDirection(Vector3.forward) * sizeZ / 2;
        }
    }
}