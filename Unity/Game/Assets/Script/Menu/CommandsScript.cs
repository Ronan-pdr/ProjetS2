using System.Collections;
using System.Collections.Generic;
using Script.EntityPlayer;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommandsScript : MonoBehaviour
{
    // ------------ SerializedField ------------
    
    [Header("Button")]
    [SerializeField] Button ForwardButton;
    [SerializeField] Button BackwardButton;
    [SerializeField] Button LeftButton;
    [SerializeField] Button RightButton;
    [SerializeField] Button JumpButton;
    [SerializeField] Button SprintButton;
    [SerializeField] Button CrouchButton;
    [SerializeField] Button SitButton;
    
    // ------------ Attributs ------------
    private Event keyEvent;
    private KeyCode newKey;
    private bool waitingForKey;
    private TMP_Text buttonText;
    TouchesClass touches;
    
    // ------------ Constructeurs ------------
    void Start()
    {
        touches = TouchesClass.Instance;
        
        waitingForKey = false;
        ForwardButton.GetComponentInChildren<TMP_Text>().text = touches.GettouchAvancer().ToString();
        BackwardButton.GetComponentInChildren<TMP_Text>().text = touches.GettouchReculer().ToString();
        LeftButton.GetComponentInChildren<TMP_Text>().text = touches.GettouchGauche().ToString();
        RightButton.GetComponentInChildren<TMP_Text>().text = touches.GettouchDroite().ToString();
        JumpButton.GetComponentInChildren<TMP_Text>().text = touches.GettouchJump().ToString();
        SprintButton.GetComponentInChildren<TMP_Text>().text = touches.GettouchSprint().ToString();
        CrouchButton.GetComponentInChildren<TMP_Text>().text = touches.GettouchAccroupi().ToString();
        SitButton.GetComponentInChildren<TMP_Text>().text = touches.GettouchLeverAssoir().ToString();
    }

    // ------------ SerializedField ------------
    void OnGUI()
    {
        keyEvent = Event.current;
        if (keyEvent.isKey && waitingForKey)
        {
            newKey = keyEvent.keyCode;
            waitingForKey = false;
        }
    }

    public void StartAssignment(string keyName)
    {
        if (!waitingForKey)
        {
            StartCoroutine(AssignKey(keyName));
        }
    }

    public void SendText(TMP_Text text)
    {
        buttonText = text;
    }

    IEnumerator WaitForKey()
    {
        while (!keyEvent.isKey)
            yield return null;
    }

    public IEnumerator AssignKey(string keyName)
    {
        waitingForKey = true;
        yield return WaitForKey();
        switch (keyName)
        {
            case "forward":
                touches.SettouchAvancer(newKey);
                buttonText.text = touches.GettouchAvancer().ToString();
                PlayerPrefs.SetString("forwardKey", touches.GettouchAvancer().ToString());
                break;
            case "backward":
                touches.SettouchReculer(newKey);
                buttonText.text = touches.GettouchReculer().ToString();
                PlayerPrefs.SetString("backwardKey", touches.GettouchReculer().ToString());
                break;
            case "left":
                touches.SettouchGauche(newKey);
                buttonText.text = touches.GettouchGauche().ToString();
                PlayerPrefs.SetString("leftKey", touches.GettouchGauche().ToString());
                break;
            case "right":
                touches.SettouchDroite(newKey);
                buttonText.text = touches.GettouchDroite().ToString();
                PlayerPrefs.SetString("rightKey", touches.GettouchDroite().ToString());
                break;
            case "jump":
                touches.SettouchJump(newKey);
                buttonText.text = touches.GettouchJump().ToString();
                PlayerPrefs.SetString("jumpKey", touches.GettouchJump().ToString());
                break;
            case "sprint":
                touches.SettouchSprint(newKey);
                buttonText.text = touches.GettouchSprint().ToString();
                PlayerPrefs.SetString("sprintKey", touches.GettouchSprint().ToString());
                break;
            case "crouch":
                touches.SettouchAccroupi(newKey);
                buttonText.text = touches.GettouchAccroupi().ToString();
                PlayerPrefs.SetString("crouchKey", touches.GettouchAccroupi().ToString());
                break;
            case "sit":
                touches.SettouchLeverAssoir(newKey);
                buttonText.text = touches.GettouchLeverAssoir().ToString();
                PlayerPrefs.SetString("sitKey", touches.GettouchLeverAssoir().ToString());
                break;
        }

        yield return null;
    }
}
