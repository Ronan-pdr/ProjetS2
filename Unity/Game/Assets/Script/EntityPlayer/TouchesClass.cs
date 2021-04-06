using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchesClass
{
    public static TouchesClass Instance;
    
    protected KeyCode touchLeverAssoir;
    protected KeyCode touchAccroupi;

    //Avancer
    protected KeyCode touchAvancer;
    protected KeyCode touchReculer;
    protected KeyCode touchDroite;
    protected KeyCode touchGauche;
    
    //Sprint
    protected KeyCode touchSprint;
    
    //Jump
    protected KeyCode touchJump;
    

    public TouchesClass()
    {
        Instance = this;
        touchJump = (KeyCode) System.Enum.Parse(typeof(KeyCode),
            PlayerPrefs.GetString("jumpKey", "Space"));
        touchSprint = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("sprintKey", "LeftShift"));
        touchGauche = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftKey", "Q"));
        touchDroite = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightKey", "D"));
        touchReculer = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("backwardKey", "S"));
        touchAvancer = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("forwardKey", "Z"));
        touchAccroupi = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("crouchKey", "C"));
        touchLeverAssoir = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("sitKey", "X"));
    }
    
    //getters
    public KeyCode GettouchAvancer() => touchAvancer;
    public KeyCode GettouchReculer() => touchReculer;
    public KeyCode GettouchGauche() => touchGauche;
    public KeyCode GettouchDroite() => touchDroite;
    public KeyCode GettouchJump() => touchJump;
    public KeyCode GettouchSprint() => touchSprint;
    public KeyCode GettouchAccroupi() => touchAccroupi;
    public KeyCode GettouchLeverAssoir() => touchLeverAssoir;
    //setters
    public void SettouchAvancer(KeyCode value)
    {
        touchAvancer = value;
    }
    public void SettouchReculer(KeyCode value)
    {
        touchReculer = value;
    }
    public void SettouchGauche(KeyCode value)
    {
        touchGauche = value;
    }
    public void SettouchDroite(KeyCode value)
    {
        touchDroite = value;
    }
    public void SettouchJump(KeyCode value)
    {
        touchJump = value;
    }
    public void SettouchSprint(KeyCode value)
    {
        touchSprint = value;
    }
    public void SettouchAccroupi(KeyCode value)
    {
        touchAccroupi = value;
    }
    public void SettouchLeverAssoir(KeyCode value)
    {
        touchLeverAssoir = value;
    }
}
