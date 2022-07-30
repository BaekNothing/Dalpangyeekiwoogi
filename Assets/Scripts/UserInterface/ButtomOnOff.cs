using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtomOnOff : MonoBehaviour
{

    public GameObject[] buttonSkins;
    public Text[] buttonSkinTexts;
    public GameObject[] NoStamina;

    public void onoff( int indexnumber)
    {
        for (int i = 0; i<buttonSkins.Length; i++){
            buttonSkins[i].SetActive(true);
            NoStamina[i].SetActive(false);
        }

        Debug.Log((int.Parse(buttonSkinTexts[indexnumber].text) * -1).ToString());

        if( int.Parse(buttonSkinTexts[indexnumber].text) * -1 > PlayerPrefs.GetFloat("Stamina") )
        {
            NoStamina[indexnumber].SetActive(true);
        }
        else 
        { buttonSkins[indexnumber].SetActive(false);}
    }

    public void off() 
    
    {
        for (int i = 0; i < buttonSkins.Length; i++)
        {
            buttonSkins[i].SetActive(true);
            NoStamina[i].SetActive(false);
        }

    }

    public GameObject JustPanel;

    public void _JustPanelOnOff(){

        JustPanel.SetActive(!JustPanel.activeSelf);    

    }

    public void _justPanelOFF(){

        JustPanel.SetActive(false);
    
    }

}
