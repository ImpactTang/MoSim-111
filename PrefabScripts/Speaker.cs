using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class Speaker : MonoBehaviour
{
    [SerializeField] private Alliance alliance;

    [SerializeField] private Collider speakerCollider;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioResource scoreSound;
    [SerializeField] private AudioResource ampedScoreSound;

    [field: SerializeField] public int numOfStoredNotes { get; set; }
    
    private DriveController[] _drives;

    private const float NoteScoreRegisterDelay = 1.5f;

    private void Start()
    {
        //Set starting values for instance variables
        numOfStoredNotes = 0;
        
        GameObject[] players;
        
        if (alliance == Alliance.Blue)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            players = players.Concat(GameObject.FindGameObjectsWithTag("Player2")).ToArray();
        }
        else
        {
            players = GameObject.FindGameObjectsWithTag("RedPlayer");
            players = players.Concat(GameObject.FindGameObjectsWithTag("RedPlayer2")).ToArray();
        }
        
        _drives = new DriveController[players.Length];
        for (var i = 0; i < players.Length; i++)
        {
            _drives[i] = players[i].GetComponent<DriveController>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Ring") &&
            !other.gameObject.CompareTag("noteShotByRed") &&
            !other.gameObject.CompareTag("noteShotByBlue") &&
            !other.gameObject.CompareTag("noteShotByBlue2") &&
            !other.gameObject.CompareTag("noteShotByRed2")) return;
        if (!speakerCollider.bounds.Intersects(other.bounds)) return;
        if (DriveController.isAmped && alliance == Alliance.Blue ||
            DriveController.isRedAmped && alliance == Alliance.Red)
        {
            numOfStoredNotes++;
            if (numOfStoredNotes > 4)
            {
                foreach (var drive in _drives)
                {
                    if (!drive.isActiveAndEnabled) continue;
                    drive.StopAmplifiedSpeaker();
                    Debug.Log("Drive amplification stopped");
                    break;
                }

                ResetNotes();
            }
        }

        StartCoroutine(ScoreDelay());

        Destroy(other.gameObject);
    }

    private IEnumerator ScoreDelay()
    {
        var isLegalScore = GameManager.GameState != GameState.End;
        var isAmped = DriveController.isAmped && alliance == Alliance.Blue ||
                      DriveController.isRedAmped && alliance == Alliance.Red;

        yield return new WaitForSeconds(NoteScoreRegisterDelay);

        if (!isLegalScore) yield break;
        switch (alliance)
        {
            case Alliance.Red:
                if (GameManager.GameState == GameState.Auto)
                {
                    source.resource = scoreSound;
                    GameScoreTracker.RedAutoSpeakerPoints += 5;
                    Score.redScore += 5;
                }
                else if (isAmped)
                {
                    source.resource = ampedScoreSound;
                    GameScoreTracker.RedTeleopSpeakerPoints += 5;
                    Score.redScore += 5;
                }
                else
                {
                    source.resource = scoreSound;
                    GameScoreTracker.RedTeleopSpeakerPoints += 2;
                    Score.redScore += 2;
                }

                break;
            case Alliance.Blue:
                if (GameManager.GameState == GameState.Auto)
                {
                    source.resource = scoreSound;
                    GameScoreTracker.BlueAutoSpeakerPoints += 5;
                    Score.blueScore += 5;
                }
                else if (isAmped)
                {
                    source.resource = ampedScoreSound;
                    GameScoreTracker.BlueTeleopSpeakerPoints += 5;
                    Score.blueScore += 5;
                }
                else
                {
                    source.resource = scoreSound;
                    GameScoreTracker.BlueTeleopSpeakerPoints += 2;
                    Score.blueScore += 2;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        source.Play();
    }

    public void ResetNotes()
    {
        numOfStoredNotes = 0;
    }

    public void ResetSpeaker()
    {
        numOfStoredNotes = 0;
        StopAllCoroutines();
    }
}