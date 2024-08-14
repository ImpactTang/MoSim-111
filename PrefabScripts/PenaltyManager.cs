using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PenaltyManager : MonoBehaviour
{
    [SerializeField] private Collider[] colliders;
    [SerializeField] private PenaltyCollisions[] sourceCollisions;

    [SerializeField] private Collider autoMiddleLineCollider;
    [SerializeField] private AudioSource errorSound;
    [SerializeField] private bool redAlliance;
    [SerializeField] private float penaltyCooldown;
    [SerializeField] private Alliance alliance;

    [SerializeField] private bool playerInsidePenaltyZone = false;
    [SerializeField] private bool playerInsideStage = false;
    [SerializeField] private bool playerPastAutoLine = false;

    private DriveController playerThatGotPenalty;
    private DriveController opponentThatGotPenalty;

    [SerializeField] private StageCollisionDetector stage;

    private bool scoreUpdated = false;
    private bool isTimerCounting = false;

    private int penaltyWorth = 5;

    private GameObject[] _players;
    private GameObject[] _enemys;

    private void Start()
    {
        _players = GameObject.FindGameObjectsWithTag("Player");
        _players = Array.FindAll(_players, player => player.activeSelf);
        _players = _players.Concat(Array.FindAll(GameObject.FindGameObjectsWithTag("Player2"), player => player.activeSelf)).ToArray();

        _enemys = GameObject.FindGameObjectsWithTag("RedPlayer");
        _enemys = Array.FindAll(_enemys, enemy => enemy.activeSelf);
        _enemys = _enemys.Concat(Array.FindAll(GameObject.FindGameObjectsWithTag("RedPlayer2"), enemy => enemy.activeSelf)).ToArray();
    }

    void Update()
    {
        if (!scoreUpdated && !isTimerCounting)
        {
            UpdateScore();
        }
        else if (!DriveController.robotsTouching && !isTimerCounting) 
        {
            scoreUpdated = false;
            isTimerCounting = false;
        }

        CheckForCollisions();
    }

    private void CheckForCollisions()
    {
        playerInsidePenaltyZone = false;

        foreach (var col in colliders)
        {
            foreach (var player in _players)
            {
                foreach (var enemy in _enemys)
                {
                    if (!col.bounds.Intersects(player.GetComponent<Collider>().bounds) &&
                        !col.bounds.Intersects(enemy.GetComponent<Collider>().bounds)) continue;
                    playerThatGotPenalty = enemy.GetComponent<DriveController>();
                    opponentThatGotPenalty = player.GetComponent<DriveController>();
                    playerInsidePenaltyZone = true;
                    break;
                }
            }
        }

        foreach (var penaltyCollisions in sourceCollisions)
        {
            if (!penaltyCollisions.inside) continue;
            playerThatGotPenalty = penaltyCollisions.playerInside;
            opponentThatGotPenalty = penaltyCollisions.enemyInside;
            playerInsidePenaltyZone = true;
            break;
        }

        playerInsideStage = stage.robotInStage;

        foreach (var player in _players)
        {
            playerPastAutoLine = autoMiddleLineCollider.bounds.Intersects(player.GetComponent<Collider>().bounds);
            if (!playerPastAutoLine) continue;
            opponentThatGotPenalty = player.GetComponent<DriveController>();
            break;
        }
    }

    private void UpdateScore()
    {
        if (!DriveController.robotsTouching) return;
        if (playerInsidePenaltyZone)
        {
            if (GameManager.GameState != GameState.End) 
            {
                if (GameManager.GameState == GameState.Auto)
                {
                    errorSound.Play();
                    AddScore(true, false);
                }
                else
                {
                    errorSound.Play();
                    AddScore(false, false);
                }
            }
        }

        if (playerInsideStage && GameManager.GameState == GameState.Endgame)
        {
            errorSound.Play();
            AddScore(false, false);
        }

        if (playerPastAutoLine && GameManager.GameState == GameState.Auto)
        {
            errorSound.Play();
            AddScore(true, true);
        }
    }

    private void AddScore(bool isAutoPoints, bool addScoreToOpponent)
    {
        StartCoroutine(addScoreToOpponent
            ? NoPenaltiesWhenThisIsRunning(opponentThatGotPenalty, penaltyCooldown)
            : NoPenaltiesWhenThisIsRunning(playerThatGotPenalty, penaltyCooldown));

        if (redAlliance) 
        {
            if (isAutoPoints)
            {
                if (addScoreToOpponent) { GameScoreTracker.BlueAutoPenaltyPoints += penaltyWorth; }
                else { GameScoreTracker.RedAutoPenaltyPoints += penaltyWorth; }
            }
            else 
            {
                if (addScoreToOpponent) { GameScoreTracker.BlueTeleopPenaltyPoints += penaltyWorth; }
                else { GameScoreTracker.RedTeleopPenaltyPoints += penaltyWorth; }
            }
            if (addScoreToOpponent) { Score.blueScore += penaltyWorth; }
            else { Score.redScore += penaltyWorth; }
        }
        else
        {
            if (isAutoPoints)
            {
                if (addScoreToOpponent) { GameScoreTracker.RedAutoPenaltyPoints += penaltyWorth; }
                else { GameScoreTracker.BlueAutoPenaltyPoints += penaltyWorth; }
            }
            else 
            {
                if (addScoreToOpponent) { GameScoreTracker.RedTeleopPenaltyPoints += penaltyWorth; }
                else { GameScoreTracker.BlueTeleopPenaltyPoints += penaltyWorth; }
            }
            if (addScoreToOpponent) { Score.redScore += penaltyWorth; }
            else { Score.blueScore += penaltyWorth; }
        }
        scoreUpdated = true;
    }

    private IEnumerator NoPenaltiesWhenThisIsRunning(DriveController controller, float duration)
    {
        controller.StartCoroutine(controller.GrayOutBumpers(duration));
        isTimerCounting = true;
        yield return new WaitForSeconds(duration);
        isTimerCounting = false;
    }
}
