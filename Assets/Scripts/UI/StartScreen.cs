using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace UI
{
    // src https://www.youtube.com/watch?v=CE9VOZivb3I
    public class StartScreen : MonoBehaviour
    {

        [SerializeField] private GameObject crossFadeObj;
        [SerializeField] private TextMeshProUGUI loadingText;
        [Header("Buttons")]
        [SerializeField] private Button startBtn;
        [SerializeField] private Button aboutBtn;
        [SerializeField] private Button closeBtn;
        [SerializeField] private Button settingsBtn;
        [SerializeField] private Button applyBtn;
        [SerializeField] private Button aboutCloseBtn;
        [SerializeField] private Button settingsCloseBtn;
        
        [Header("Settings Menu")] 
        [SerializeField] private Resolution[] _screenResolutions;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private float currentVolume;
        
        [Header("UI Panel Objects")]
        [SerializeField] private GameObject aboutMenu;
        [SerializeField] private GameObject settingsMenu;
        
        
        private static readonly int Start1 = Animator.StringToHash("Start");
        private Animator crossFadeAnim;
        private int _currentResolutionIndex;

        private bool _hasFaded;
        private void Start()
        {
            loadingText.enabled = false;
            // buttons
            startBtn.onClick.AddListener(StartGame);
            closeBtn.onClick.AddListener(CloseGame);
            aboutBtn.onClick.AddListener(ShowAboutMenu);
            aboutCloseBtn.onClick.AddListener(CloseAboutMenu);
            settingsBtn.onClick.AddListener(ShowSettingsMenu);
            settingsCloseBtn.onClick.AddListener(CloseSettingsMenu);
            // settings
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
            fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);
            volumeSlider.onValueChanged.AddListener(SetVolume);
            applyBtn.onClick.AddListener(SaveSettings);
            
            if (!crossFadeObj.activeSelf) crossFadeObj.SetActive(true);
            if (aboutMenu.activeSelf) aboutMenu.SetActive(false);
            if (settingsMenu.activeSelf) settingsMenu.SetActive(false);
            
            InitializeResolutions();
            LoadSettings(_currentResolutionIndex);
            
            crossFadeAnim = crossFadeObj.GetComponent<Animator>();
        }
        
        private void StartGame()
        {
            LoadNextScene();
        }



        private void ShowAboutMenu()
        {
            ToggleButtonInteractivity(false);
            if (!CheckButtonInteractivity()) return;
            CloseAllMenus();
            aboutMenu.SetActive(true);
        }

        private void CloseAboutMenu()
        {
            ToggleButtonInteractivity(true);
            if (!aboutMenu.activeSelf) return;
            aboutMenu.SetActive(false);
        }

        private void ShowSettingsMenu()
        {
            ToggleButtonInteractivity(true);
            if (!CheckButtonInteractivity()) return;
            CloseAllMenus();
            settingsMenu.SetActive(true);
        }

        private void CloseSettingsMenu()
        {
            ToggleButtonInteractivity(true);
            if (!settingsMenu.activeSelf) return;
            settingsMenu.SetActive(false);
        }

        private bool CheckButtonInteractivity()
        {
            return !(settingsMenu.activeSelf || aboutMenu.activeSelf);
        }

        private void ToggleButtonInteractivity(bool yn)
        {
            startBtn.interactable = yn;
            aboutBtn.interactable = yn;
            closeBtn.interactable = yn;
            settingsBtn.interactable = yn;
        }
        

        private static void CloseGame()
        {
            switch (Application.platform)
            {
#if UNITY_EDITOR
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.LinuxEditor:
                EditorApplication.ExitPlaymode();
                break;
#endif
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.LinuxPlayer:
                Application.Quit();
                break;
            }
        }

        private void LoadNextScene()
        {
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        }


        private void CloseAllMenus()
        {
            settingsMenu.SetActive(false);
            aboutMenu.SetActive(false);
            ToggleButtonInteractivity(true);
        }
        
        private void ToggleFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }
        
        
        private void SetResolution(int resolutionIndex)
        {
            var resolution = _screenResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
        
        private void SetVolume(float volume)
        {
            audioMixer.SetFloat("Volume", volume);
            currentVolume = volume;
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetInt("FullScreenPreference", Convert.ToInt32(Screen.fullScreen));
            PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);
            PlayerPrefs.SetFloat("VolumePreference", currentVolume);
        }

        private void LoadSettings(int resolutionIndex)
        {
            Screen.fullScreen = PlayerPrefs.HasKey("FullScreenPreference") &&
                                Convert.ToBoolean(PlayerPrefs.GetInt("FullScreenPreference"));

            resolutionDropdown.value = PlayerPrefs.HasKey("ResolutionPreference")
                ? PlayerPrefs.GetInt("ResolutionPreference")
                : resolutionIndex;

            volumeSlider.value = PlayerPrefs.HasKey("VolumePreference") ? PlayerPrefs.GetFloat("VolumePreference") : 1f;
        }

        private void InitializeResolutions()
        {
            resolutionDropdown.ClearOptions();
            var options = new List<string>();
            _screenResolutions = Screen.resolutions;
            for (var i = 0; i < _screenResolutions.Length; i++)
            {
                var option = _screenResolutions[i].width + " x " + _screenResolutions[i].height;
                options.Add(option);
                if (_screenResolutions[i].width == Screen.currentResolution.width &
                    _screenResolutions[i].height == Screen.currentResolution.height)
                    _currentResolutionIndex = i;
            }
            
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.RefreshShownValue();
        }

        private IEnumerator LoadLevel(int levelIndex)
        {
            loadingText.enabled = true;
            crossFadeAnim.SetTrigger(Start1);
            yield return new WaitForSeconds(1f);
            SceneManager.LoadSceneAsync(levelIndex);
        }



    }
}
