using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
   /*  Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;
    int currentResolutionIndex = 0;

    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);
            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        
        //##################################################
        
    }
    
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }*/
   
   private const string resolutionWidthPlayerPrefKey = "ResolutionWidth";
     private const string resolutionHeightPlayerPrefKey = "ResolutionHeight";
     private const string resolutionRefreshRatePlayerPrefKey = "RefreshRate";
     private const string fullScreenPlayerPrefKey = "FullScreen";
     [SerializeField] Toggle fullScreenToggle;
     [SerializeField] TMP_Dropdown resolutionDropdown;
     [SerializeField] private TMP_Dropdown graphicsDropdown;
     Resolution[] resolutions;
     Resolution selectedResolution;
     void Start()
     {
         resolutions = Screen.resolutions;
         LoadSettings();
         CreateResolutionDropdown();
         LoadQuality();
 
         fullScreenToggle.onValueChanged.AddListener(SetFullscreen);
         resolutionDropdown.onValueChanged.AddListener(SetResolution);
     }
 
     private void LoadSettings()
     {
         selectedResolution = new Resolution();
         selectedResolution.width = PlayerPrefs.GetInt(resolutionWidthPlayerPrefKey, Screen.currentResolution.width);
         selectedResolution.height = PlayerPrefs.GetInt(resolutionHeightPlayerPrefKey, Screen.currentResolution.height);
         selectedResolution.refreshRate = PlayerPrefs.GetInt(resolutionRefreshRatePlayerPrefKey, Screen.currentResolution.refreshRate);
         
         fullScreenToggle.isOn = PlayerPrefs.GetInt(fullScreenPlayerPrefKey, Screen.fullScreen ? 1 : 0) > 0;
 
         Screen.SetResolution(
             selectedResolution.width,
             selectedResolution.height,
             fullScreenToggle.isOn
         );
     }

     private void LoadQuality()
     {
         int qualityIndex = PlayerPrefs.GetInt("qualityGraphics", 0);
         graphicsDropdown.value = qualityIndex;
     }
    
 
     private void CreateResolutionDropdown()
     {
         resolutionDropdown.ClearOptions();
         List<string> options = new List<string>();
         int currentResolutionIndex = 0;
         for (int i = 0; i < resolutions.Length; i++)
         {
             string option = resolutions[i].width + " x " + resolutions[i].height;
             options.Add(option);
             if (Mathf.Approximately(resolutions[i].width, selectedResolution.width) && Mathf.Approximately(resolutions[i].height, selectedResolution.height))
             {
                 currentResolutionIndex = i;
             }
         }
         resolutionDropdown.AddOptions(options);
         resolutionDropdown.value = currentResolutionIndex;
         resolutionDropdown.RefreshShownValue();
     }
 
     public void SetFullscreen(bool isFullscreen)
     {
         Screen.fullScreen = isFullscreen;
         PlayerPrefs.SetInt(fullScreenPlayerPrefKey, isFullscreen ? 1 : 0);
     }
     public void SetResolution(int resolutionIndex)
     {
         selectedResolution = resolutions[resolutionIndex];
         Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
         PlayerPrefs.SetInt(resolutionWidthPlayerPrefKey, selectedResolution.width);
         PlayerPrefs.SetInt(resolutionHeightPlayerPrefKey, selectedResolution.height);
         PlayerPrefs.SetInt(resolutionRefreshRatePlayerPrefKey, selectedResolution.refreshRate);
     }
     
     public void SetQuality(int qualityIndex)
     {
         QualitySettings.SetQualityLevel(qualityIndex);
         PlayerPrefs.SetInt("qualityGraphics", qualityIndex);
     }
}