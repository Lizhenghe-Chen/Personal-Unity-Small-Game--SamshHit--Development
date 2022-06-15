using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    public TMP_Dropdown videoDropdown, levelDropdown;
    [SerializeField] private GameObject PlayerBunble, SpectatorBunble;
    public GameObject PlayerBunbleHUD, SpectatorBunbleHUD;
    public Transform PlayerPos, SpectatorPos;
    //public GameObject escUI;
    public CharacterCtrl characterCtrl;
    public Transform MainCamera;

    public CinemachineVirtualCamera PlayervCam, SpectatorvCam;
    public CinemachineFreeLook PlayerfreeLook;
    public Canvas escCanvas;
    CinemachineBrain cameraBrain;


    // Start is called before the first frame update
    void Awake()
    {
        escCanvas = this.GetComponent<Canvas>();
        videoDropdown.value = QualitySettings.GetQualityLevel();
        // levelDropdown.value = SceneManager.GetActiveScene().buildIndex;
        ChangeQualityLevel();
        cameraBrain = MainCamera.GetComponent<CinemachineBrain>();
        InGameMenu();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            InGameMenu();
        }
    }
    public void InGameMenu()
    {
        CancelInvoke();
        if (escCanvas.enabled)//if menu is active, switch it inactive
        {
            escCanvas.enabled = false;

            Cursor.visible = false;
            cameraBrain.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            SpectatorBunbleHUD.SetActive(false);
            //PlayerBunbleHUD.SetActive(false);
            //Time.timeScale = 1f;
            // Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
        else// if menu is inactive, switch it active,open the menu
        {
            escCanvas.enabled = true;
            cameraBrain.enabled = false;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SpectatorBunbleHUD.SetActive(true);
            //PlayerBunbleHUD.SetActive(true);
            Time.timeScale = 0.0001f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            //   Invoke("UnableCinemachineBrain", 1f * Time.timeScale);
        }

    }
    public void QuitGame()
    {
        Application.Quit();
    }


    public void SwitchBunble()
    {
        CancelInvoke();
        cameraBrain.enabled = true;
        if (characterCtrl.enabled)
        {
            characterCtrl.enabled = false;
            PlayervCam.gameObject.SetActive(false);
            PlayerfreeLook.gameObject.SetActive(false);
            PlayerBunbleHUD.SetActive(false);
            SpectatorBunble.SetActive(true);
            //PlayerBunble.SetActive(false);
            SwitchBubleWithCamera();


        }
        else
        {
            characterCtrl.enabled = true;
            // PlayervCam.gameObject.SetActive(true);
            PlayerfreeLook.gameObject.SetActive(true);
            PlayerBunbleHUD.SetActive(true);

            // PlayerBunble.SetActive(true);
            SpectatorBunble.SetActive(false);
            //PlayerPos.position = SpectatorPos.position;
        }
        Invoke(nameof(UnableCinemachineBrain), 1f * Time.timeScale);
    }


    public void ChangeQualityLevel()
    {
        QualitySettings.SetQualityLevel(videoDropdown.value, true);

    }
    public void ChangeLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex == levelDropdown.value)
        {
            return;
        }
        PlayerPos.parent.position += new Vector3(0, 2, 0);
        Debug.Log(levelDropdown.value);
        SceneManager.LoadScene(levelDropdown.value);
        Time.timeScale = 0.01f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

    }
    void SwitchBubleWithCamera()
    { //let two bundle's camera have same direction

        if (PlayervCam.gameObject.activeSelf)
        {
            SpectatorvCam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = PlayervCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value;
            // SpectatorvCam.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = PlayervCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value;
        }
        else if (PlayerfreeLook.gameObject.activeSelf)
        {
            SpectatorvCam.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = PlayerfreeLook.m_XAxis.Value;
            SpectatorvCam.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = PlayerfreeLook.m_YAxis.Value * 90;
        }
        SpectatorPos.position = MainCamera.position;
    }
    void UnableCinemachineBrain()
    {
        cameraBrain.enabled = false;
        Debug.Log("UnableCinemachineBrain");
    }
}

