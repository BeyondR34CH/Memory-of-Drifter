using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public PlayerController player;
    public Image currentMapImage;
    public GameObject wind;
    public CinemachineConfiner confiner;
    public SceneInfo startScene;

    public static Camera mainCamera;
    public static PlayerInput playerinput;
    public static SettingData setting;
    public static bool inCombat;
    public static string controlScheme { get; private set; }

    public static event Action OnInspectInfo;
    public static event Action OnTransScene;
    public static event Action OnGameOver;

    private static Scene lastScene => SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
    private static CinemachineImpulseSource cameraImpulse;

    protected override void Awake()
    {
        Application.targetFrameRate = 60;
        setting = Resources.Load<SettingData>("SettingData");

        base.Awake();
        mainCamera = Camera.main;
        playerinput = GetComponent<PlayerInput>();
        cameraImpulse = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        if (SaveManager.ExitstsSaveData()) SaveManager.Load();
        else LoadScene(startScene);
    }

    public void ControlsChanged(PlayerInput input) => controlScheme = input.currentControlScheme;

    public static void CameraImpulse() => cameraImpulse.GenerateImpulse();

    public static void InspectInfo(InputAction.CallbackContext context)
    {
        if (context.performed) OnInspectInfo?.Invoke();
    }

    public static void EnterGameOver() => OnGameOver?.Invoke();

    public static void InChamber(bool value)
    {
        instance.wind.SetActive(!value);
        AudioManager.instance.noise.enabled = !value;
    }

    #region Scene

    public static void LoadScene(SceneInfo scene)
    {
        instance.StartCoroutine(OnLoadScene(scene));
    }

    private static IEnumerator OnLoadScene(SceneInfo scene)
    {
        yield return SceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(lastScene);
        instance.player.transform.position = scene.position;
        SaveManager.data.sceneInfo = scene;
    }

    public static void TransitionScene(SceneInfo scene)
    {
        OnTransScene?.Invoke();
        instance.StartCoroutine(OnTransitionScene(scene));
    }

    private static IEnumerator OnTransitionScene(SceneInfo scene)
    {
        if (SceneManager.sceneCount > 1)
        {
            yield return SceneManager.UnloadSceneAsync(lastScene);
        }
        yield return OnLoadScene(scene);
        SaveManager.Save();
    }

    #endregion
}
