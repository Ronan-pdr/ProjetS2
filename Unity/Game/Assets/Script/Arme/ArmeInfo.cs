using UnityEngine;


[CreateAssetMenu(menuName = "FPS/New Arme Info")]
public class ArmeInfo : ScriptableObject
{
    [SerializeField] private string armeName;
    [SerializeField] private float damage;
    [SerializeField] private float portéeAttaque;
    [SerializeField] private float frequenceAttaque;
    
    //Getter
    public string GetName() => armeName;
    public float GetDamage() => damage;
    public float GetPortéeAttaque() => portéeAttaque;
    public float GetFrequenceAttaque() => frequenceAttaque;
}