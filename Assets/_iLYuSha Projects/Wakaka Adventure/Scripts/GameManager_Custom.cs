﻿using UnityEngine;
using UnityEngine.UI;

namespace WakakaAdventureSpace
{
    public partial class GameManager : Singleton<GameManager>
    {
        public Toggle[] toggleTheme;
        public Toggle[] toggleDiffculty;
        public Text[] textTheme;
        public Text[] textDiffculty;
        public GameObject[] lockTheme;
        public GameObject[] lockDiffculty;
        private int unlockTheme;
        private int unlockDiffculty;

        public void CheckUnlocked()
        {
            unlockTheme = 3; // Mathf.Max(1, PlayerPrefs.GetInt("Unlock Theme"));
            unlockDiffculty = 7; // Mathf.Max(1, PlayerPrefs.GetInt("Unlock Diffculty"));

            for (int i = 0; i < lockTheme.Length; i++)
            {
                if (i > unlockTheme - 2)
                {
                    toggleTheme[i].interactable = false;
                    textTheme[i].text = "???";
                    lockTheme[i].SetActive(true);
                }
                else
                {
                    toggleTheme[i].interactable = true;
                    textTheme[i].text = ((ThemeCht)(i + 2)).ToString();
                    lockTheme[i].SetActive(false);
                }
            }

            for (int i = 0; i < lockDiffculty.Length; i++)
            {
                if (i > unlockDiffculty - 2)
                {
                    toggleDiffculty[i].interactable = false;
                    textDiffculty[i].text = "???";
                    lockDiffculty[i].SetActive(true);
                }
                else
                {
                    toggleDiffculty[i].interactable = true;
                    textDiffculty[i].text = ((DifficultyCht)(i + 2)).ToString();
                    lockDiffculty[i].SetActive(false);
                }
            }
        }
    }
}