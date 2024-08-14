using System;
using Unity.Mathematics;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    [SerializeField] private Alliance alliance;
    [SerializeField] private bool isSecondaryCam;

    public float smoothSpeed = 5f;
    public Vector2 rotationConstraints = new Vector2(-90, 90);
    public float closeDistanceThreshold = 2f;
    public float forwardMoveDistance = 5f;

    public static bool parentMoving = false;
    private bool _move = true;

    private Vector3 _targetPosition;
    private Transform _target;

    [SerializeField] private Transform follow;

    private void Start()
    {
        _target = GetEnabledTarget();
        _targetPosition = follow.position;

        if (alliance == Alliance.Red) { forwardMoveDistance = -forwardMoveDistance; }
    }

    private void LateUpdate()
    {
        if (_target is null) return;

        var distance = Vector3.Distance(transform.position, _target.position);

        if (parentMoving)
        {
            _move = true;
            _targetPosition = follow.position;
        }
        else
        {
            if (distance < closeDistanceThreshold && _move)
            {
                _move = false;
                _targetPosition = new Vector3(transform.position.x + forwardMoveDistance, transform.position.y, transform.position.z);
            }
            else if (distance >= closeDistanceThreshold && !_move)
            {
                _move = true;
            }
        }

        if (_move) 
        {
            _targetPosition = follow.position;
        }
        
        transform.position = Vector3.Lerp(transform.position, _targetPosition, smoothSpeed * Time.deltaTime);

        transform.LookAt(_target);
    }

    private Transform GetEnabledTarget()
    {
        if (alliance == Alliance.Blue)
        {
            return isSecondaryCam ? GameObject.FindGameObjectWithTag("Player2").transform : GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        return isSecondaryCam ? GameObject.FindGameObjectWithTag("RedPlayer2").transform : GameObject.FindGameObjectWithTag("RedPlayer").transform;
    }
}
