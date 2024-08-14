using UnityEngine;

public class NoteBugHandler : MonoBehaviour
{
    private GameObject _blueRobot;
    private GameObject _otherBlueRobot;
    private GameObject _redRobot;
    private GameObject _otherRedRobot;

    private RobotNoteManager _blueRing;
    private RobotNoteManager _redRing;
    private DriveController _blueDrive;
    private DriveController _redDrive;

    private RobotNoteManager _otherBlueRing;
    private RobotNoteManager _otherRedRing;
    private DriveController _otherBlueDrive;
    private DriveController _otherRedDrive;

    private bool _robotsGot;
    private bool _sameAlliance;
    private bool _isMultiplayer;
    private bool _isBlueAlliance;

    private void Start()
    {
        _isBlueAlliance = PlayerPrefs.GetString("alliance") == "blue";

        _sameAlliance = RobotSpawnController.sameAlliance;
        _isMultiplayer = RobotSpawnController.isMultiplayer;

        if (!_sameAlliance && _isMultiplayer)
        {
            _blueRobot = GameObject.FindGameObjectWithTag("Player");
            _blueRing = _blueRobot.GetComponent<RobotNoteManager>();
            _blueDrive = _blueRobot.GetComponent<DriveController>();

            _redRobot = GameObject.FindGameObjectWithTag("RedPlayer");
            _redRing = _redRobot.GetComponent<RobotNoteManager>();
            _redDrive = _redRobot.GetComponent<DriveController>();
        }
        else if (!_isMultiplayer && _sameAlliance)
        {
            if (_isBlueAlliance)
            {
                _blueRobot = GameObject.FindGameObjectWithTag("Player");
                _blueRing = _blueRobot.GetComponent<RobotNoteManager>();
                _blueDrive = _blueRobot.GetComponent<DriveController>();

                _otherBlueRobot = GameObject.FindGameObjectWithTag("Player2");
                if (_otherBlueRobot != null)
                {
                    _otherBlueRing = _otherBlueRobot.GetComponent<RobotNoteManager>();
                    _otherBlueDrive = _otherBlueRobot.GetComponent<DriveController>();
                }
            }
            else
            {
                _redRobot = GameObject.FindGameObjectWithTag("RedPlayer");
                _redRing = _redRobot.GetComponent<RobotNoteManager>();
                _redDrive = _redRobot.GetComponent<DriveController>();

                _otherRedRobot = GameObject.FindGameObjectWithTag("RedPlayer2");
                if (_otherRedRobot != null)
                {
                    _otherRedRing = _otherRedRobot.GetComponent<RobotNoteManager>();
                    _otherRedDrive = _otherRedRobot.GetComponent<DriveController>();
                }
            }
        }
        _robotsGot = true;
    }

    private void Update()
    {
        if (!_robotsGot) return;
        if (!_sameAlliance && _isMultiplayer)
        {
            if (_blueRing.hasRingInRobot || !_blueRing.ringWithinIntakeCollider || !(_blueDrive.intakeValue > 0) ||
                _redRing.hasRingInRobot || !_redRing.ringWithinIntakeCollider || !(_redDrive.intakeValue > 0)) return;
            //Randomly choose between blue and red robot
            var giveToBlue = Random.value < 0.5f; //50% chance for each robot

            if (giveToBlue)
            {
                _blueRing.ringWithinIntakeCollider = true;
                _redRing.ringWithinIntakeCollider = false;
            }
            else
            {
                _redRing.ringWithinIntakeCollider = true;
                _blueRing.ringWithinIntakeCollider = false;
            }
        }
        else if (!_isMultiplayer && _sameAlliance)
        {
            if (_isBlueAlliance)
            {
                if (_blueRing.hasRingInRobot || !_blueRing.ringWithinIntakeCollider || !(_blueDrive.intakeValue > 0) ||
                    _otherBlueRing.hasRingInRobot || !_otherBlueRing.ringWithinIntakeCollider ||
                    !(_otherBlueDrive.intakeValue > 0)) return;
                //Randomly choose between blue and other blue robot
                var giveToBlue = Random.value < 0.5f; //50% chance for each robot

                if (giveToBlue)
                {
                    _blueRing.ringWithinIntakeCollider = true;
                    _otherBlueRing.ringWithinIntakeCollider = false;
                }
                else
                {
                    _otherBlueRing.ringWithinIntakeCollider = true;
                    _blueRing.ringWithinIntakeCollider = false;
                }
            }
            else
            {
                if (_redRing.hasRingInRobot || !_redRing.ringWithinIntakeCollider || !(_redDrive.intakeValue > 0) ||
                    _otherRedRing.hasRingInRobot || !_otherRedRing.ringWithinIntakeCollider ||
                    !(_otherRedDrive.intakeValue > 0)) return;
                //Randomly choose between red and other red robot
                var giveToRed = Random.value < 0.5f; //50% chance for each robot

                if (giveToRed)
                {
                    _redRing.ringWithinIntakeCollider = true;
                    _otherRedRing.ringWithinIntakeCollider = false;
                }
                else
                {
                    _otherRedRing.ringWithinIntakeCollider = true;
                    _redRing.ringWithinIntakeCollider = false;
                }
            }
        }
    }
}