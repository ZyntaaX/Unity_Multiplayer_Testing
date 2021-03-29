using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    [SerializeField] private Text errorText = null;
    [System.Serializable]
    public struct MenuHolder {
        public string name;
        public GameObject holder;

        public MenuHolder(string _name, GameObject _holder) {
            name = _name;
            holder = _holder;
        }
    }

    // Public
    [Header("Menu Holders")]
    public MenuHolder[] menuHolders = null;

    // Private
    int currentActiveHolderIndex;

    // Start is called before the first frame update
    public void Awake() {
        for (int i = 0; i < menuHolders.Length; i++) {
            if (menuHolders[i].holder.activeSelf) {
                currentActiveHolderIndex = i;
            }
        }
    }

    private void Start() {
        Screen.fullScreen = false;
    }

    public void Quit() {
        Application.Quit();
    }

    public void NavigateToMenu(string menuName) {
        for (int i = 0; i < menuHolders.Length; i++) {
            if (menuHolders[i].name == menuName) {
                menuHolders[i].holder.SetActive(true);
                currentActiveHolderIndex = i;
            } else {
                menuHolders[i].holder.SetActive(false);
            }
        }
    }

    public void NavigateToMenu(string menuName, string errorMessage) {
        errorText.text = $"Error: {errorMessage}";
        NavigateToMenu(menuName);
    }
}
