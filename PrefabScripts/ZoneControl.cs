using System;
using System.Linq;
using UnityEngine;

public class ZoneControl : MonoBehaviour
{
    public BoxCollider blueZone;
    public BoxCollider redZone;

    private GameObject _blueRobot;
    private GameObject _otherBlueRobot;

    private GameObject _redRobot;
    private GameObject _otherRedRobot;

    public static bool blueRobotInRedZone;
    public static bool blueRobotInRedZoneUpdated;

    public static bool blueOtherRobotInRedZone;
    public static bool blueOtherRobotInRedZoneUpdated;

    public static bool redRobotInBlueZone;
    public static bool redRobotInBlueZoneUpdated;

    public static bool redOtherRobotInBlueZone;
    public static bool redOtherRobotInBlueZoneUpdated;

    private bool _gotRobots;
    private bool _gotFirstRobot;

    public void Start()
    {
        _blueRobot = GameObject.FindGameObjectWithTag("Player");
        _otherBlueRobot = GameObject.FindGameObjectWithTag("Player2");
        _redRobot = GameObject.FindGameObjectWithTag("RedPlayer");
        _otherRedRobot = GameObject.FindGameObjectWithTag("RedPlayer2");

        if (_blueRobot != null || _redRobot != null)
        {
            _gotRobots = true;
        }
        else
        {
            throw new Exception("ZoneControl No Robots Found");
        }
    }

    private void Update()
    {
        if (!_gotRobots) return;
        redRobotInBlueZoneUpdated = false;
        redOtherRobotInBlueZoneUpdated = false;

        blueRobotInRedZoneUpdated = false;
        blueOtherRobotInRedZoneUpdated = false;

        if (_redRobot != null)
        {
            if (blueZone.bounds.Intersects(_redRobot.GetComponent<Collider>().bounds))
            {
                redRobotInBlueZoneUpdated = true;
            }

            if (_otherRedRobot != null && blueZone.bounds.Intersects(_otherRedRobot.GetComponent<Collider>().bounds))
            {
                redOtherRobotInBlueZoneUpdated = true;
            }
        }
        else if (_blueRobot != null)
        {
            if (redZone.bounds.Intersects(_blueRobot.GetComponent<Collider>().bounds))
            {
                blueRobotInRedZoneUpdated = true;
            }

            if (_otherBlueRobot != null && redZone.bounds.Intersects(_otherBlueRobot.GetComponent<Collider>().bounds))
            {
                blueOtherRobotInRedZoneUpdated = true;
            }
        }
    }

    public void CheckBlueZoneCollisions()
    {
        blueRobotInRedZone = false;
        blueOtherRobotInRedZone = false;

        if (redZone.bounds.Intersects(_blueRobot.GetComponent<Collider>().bounds))
        {
            blueRobotInRedZone = true;
        }
        else if (_otherBlueRobot != null && redZone.bounds.Intersects(_otherBlueRobot.GetComponent<Collider>().bounds))
        {
            blueOtherRobotInRedZone = true;
        }
    }

    public void CheckRedZoneCollisions()
    {
        redRobotInBlueZone = false;
        redOtherRobotInBlueZone = false;

        if (blueZone.bounds.Intersects(_redRobot.GetComponent<Collider>().bounds))
        {
            redRobotInBlueZone = true;
        }
        else if (_otherRedRobot != null && blueZone.bounds.Intersects(_otherRedRobot.GetComponent<Collider>().bounds))
        {
            redOtherRobotInBlueZone = true;
        }
    }
}