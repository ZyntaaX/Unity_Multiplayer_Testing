using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {
    // Public
    [SerializeField] private Toggle fullscreenToggle = null;

    // Private
    bool isFullscreen;

    // Start is called before the first frame update
    void Start() {
        isFullscreen = PlayerPrefs.GetInt(PlayerPrefStrings.playerFullscreenPref) == 1 ? true : false;

        fullscreenToggle.isOn = isFullscreen;

        UpdateDisplaySettings();
    }

    public void SetFullScreen(bool value) {
        isFullscreen = value;

        PlayerPrefs.SetInt(PlayerPrefStrings.playerFullscreenPref, (value ? 1 : 0));
        PlayerPrefs.Save();

        UpdateDisplaySettings();
    }

    private void UpdateDisplaySettings() {
        Screen.fullScreen = isFullscreen;
    }
}
