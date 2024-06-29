using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionUIController : MonoBehaviour
{
    public AudioMixer mixer;
    [Space]
    public Slider music;
    public Slider sound;
    public Toggle muteAudio;
    public Toggle fullScreen;
    public HoldButtonController saveAndExit;
    public HoldButtonController resetSaveData;

    private HoldButtonController currentButton;
    private float TransToVolume(float value) => value == 0 ? -80 : value * 4 - 20;

    public void ChangeMusicAudio(float value)
    {
        AudioManager.Play(AudioType.Select);
        PlayerPrefs.SetFloat("Music", value);
        mixer.SetFloat("Music", TransToVolume(value));
    }

    public void ChangeSoundAudio(float value)
    {
        AudioManager.Play(AudioType.Select);
        PlayerPrefs.SetFloat("Sound", value);
        mixer.SetFloat("Sound", TransToVolume(value));
    }

    public void MuteAudio(bool mute)
    {
        AudioManager.Play(AudioType.Select);
        PlayerPrefs.SetInt("MuteAudio", mute ? 1 : 0);
        mixer.SetFloat("Master", mute ? -80 : 0);
    }

    public void FullScreen(bool full)
    {
        AudioManager.Play(AudioType.Select);
        PlayerPrefs.SetInt("FullScreen", full ? 1 : 0);
        Screen.SetResolution(full ? 1920 : 1280, full ? 1080 : 720, full);
    }

    public void SaveAndExit()
    {
        Debug.Log("保存并退出游戏");
        AudioManager.Play(AudioType.UnequipItem);
        PlayerPrefs.Save();
        SaveManager.Save();
        Application.Quit();
    }

    public void ResetSaveData()
    {
        Debug.Log("重置存档");
        AudioManager.Play(AudioType.UnequipItem);
        GameManager.instance.player.Reborn();
        GetComponent<PlayerUIController>().state.Transition(PlayerUIStateType.Normal);
        SaveManager.Reset();
    }

    public void EnterHoldButton(InputAction.CallbackContext context)
    {
        if (currentButton)
        {
            if (context.performed)
            {
                AudioManager.Play(AudioType.Select);
                currentButton.isHold = true;
            }
            else if (context.canceled)
            {
                currentButton.isHold = false;
            }
        }
    }

    private void Awake()
    {
        saveAndExit.SetCurrentButton += (button) => currentButton = button;
        saveAndExit.OnPerform += SaveAndExit;
        resetSaveData.SetCurrentButton += (button) => currentButton = button;
        resetSaveData.OnPerform += ResetSaveData;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Music"))
        {
            music.value = PlayerPrefs.GetFloat("Music");
            mixer.SetFloat("Music", TransToVolume(PlayerPrefs.GetFloat("Music")));
        }
        if (PlayerPrefs.HasKey("Sound"))
        {
            sound.value = PlayerPrefs.GetFloat("Sound");
            mixer.SetFloat("Sound", TransToVolume(PlayerPrefs.GetFloat("Sound")));
        }
        if (PlayerPrefs.HasKey("MuteAudio"))
        {
            muteAudio.isOn = PlayerPrefs.GetInt("MuteAudio") == 1 ? true : false;
            mixer.SetFloat("Master", PlayerPrefs.GetInt("MuteAudio") == 1 ? 0 : -80);
        }
        if (PlayerPrefs.HasKey("FullScreen"))
        {
            fullScreen.isOn = PlayerPrefs.GetInt("FullScreen") == 1 ? true : false;
            Screen.fullScreen = PlayerPrefs.GetInt("FullScreen") == 1 ? true : false;
        }
    }
}
