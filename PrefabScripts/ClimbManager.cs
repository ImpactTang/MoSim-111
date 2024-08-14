using System.Linq;
using UnityEngine;

public class ClimbManager : MonoBehaviour
{
    private DriveController _drive;

    private void Start()
    {
        _drive = gameObject.GetComponent<DriveController>();
    }

    private void Update()
    {
        if (GameManager.GameState != GameState.Endgame && !GameManager.endBuzzerPlaying) return;
        
        if (!_drive.robotClimbs) return;
        
        if (_drive.isRedRobot)
        {
            if (!_drive.isTouchingGround && !_drive.isClimbed && ChainDetector.isRedTouchingChain)
            {
                _drive.isClimbed = true;
                GameScoreTracker.RedStagePoints += 3;
                Score.redScore += 3;
            }
            else if (_drive.isClimbed && _drive.isTouchingGround)
            {
                _drive.isClimbed = false;
                GameScoreTracker.RedStagePoints -= 3;
                Score.redScore -= 3;
            }
        }
        else
        {
            if (!_drive.isTouchingGround && !_drive.isClimbed && ChainDetector.isBlueTouchingChain)
            {
                _drive.isClimbed = true;
                GameScoreTracker.BlueStagePoints += 3;
                Score.blueScore += 3;
            }
            else if (_drive.isClimbed && _drive.isTouchingGround)
            {
                _drive.isClimbed = false;
                GameScoreTracker.BlueStagePoints -= 3;
                Score.blueScore -= 3;
            }
        }
    }
}