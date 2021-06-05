using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class PlayerMovement : MonoBehaviour
{
    public enum PlayerStates
    {
        Grounded,
        InAir,
        OnWalls,
        LedgeGrab,
    }

    public enum ParkourMode
    {
        Run,
        Jog,
        Weapon,
    }

    private PlayerCollision Coli;
    private Rigidbody Rigid;
    private CapsuleCollider Cap;
    private Animator Anim;

    [Header("Physics")]
    public float MaxSpeed;
    public float JogSpeed;
    public float WeaponMoveSpeed;
    private float PreviousMaxSpeed;
    public float BackwardsSpeed;
    public float InAirControl;

    private float ActSpeed;
    public float Acceleration;
    public float Decceleration;
    public float DirectionControl = 8;
    public PlayerStates CurrentState;
    public ParkourMode Mode;
    public float InAirTimer;
    private float CurrentInAirTimer = 0;
    private float OnGroundTimer;
    private float AdjustmentAmt;

    [HideInInspector]
    public Vector3 Vel = Vector3.zero;


    [Header("Turning")]
    public float TurnSpeed;
    public float TurnSpeedInAir;
    public float TurnSpeedOnWalls;
    public float LookUpSpeed;
    public Camera Head;
    private float YTurn;
    private float XTurn;
    public float MaxLookAngle = 65;
    public float MinLookAngle = -30;

    [Header("Jumping")]
    public float JumpHeight;

    [Header("Wall Runs")]
    public float WallRunTime = 2f;
    private float ActWallRunTime = 0; 
    public float TimeBeforeWallRun = 0.2f;
    public float WallRunUpwardsMovement = 2f;
    public float WallRunSpeedAcceleration = 2f;
    public float WallRunJump = 2f;

    [Header("Crouching")]
    public float CrouchSpeed = 10;
    public float CrouchHeight = 1.5f;
    private float StandingHeight = 2f;
    private bool Crouch;

    [Header("Sliding")]
    public float SlideAmt;
    public float SlideSpeedLimit;
    public float SlideControl;

    [Header("WallGrabbing")]
    public float PullUpTime;
    private float ActPullTm;
    private Vector3 OrigPos;
    private Vector3 LedgePos;

    [Header("FOV")]
    public float MaxFov;
    private float CurrentMaxFov;
    private float MinFov;
    public float FOVSpeed;

    [Header("Audio")]
    public AudioSource InAir;
    public AudioSource JumpLand;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Coli = GetComponent<PlayerCollision>();      
        Rigid = GetComponent<Rigidbody>();
        Anim = GetComponentInChildren<Animator>();
        MinFov = Head.fieldOfView;
        CurrentMaxFov = MaxFov;
        Cap = GetComponent<CapsuleCollider>();
        StandingHeight = Cap.height;

        AdjustmentAmt = 1;
        PreviousMaxSpeed = MaxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void ScreenShakeFeedback()
    {
        if (InAirTimer > 1) CameraShaker.Instance.ShakeOnce(3f, 0.2f, 0.2f, 0.4f);
    }

    void AnimCtrl()
    {
        int State = 0;
        if (CurrentState == PlayerStates.InAir)
            State = 1;
        else if (CurrentState == PlayerStates.OnWalls)
            State = 2;
        else if (CurrentState == PlayerStates.LedgeGrab)
            State = 3;

        Anim.SetInteger("State", State);
        Anim.SetBool("Crouching", Crouch);

        Vel = transform.InverseTransformDirection(Rigid.velocity);
        Anim.SetFloat("XVelocity", Vel.x);
        Anim.SetFloat("ZVelocity", Vel.z);
        Anim.SetFloat("YVelocity", Rigid.velocity.y);
        //Anim.SetFloat("XInput", Input.GetAxis("Horizontal"));
    }

    private void FixedUpdate()
    {
        
        float XMove = Input.GetAxis("Horizontal");
        float YMove = Input.GetAxis("Vertical");

        if (Mode == ParkourMode.Run)
        {
            MaxSpeed = PreviousMaxSpeed;
        } 
        else if(Mode == ParkourMode.Jog)
        {
            if (Input.GetKey(KeyCode.LeftShift)) MaxSpeed = PreviousMaxSpeed;
            else MaxSpeed = JogSpeed;
        } 
        else if (Mode == ParkourMode.Weapon)
        {
            MaxSpeed = WeaponMoveSpeed;
        }

        if (CurrentState == PlayerStates.Grounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                JumpUp();
            }

            
        }
        
        else if (CurrentState == PlayerStates.InAir)
        {
            if (Input.GetButton("Grab"))
            {
                Vector3 LedgePos = Coli.CheckLedges();
                if (LedgePos != Vector3.zero)
                {
                    LedgeGrab(LedgePos);
                }
            }

            bool Wall = CheckWalls(XMove, YMove);

            if (Wall)
            {
                if (InAirTimer > TimeBeforeWallRun)
                {
                    SetOnWall();
                    return;
                }
            }

            bool Grounded = Coli.CheckFloor(-transform.up);

            if (Grounded && InAirTimer > 0.25f)
            {
                SetOnGround();
            }
        }
        else if (CurrentState == PlayerStates.OnWalls)
        {
            if (Input.GetButton("Grab"))
            {
                Vector3 LedgePos = Coli.CheckLedges();

                if (LedgePos != Vector3.zero)
                {
                    LedgeGrab(LedgePos);
                }
            }

            bool Wall = CheckWalls(XMove, YMove);

            if (!Wall)
            {
                SetInAir();
                MaxFov = CurrentMaxFov;

                return;
            }

            bool Grounded = Coli.CheckFloor(-transform.up);

            if (Grounded)
            {
                SetOnGround();
            }
        }
        else if(CurrentState == PlayerStates.LedgeGrab)
        {
            Rigid.velocity = Vector3.zero;
        }

        AnimCtrl();
        ScreenShakeFeedback();

        float Del = Time.deltaTime;

        float CamX = Input.GetAxis("Mouse X");
        float CamY = Input.GetAxis("Mouse Y");

        LookUpDown(CamY, Del);

        HandleFov(Del);

        float horInput = Input.GetAxis("Horizontal");
        float verInput = Input.GetAxis("Vertical");

        if (CurrentState == PlayerStates.Grounded)
        {
            if (OnGroundTimer < 10)
                OnGroundTimer += Del;
           

            float InputMagnitude = new Vector2(horInput, verInput).normalized.magnitude;
            float TargetSpd = Mathf.Lerp(BackwardsSpeed, MaxSpeed, verInput);

            if (Crouch)
                TargetSpd = CrouchSpeed;

            LerpSpeed(InputMagnitude, Del, TargetSpd);

            MovePlayer(horInput, verInput, Del);
            TurnPlayer(CamX, Del, TurnSpeed);

            if(Input.GetButton("Crouch"))
            { 
                if(!Crouch)
                {
                    StartCrouch();
                }
            }
            else
            {
                bool check = Coli.CheckRoof(transform.up);
                if (!check)
                {
                    StopCrouching();
                }
            }

            if (AdjustmentAmt < 1)
                AdjustmentAmt += Del * SlideControl;
            else
                AdjustmentAmt = 1;
            
            bool Grounded = Coli.CheckFloor(-transform.up);

            if (!Grounded)
            {
                if (InAirTimer < 0.2f)
                    InAirTimer += Del;
                else
                {
                    SetInAir();
                    return;
                }
            }
            else
            {
                InAirTimer = 0;
            }
        }
        else if(CurrentState == PlayerStates.InAir)
        {
            if (InAirTimer < 10)
                InAirTimer += Del;

            MoveInAir(horInput, verInput ,Del);

            TurnPlayer(CamX, Del, TurnSpeedInAir);
        }
        else if (CurrentState == PlayerStates.OnWalls)
        {
            if (!Input.GetButtonDown("Jump"))
            {
                ActWallRunTime += Del;
                Debug.Log(ActWallRunTime);
                TurnPlayer(CamX, Del, TurnSpeedOnWalls);

                WallMove(verInput, Del);
                MaxFov = 51;
            }
            else
            {
                SetInAir();
                MaxFov = CurrentMaxFov;
                Rigid.AddForce(Coli.GetWallNormal() * WallRunJump, ForceMode.Impulse);
            }
        }
            
        else if(CurrentState == PlayerStates.LedgeGrab)
        {
            ActPullTm += Del;

            float PullUpLerp = ActPullTm / PullUpTime;

            if (PullUpLerp < 0.5)
            {              
                float LAmt = PullUpLerp * 2;
                transform.position = Vector3.Lerp(OrigPos, new Vector3(OrigPos.x, LedgePos.y, OrigPos.z), LAmt);               
            }
            else if(PullUpLerp <= 1)
            {
                Cap.enabled = false;
                if (OrigPos.y != LedgePos.y)
                    OrigPos = new Vector3(transform.position.x, LedgePos.y, transform.position.z);

                float LAmt = (PullUpLerp - 0.5f) * 2;
                transform.position = Vector3.Lerp(OrigPos, LedgePos, PullUpLerp);
            }
            else
            {
                Cap.enabled = true;
                SetOnGround();
            }
        }
    }


    void LerpSpeed(float InputMag, float D, float TargetSpeed)
    {
        float LerpAmt = TargetSpeed * InputMag;
        float Accel = Acceleration;

        if (InputMag == 0)
            Accel = Decceleration;
        
        ActSpeed = Mathf.Lerp(ActSpeed, LerpAmt, D * Accel);
    }

    void SetSpeedToVelocity()
    {
        float Mag = new Vector2(Rigid.velocity.x, Rigid.velocity.z).magnitude;
        ActSpeed = Mag;
    }

    bool CheckWalls(float X, float Y)
    {
        if (X == 0 && Y == 0)
            return false;

        if (ActWallRunTime >= WallRunTime)
            return false;

        
        float ClampedY = Mathf.Clamp(Y, 0, 1);
        Vector3 Dir = transform.forward * ClampedY + transform.right * X;

        bool WallCol = Coli.CheckWall(Dir);

        return WallCol;
    }

    void SetInAir()
    {
        StopCrouching();

        OnGroundTimer = 0;
        CurrentState = PlayerStates.InAir;

        if (!InAir.isPlaying) InAir.Play();
    }

    void SetOnGround()
    {
        SetSpeedToVelocity();

        ActWallRunTime = 0;
        InAirTimer = 0; 
        CurrentState = PlayerStates.Grounded;

        InAir.Stop();

        JumpLand.pitch = Random.Range(1.1f, 1.35f);
        JumpLand.Play();
    }

    void SetOnWall()
    {
        OnGroundTimer = 0;
        InAirTimer = 0;
        CurrentState = PlayerStates.OnWalls;
    }

    void LedgeGrab(Vector3 Ledge)
    {
        LedgePos = Ledge;
        OrigPos = transform.position;

        //reset ledge grab time
        ActPullTm = 0;

        //remove speed and velocity
        Rigid.velocity = Vector3.zero;

        ActSpeed = 0;
        //start ledge grabs
        CurrentState = PlayerStates.LedgeGrab;
    }

    void StartCrouch()
    {
        Crouch = true;
        Cap.height = CrouchHeight;

        if (ActSpeed > SlideSpeedLimit)
            SlideSelf();
    }

    void StopCrouching()
    {
        Crouch = false;
        Cap.height = StandingHeight;
    }

    void TurnPlayer(float Hor, float D, float turn)
    {
        YTurn += (Hor * D) * turn; 
        transform.rotation = Quaternion.Euler(0, YTurn, 0);
    }

    void LookUpDown(float Ver, float D)
    {
        XTurn -= (Ver * D) * LookUpSpeed;
        XTurn = Mathf.Clamp(XTurn, MinLookAngle, MaxLookAngle);

        Head.transform.localRotation = Quaternion.Euler(XTurn, 0, 0);
    }

    void MovePlayer(float Hor, float Ver, float D)
    {
        Vector3 MovementDirection = (transform.forward * Ver) + (transform.right * Hor);
        MovementDirection = MovementDirection.normalized;

        if (Hor == 0 && Ver == 0)
            MovementDirection = Rigid.velocity.normalized;

        MovementDirection = MovementDirection * ActSpeed;

        MovementDirection.y = Rigid.velocity.y;

        float Acel = DirectionControl * AdjustmentAmt;
        Vector3 LerpVelocity = Vector3.Lerp(Rigid.velocity, MovementDirection, Acel * D);
        Rigid.velocity = LerpVelocity;          
    }

    void MoveInAir(float Hor, float Ver, float D)
    {
        Vector3 MovementDirection = (transform.forward * Ver) + (transform.right * Hor);
        MovementDirection = MovementDirection.normalized;

        if (Hor == 0 && Ver == 0)
            MovementDirection = Rigid.velocity.normalized;

        MovementDirection = MovementDirection * ActSpeed;

        MovementDirection.y = Rigid.velocity.y;

        Vector3 LerpVelocity = Vector3.Lerp(Rigid.velocity, MovementDirection, InAirControl * D);
        Rigid.velocity = LerpVelocity;

    }

    void WallMove(float Ver, float D)
    {
        Vector3 MovementDirection = transform.up * Ver;
        MovementDirection = MovementDirection * WallRunUpwardsMovement;

        MovementDirection += transform.forward * ActSpeed;

        Vector3 LerpVelocity = Vector3.Lerp(Rigid.velocity, MovementDirection, WallRunSpeedAcceleration * D);
        Rigid.velocity = LerpVelocity;
    }

    void JumpUp()
    {
        if (CurrentState == PlayerStates.Grounded)
        {
            Vector3 VelAmt = Rigid.velocity;
            VelAmt.y = 0;
            Rigid.velocity = VelAmt;

            Rigid.AddForce(transform.up * JumpHeight, ForceMode.Impulse);

            SetInAir();
        }
    }

    void HandleFov(float D)
    {
        //get our velocity magniture
        float mag = new Vector2(Rigid.velocity.x, Rigid.velocity.z).magnitude;
        //get appropritate fov 
        float LerpAmt = mag / FOVSpeed;
        float FieldView = Mathf.Lerp(MinFov, MaxFov, LerpAmt);
        //ease into this fov
        Head.fieldOfView = Mathf.Lerp(Head.fieldOfView, FieldView, 4 * D);
    }

    void SlideSelf()
    {
        ActSpeed = SlideSpeedLimit;

        AdjustmentAmt = 0;

        Vector3 Dir = Rigid.velocity.normalized;
        Dir.y = 0;

        Rigid.AddForce(transform.forward * SlideAmt, ForceMode.Impulse);
    }
}
