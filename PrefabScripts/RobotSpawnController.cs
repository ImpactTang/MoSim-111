using UnityEngine;
using UnityEngine.InputSystem;

public class RobotSpawnController : MonoBehaviour
{
    private int _gamemode;
    private int _cameraMode;
    private int _blueRobotIndex;
    private int _redRobotIndex;

    public static bool isMultiplayer;
    public static bool sameAlliance;

    [SerializeField] private GameObject[] blueRobotPrefabs;
    [SerializeField] private GameObject[] blueCameras;

    [SerializeField] private GameObject[] redRobotPrefabs;
    [SerializeField] private GameObject[] redCameras;

    [SerializeField] private GameObject[] secondaryBlueRobotPrefabs;
    [SerializeField] private GameObject[] secondaryBlueCameras;

    [SerializeField] private GameObject[] secondaryRedRobotPrefabs;
    [SerializeField] private GameObject[] secondaryRedCameras;

    [SerializeField] private GameObject cameraBorder;

    [SerializeField] private Transform blueSpawn;
    [SerializeField] private Transform secondaryBlueSpawn;
    [SerializeField] private Transform redSpawn;
    [SerializeField] private Transform secondaryRedSpawn;

    private ZoneControl _zoneCtrl;

    private void Start()
    {
        _zoneCtrl = FindFirstObjectByType<ZoneControl>();

        cameraBorder.SetActive(false);

        _gamemode = PlayerPrefs.GetInt("gamemode");
        _cameraMode = PlayerPrefs.GetInt("cameraMode");
        _redRobotIndex = PlayerPrefs.GetInt("redRobotSettings");
        _blueRobotIndex = PlayerPrefs.GetInt("blueRobotSettings");

        switch (_gamemode)
        {
            case 1:
                isMultiplayer = true;
                break;
            case 2:
                sameAlliance = true;
                break;
            default:
                sameAlliance = false;
                isMultiplayer = false;
                break;
        }

        HideAll();

        if (isMultiplayer)
        {
            cameraBorder.SetActive(true);

            blueRobotPrefabs[_blueRobotIndex].GetComponent<PlayerInput>().defaultControlScheme = "Controls 1";
            redRobotPrefabs[_redRobotIndex].GetComponent<PlayerInput>().defaultControlScheme = "Controls 2";

            Instantiate(redRobotPrefabs[_redRobotIndex], redSpawn.position, redSpawn.rotation);
            redCameras[_cameraMode + 3].SetActive(true);

            switch (_cameraMode)
            {
                case 0:
                    redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().isFieldCentric = true;
                    redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().startingReversed = true;
                    break;
                case 1:
                {
                    redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().isFieldCentric = true;

                    redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().startingReversed =
                        redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().robotType ==
                        RobotSettings.StealthRobotics;

                    break;
                }
                case 2:
                    redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().isFieldCentric = false;
                    redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().startingReversed = true;
                    break;
            }

            Instantiate(blueRobotPrefabs[_blueRobotIndex], blueSpawn.position, blueSpawn.rotation);
            blueCameras[_cameraMode + 3].SetActive(true);

            switch (_cameraMode)
            {
                case 0:
                    blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().isFieldCentric = true;
                    blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().startingReversed = true;
                    break;
                case 1:
                {
                    blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().isFieldCentric = true;
                    blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().startingReversed =
                        blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().robotType ==
                        RobotSettings.StealthRobotics;

                    break;
                }
                case 2:
                    blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().isFieldCentric = false;
                    blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().startingReversed = false;
                    break;
            }
        }
        else if (sameAlliance)
        {
            cameraBorder.SetActive(true);

            if (PlayerPrefs.GetString("alliance") == "red")
            {
                redRobotPrefabs[_redRobotIndex].GetComponent<PlayerInput>().defaultControlScheme = "Controls 1";
                secondaryRedRobotPrefabs[_blueRobotIndex].GetComponent<PlayerInput>().defaultControlScheme =
                    "Controls 2";

                Instantiate(redRobotPrefabs[_redRobotIndex], redSpawn.position, redSpawn.rotation);
                redCameras[_cameraMode + 3].SetActive(true);

                Instantiate(secondaryRedRobotPrefabs[_blueRobotIndex], secondaryRedSpawn.position,
                    secondaryRedSpawn.rotation);
                secondaryRedCameras[_cameraMode].SetActive(true);

                secondaryRedRobotPrefabs[_blueRobotIndex].GetComponent<RobotNoteManager>().isOtherRobot = true;

                switch (_cameraMode)
                {
                    case 0:
                        redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().isFieldCentric = true;
                        redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().startingReversed =
                            !redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().startingReversed;

                        secondaryRedRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().isFieldCentric = true;
                        secondaryRedRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().startingReversed =
                            !secondaryRedRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().startingReversed;
                        break;
                    case 1:
                    {
                        redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().isFieldCentric = true;
                        redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().startingReversed = false;

                        secondaryRedRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().isFieldCentric = true;
                        secondaryRedRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().startingReversed =
                            false;

                        if (redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().robotType ==
                            RobotSettings.StealthRobotics)
                        {
                            redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().startingReversed = true;
                        }

                        if (secondaryRedRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().robotType ==
                            RobotSettings.StealthRobotics)
                        {
                            secondaryRedRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().startingReversed =
                                true;
                        }

                        break;
                    }
                    case 2:
                        redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().isFieldCentric = false;
                        redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().startingReversed = true;

                        secondaryRedRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().isFieldCentric =
                            false;
                        secondaryRedRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().startingReversed =
                            true;
                        break;
                }
            }
            else
            {
                blueRobotPrefabs[_blueRobotIndex].GetComponent<PlayerInput>().defaultControlScheme = "Controls 1";
                secondaryBlueRobotPrefabs[_redRobotIndex].GetComponent<PlayerInput>().defaultControlScheme =
                    "Controls 2";

                Instantiate(blueRobotPrefabs[_blueRobotIndex], blueSpawn.position, blueSpawn.rotation);
                blueCameras[_cameraMode + 3].SetActive(true);

                Instantiate(secondaryBlueRobotPrefabs[_redRobotIndex], secondaryBlueSpawn.position,
                    secondaryBlueSpawn.rotation);
                secondaryBlueCameras[_cameraMode].SetActive(true);

                secondaryBlueRobotPrefabs[_redRobotIndex].GetComponent<RobotNoteManager>().isOtherRobot = true;

                switch (_cameraMode)
                {
                    case 0:
                        blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().isFieldCentric = true;
                        blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().startingReversed =
                            !blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().startingReversed;

                        secondaryBlueRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().isFieldCentric = true;
                        secondaryBlueRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().startingReversed =
                            !secondaryBlueRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().startingReversed;
                        break;
                    case 1:
                    {
                        blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().isFieldCentric = true;
                        blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().startingReversed = false;

                        secondaryBlueRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().isFieldCentric = true;
                        secondaryBlueRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().startingReversed =
                            false;

                        if (blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().robotType ==
                            RobotSettings.StealthRobotics)
                        {
                            blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().startingReversed = true;
                        }

                        if (secondaryBlueRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().robotType ==
                            RobotSettings.StealthRobotics)
                        {
                            secondaryBlueRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().startingReversed =
                                true;
                        }

                        break;
                    }
                    case 2:
                        blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().isFieldCentric = false;
                        blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().startingReversed = true;

                        secondaryBlueRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().isFieldCentric =
                            false;
                        secondaryBlueRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().startingReversed =
                            true;
                        break;
                }
            }
        }
        else
        {
            //Set correct robots & cameras active
            if (PlayerPrefs.GetString("alliance") == "red")
            {
                redRobotPrefabs[_redRobotIndex].GetComponent<PlayerInput>().defaultControlScheme = "Controls 1";

                Instantiate(redRobotPrefabs[_redRobotIndex], redSpawn.position, redSpawn.rotation);
                redCameras[_cameraMode].SetActive(true);

                if (_cameraMode == 0)
                {
                    redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().startingReversed =
                        !redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().startingReversed;
                    redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().isFieldCentric = true;
                }
                else if (_cameraMode == 1)
                {
                    redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().isFieldCentric = true;
                }
                else if (_cameraMode == 2)
                {
                    redRobotPrefabs[_redRobotIndex].GetComponent<DriveController>().isFieldCentric = false;
                }
            }
            else
            {
                blueRobotPrefabs[_blueRobotIndex].GetComponent<PlayerInput>().defaultControlScheme = "Controls 1";

                Instantiate(blueRobotPrefabs[_blueRobotIndex], blueSpawn.position, blueSpawn.rotation);
                blueCameras[_cameraMode].SetActive(true);

                if (_cameraMode == 0)
                {
                    blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().startingReversed =
                        !blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().startingReversed;
                    blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().isFieldCentric = true;
                }
                else if (_cameraMode == 1)
                {
                    blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().isFieldCentric = true;
                }
                else if (_cameraMode == 2)
                {
                    blueRobotPrefabs[_blueRobotIndex].GetComponent<DriveController>().isFieldCentric = false;
                }
            }
        }
    }

    private void HideAll()
    {
        foreach (var blueCamera in blueCameras)
        {
            blueCamera.SetActive(false);
        }

        foreach (var redCamera in redCameras)
        {
            redCamera.SetActive(false);
        }

        foreach (var blueCamera in secondaryBlueCameras)
        {
            blueCamera.SetActive(false);
        }

        foreach (var redCamera in secondaryRedCameras)
        {
            redCamera.SetActive(false);
        }
    }
}
