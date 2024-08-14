using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class DriveController : MonoBehaviour, IResettable
{
    public RobotSettings robotType;
    [SerializeField] private TMP_Text[] bumperNumbers;
    [SerializeField] private bool reverseBumperAllianceText;

    [SerializeField] private Transform[] rayCastPoints;
    [SerializeField] private float rayCastDistance;
    [SerializeField] private bool flipRayCastDir;

    [SerializeField] private bool flipStartingReverse;

    [SerializeField] private Collider field;

    public Collider blueStage;
    public Collider redStage;

    private SourceRingLoader _sourceRingLoader;

    //Handles climbing logic
    public bool isGrounded = true;
    public bool isTouchingGround = true;
    public bool isClimbed;
    public bool robotClimbs;

    public AudioSource redCountDown;
    public AudioSource blueCountDown;
    public AudioSource robotPlayer;
    public AudioSource treadPlayer;
    public AudioSource gearPlayer;
    public AudioResource intakeSound;
    public AudioResource swerveSound;
    public AudioResource gearSound;
    public float moveSpeed = 20f;
    public float rotationSpeed = 15f;
    public bool isRedRobot;
    public bool areRobotsTouching;
    public bool startingReversed;

    public bool is930;

    public static bool canBlueRotate;
    public static bool canRedRotate;
    public static bool isTouchingWallColliderRed;
    public static bool isTouchingWallColliderBlue;
    public Vector3 velocity { get; set; }
    public bool canIntake { get; set; }
    public static bool robotsTouching;
    public static bool isPinningRed;
    public static bool isPinningBlue;
    public static bool isAmped;
    public static bool isRedAmped;
    public bool isIntaking;

    private Rigidbody _rb;
    private Vector2 _translateValue;
    private float _rotateValue;
    private Vector3 _startingDirection;
    private Vector3 _startingRotation;
    public float intakeValue;
    private bool _ampSpeaker;

    private bool _dontPlayDriveSounds;
    private bool _useSwerveSounds;
    private bool _useIntakeSounds;

    public Material materialPrefab;
    [SerializeField] private GameObject bumper;
    private Material _bumperMat;
    private Color _defaultBumperColor;

    [SerializeField] private Amp allianceAmp;
    [SerializeField] private Speaker allianceSpeaker;

    public float beforeVelocity;
    private bool _dontUpdateBeforeVelocity;

    private Vector3 _centerOfMass;

    [SerializeField] private float maxAngularVelocity = 5f;
    public bool isFieldCentric;

    private Coroutine _ampTimerCoroutine;

    private GameManager _gameManager;

    private Vector3 _startingPos;
    private Quaternion _startingRot;

    public bool atTargetPos;
    public bool atTargetRot;

    private void Start()
    {
        canIntake = true;

        _startingPos = transform.position;
        _startingRot = transform.rotation;

        if (materialPrefab != null)
        {
            _bumperMat = Instantiate(materialPrefab);

            if (is930)
            {
                Material[] mat = bumper.GetComponent<Renderer>().materials;
                mat[3] = _bumperMat;
                mat[4] = _bumperMat;
            }
            else
            {
                bumper.GetComponent<Renderer>().material = _bumperMat;
            }

            _defaultBumperColor = _bumperMat.color;
        }
        else
        {
            Debug.LogError("Material prefab is not assigned!");
        }

        if (!reverseBumperAllianceText)
        {
            if (isRedRobot && PlayerPrefs.GetString("redName") != "")
            {
                foreach (TMP_Text bumperNumber in bumperNumbers)
                {
                    bumperNumber.text = PlayerPrefs.GetString("redName");
                }
            }
            else if (!isRedRobot && PlayerPrefs.GetString("blueName") != "")
            {
                foreach (TMP_Text bumperNumber in bumperNumbers)
                {
                    bumperNumber.text = PlayerPrefs.GetString("blueName");
                }
            }
        }
        else
        {
            if (isRedRobot && PlayerPrefs.GetString("blueName") != "")
            {
                foreach (TMP_Text bumperNumber in bumperNumbers)
                {
                    bumperNumber.text = PlayerPrefs.GetString("blueName");
                }
            }
            else if (!isRedRobot && PlayerPrefs.GetString("redName") != "")
            {
                foreach (TMP_Text bumperNumber in bumperNumbers)
                {
                    bumperNumber.text = PlayerPrefs.GetString("redName");
                }
            }
        }


        _useSwerveSounds = PlayerPrefs.GetInt("swerveSounds") == 1;
        _useIntakeSounds = PlayerPrefs.GetInt("intakeSounds") == 1;

        treadPlayer.resource = swerveSound;
        treadPlayer.loop = true;

        gearPlayer.resource = gearSound;
        gearPlayer.loop = true;

        moveSpeed = moveSpeed - (moveSpeed * (PlayerPrefs.GetFloat("movespeed") / 100f));
        rotationSpeed = rotationSpeed - (rotationSpeed * (PlayerPrefs.GetFloat("rotatespeed") / 100f));

        //Resetting static variables on start
        canBlueRotate = true;
        canRedRotate = true;

        isTouchingWallColliderRed = false;
        isTouchingWallColliderBlue = false;

        isPinningRed = false;
        isPinningBlue = false;
        robotsTouching = false;
        velocity = new Vector3(0f, 0f, 0f);
        isAmped = false;
        isRedAmped = false;
        isIntaking = false;

        //Initializing starting transforms
        _rb = GetComponent<Rigidbody>();

        if (flipStartingReverse)
        {
            startingReversed = !startingReversed;
        }

        if (!startingReversed)
        {
            _startingDirection = gameObject.transform.forward;
            _startingRotation = gameObject.transform.right;
        }
        else
        {
            _startingDirection = -gameObject.transform.forward;
            _startingRotation = -gameObject.transform.right;
        }

        _gameManager = GameObject.Find("GameGUI").GetComponent<GameManager>();

        field = GameObject.Find("FieldTriggerCollider").GetComponent<Collider>();
        blueStage = GameObject.Find("BlueStageCollider").GetComponent<Collider>();
        redStage = GameObject.Find("RedStageCollider").GetComponent<Collider>();
        
        blueCountDown = GameObject.Find("BlueCountDown").GetComponent<AudioSource>();
        redCountDown = GameObject.Find("RedCountDown").GetComponent<AudioSource>();
        
        allianceAmp = isRedRobot ? GameObject.Find("RedAmp").GetComponent<Amp>() : GameObject.Find("BlueAmp").GetComponent<Amp>();
        allianceSpeaker = isRedRobot ? GameObject.Find("RedSpeaker").GetComponent<Speaker>() : GameObject.Find("BlueSpeaker").GetComponent<Speaker>();
        _sourceRingLoader = isRedRobot
            ? GameObject.Find("RedSource").GetComponent<SourceRingLoader>()
            : GameObject.Find("BlueSource").GetComponent<SourceRingLoader>();
    }

    private void Update()
    {
        isGrounded = CheckGround();
        _rb.centerOfMass = _centerOfMass;
        areRobotsTouching = robotsTouching;

        if (GameManager.GameState == GameState.Endgame || GameManager.endBuzzerPlaying)
        {
            if (robotClimbs)
            {
                isTouchingGround = CheckTouchingGround();
            }
        }

        if (!_dontUpdateBeforeVelocity)
        {
            if (!isTouchingWallColliderBlue && !isRedRobot || !isTouchingWallColliderRed && isRedRobot)
            {
                beforeVelocity = _rb.velocity.magnitude;
            }
        }

        if (!isRedRobot)
        {
            if (robotsTouching && isTouchingWallColliderBlue)
            {
                isPinningBlue = true;
            }
            else
            {
                isPinningBlue = false;
            }
        }
        else
        {
            if (robotsTouching && isTouchingWallColliderRed)
            {
                isPinningRed = true;
            }
            else
            {
                isPinningRed = false;
            }
        }


        if (!isRedRobot)
        {
            if (_ampSpeaker && allianceAmp.numOfStoredNotes >= 2)
            {
                if (GameManager.GameState != GameState.Auto)
                {
                    blueCountDown.Play();
                    isAmped = true;
                    AmplifySpeaker();
                    allianceAmp.ResetStoredNotes();
                }
            }
        }
        else
        {
            if (_ampSpeaker && allianceAmp.numOfStoredNotes >= 2)
            {
                if (GameManager.GameState != GameState.Auto)
                {
                    redCountDown.Play();
                    isRedAmped = true;
                    AmplifySpeaker();
                    allianceAmp.ResetStoredNotes();
                }
            }
        }

        if (intakeValue > 0f && GameManager.canRobotMove && canIntake)
        {
            robotPlayer.resource = intakeSound;
            isIntaking = true;
        }
        else
        {
            isIntaking = false;
        }

        if (_useIntakeSounds)
        {
            if (isIntaking && !robotPlayer.isPlaying)
            {
                robotPlayer.Play();
            }
            else if (!isIntaking && robotPlayer.isPlaying)
            {
                robotPlayer.Stop();
            }
        }

        if (_useSwerveSounds)
        {
            bool isMovingOrRotating = Math.Abs(Math.Round(velocity.x)) > 0f || Math.Abs(Math.Round(velocity.z)) > 0f ||
                                      Math.Abs(_rotateValue) > 0f;

            if (isMovingOrRotating && !_dontPlayDriveSounds)
            {
                PlaySwerveSounds();
            }
            else
            {
                StopSwerveSounds();
            }
        }
    }

    void FixedUpdate()
    {
        if (GameManager.canRobotMove)
        {
            if (isGrounded)
            {
                _dontPlayDriveSounds = false;

                Vector3 moveDirection;

                if (isFieldCentric)
                {
                    moveDirection = _startingDirection * _translateValue.y + _startingRotation * _translateValue.x;
                }
                else
                {
                    moveDirection = transform.forward * _translateValue.y + transform.right * _translateValue.x;
                }

                Vector3 rotation = new Vector3(0f, _rotateValue * rotationSpeed, 0f);

                _rb.AddForce(moveDirection * moveSpeed);

                if (isRedRobot && canRedRotate || !isRedRobot && canBlueRotate)
                {
                    _rb.AddTorque(rotation);
                    _rb.angularVelocity = Vector3.ClampMagnitude(_rb.angularVelocity, maxAngularVelocity);
                }

                velocity = _rb.velocity;
            }
            else
            {
                _dontPlayDriveSounds = true;
            }
        }
        else
        {
            if (_useSwerveSounds)
            {
                _dontPlayDriveSounds = true;
                StopSwerveSounds();
            }
        }
    }

    public void StopCountdown()
    {
        redCountDown.Stop();
        blueCountDown.Stop();
    }

    private void PlaySwerveSounds()
    {
        float velocityFactor = Mathf.Clamp01(velocity.magnitude / moveSpeed);
        float accelerationFactor = Mathf.Clamp(1f + (velocity.magnitude / moveSpeed), 1f, 2f);

        float rotationFactor = Mathf.Clamp01(Mathf.Abs(_rotateValue) / rotationSpeed);

        float volume = velocityFactor + (rotationFactor * 10f);

        float pitch = Mathf.Max(accelerationFactor, rotationFactor);

        treadPlayer.volume = volume * 0.8f;
        treadPlayer.pitch = pitch * 0.7f;
        gearPlayer.volume = volume * 0.5f;

        if (!treadPlayer.isPlaying && !gearPlayer.isPlaying)
        {
            gearPlayer.Play();
            treadPlayer.Play();
        }
    }

    private void StopSwerveSounds()
    {
        if (treadPlayer.isPlaying || gearPlayer.isPlaying)
        {
            treadPlayer.Stop();
            gearPlayer.Stop();
        }
    }

    private void AmplifySpeaker()
    {
        _ampTimerCoroutine = StartCoroutine(StartTimer());
    }

    public void StopAmplifiedSpeaker()
    {
        //Stop the countdown coroutine
        if (_ampTimerCoroutine != null)
        {
            StopCoroutine(_ampTimerCoroutine);
        }

        //Reset note worth after amplification ends
        allianceSpeaker.ResetNotes();

        if (isRedRobot)
        {
            redCountDown.Stop();
        }
        else
        {
            blueCountDown.Stop();
        }

        //Reset isAmped flag to false
        if (!isRedRobot)
        {
            isAmped = false;
        }
        else
        {
            isRedAmped = false;
        }
    }

    public IEnumerator GrayOutBumpers(float duration)
    {
        if (is930)
        {
            Material[] mat = bumper.GetComponent<Renderer>().materials;
            mat[3].color = Color.gray;
            mat[4].color = Color.gray;
        }
        else
        {
            _bumperMat.color = Color.gray;
        }

        yield return new WaitForSeconds(duration);
        if (is930)
        {
            Material[] mat = bumper.GetComponent<Renderer>().materials;
            mat[3].color = _defaultBumperColor;
            mat[4].color = _defaultBumperColor;
        }
        else
        {
            _bumperMat.color = _defaultBumperColor;
        }
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(10f);
        StopAmplifiedSpeaker();
    }

    public void Reset()
    {
        StopAllCoroutines();

        //Reset bumper colors
        if (is930)
        {
            Material[] mat = bumper.GetComponent<Renderer>().materials;
            mat[3].color = _defaultBumperColor;
            mat[4].color = _defaultBumperColor;
        }
        else
        {
            _bumperMat.color = _defaultBumperColor;
        }

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        //Reset position
        _rb.MovePosition(_startingPos);
        _rb.MoveRotation(_startingRot);
    }

    public void OnTranslate(InputAction.CallbackContext ctx)
    {
        _translateValue = ctx.ReadValue<Vector2>();
    }

    public void OnRotate(InputAction.CallbackContext ctx)
    {
        _rotateValue = ctx.ReadValue<float>();
    }

    public void OnIntake(InputAction.CallbackContext ctx)
    {
        intakeValue = ctx.ReadValue<float>();
    }

    public void OnAmpSpeaker(InputAction.CallbackContext ctx)
    {
        _ampSpeaker = ctx.action.triggered;
    }

    public void OnRestart(InputAction.CallbackContext ctx)
    {
        _gameManager.ResetMatch();
    }

    public bool CheckGround()
    {
        float distanceToTheGround = rayCastDistance;
        foreach (Transform rayCastPoint in rayCastPoints)
        {
            if (!flipRayCastDir)
            {
                if (Physics.Raycast(rayCastPoint.position, -transform.up, distanceToTheGround))
                {
                    return true;
                }
            }
            else
            {
                if (Physics.Raycast(rayCastPoint.position, transform.up, distanceToTheGround))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool CheckTouchingGround()
    {
        if (field.bounds.Intersects(gameObject.GetComponent<Collider>().bounds))
        {
            return true;
        }

        return false;
    }

    public IEnumerator DriveTo(Transform target)
    {
        atTargetPos = false;
        while (target != null && Vector2.Distance(transform.position, target.position) > 0.1f)
        {
            if (!GameManager.canRobotMove)
            {
                atTargetPos = true;
                break;
            }

            Vector3 targetDirection = target.position - transform.position;
            targetDirection.y = 0f;

            targetDirection.Normalize();

            Vector3 force = targetDirection * moveSpeed * Time.deltaTime;

            _rb.AddForce(force, ForceMode.VelocityChange);

            velocity = _rb.velocity;

            PlaySwerveSounds();
            yield return null;
        }

        atTargetPos = true;
    }

    public IEnumerator RotateTowardsTarget(Transform target)
    {
        atTargetRot = false;
        while (target != null)
        {
            if (!GameManager.canRobotMove || isRedRobot && !canRedRotate || !isRedRobot && !canBlueRotate)
            {
                atTargetRot = true;
                break;
            }

            Vector3 targetDirection = target.position - transform.position;
            targetDirection.y = 0f;

            if (targetDirection == new Vector3(0, 0, 0))
            {
                atTargetRot = true;
                break;
            }

            Quaternion targetRotation = Quaternion.LookRotation(-targetDirection, Vector3.up);

            Quaternion newRotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            _rb.MoveRotation(newRotation);

            if (Quaternion.Angle(transform.rotation, targetRotation) == 0)
            {
                break;
            }

            yield return null;
        }

        atTargetRot = true;
    }

    public bool InStowZone()
    {
        return gameObject.GetComponent<Collider>().bounds.Intersects(blueStage.bounds) ||
               gameObject.GetComponent<Collider>().bounds.Intersects(redStage.bounds);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isRedRobot)
        {
            if (other.gameObject.CompareTag("RedPlayer"))
            {
                robotsTouching = true;
            }
            else if (other.gameObject.CompareTag("Field") || other.gameObject.CompareTag("Wall"))
            {
                _dontUpdateBeforeVelocity = true;
                isTouchingWallColliderBlue = true;
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                robotsTouching = true;
            }
            else if (other.gameObject.CompareTag("Field") || other.gameObject.CompareTag("Wall"))
            {
                _dontUpdateBeforeVelocity = true;
                isTouchingWallColliderRed = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isRedRobot)
        {
            if (other.gameObject.CompareTag("RedPlayer"))
            {
                robotsTouching = false;
            }
            else if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Field"))
            {
                _dontUpdateBeforeVelocity = false;
                if (!isRedRobot)
                {
                    isTouchingWallColliderBlue = false;
                }
                else
                {
                    isTouchingWallColliderRed = false;
                }
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                robotsTouching = false;
            }
            else if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Field"))
            {
                _dontUpdateBeforeVelocity = false;
                if (!isRedRobot)
                {
                    isTouchingWallColliderBlue = false;
                }
                else
                {
                    isTouchingWallColliderRed = false;
                }
            }
        }
    }

    public void OnSourceDrop(InputAction.CallbackContext ctx)
    {
        _sourceRingLoader.OnSourceDrop(ctx);
    }
}