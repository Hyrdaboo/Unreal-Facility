using UnityEngine;

public class FPScontroller : MonoBehaviour
{
    public Camera PlayerCamera;
    private Rigidbody rb;
    public float smoothCamSpeed = 10;
    public float speed = 100;
    public float walkSoundRate = .5f;
    public float GroundFriction;
    public float AirFriction;
    public float JumpHeight = 3;
    public float DashForce = 5;
    public float minDashKillDistance = 10;
    public bool allowDash = true;
    public GameObject groundCheck;
    public LayerMask WhatIsGround;
    public float dashCooldown = 10;
    private SoundManager soundManager;
    private float savedDashCoolDown;
    private float nextTimeToWalk;
    float xInputDir;
    float zInputDir;

    bool isGrounded;
    float savedSpeed;

    Vector3 PlayerRotation;
    private void Start()
    {
        TimeCounter.Start();
        Cursor.lockState = CursorLockMode.Locked;
        PlayerRotation = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
        rb = GetComponent<Rigidbody>();
        soundManager = FindObjectOfType<SoundManager>();
        if (soundManager == null)
        {
            Debug.LogWarning("Sound Manager does not exist but you are trying to access it");
        }
        PlayerCamera.transform.eulerAngles = Vector3.zero;
        savedSpeed = speed;
        savedDashCoolDown = dashCooldown;
        dashCooldown = 1;
    }

    private void Update()
    {
        xInputDir = Input.GetAxis("Horizontal");
        zInputDir = Input.GetAxis("Vertical");
        MoveCam();

        isGrounded = Physics.CheckSphere(groundCheck.transform.position, 0.4f, WhatIsGround);
        if ((xInputDir != 0 || zInputDir != 0) && isGrounded)
        {
            PlayWalkSound();
        }
        if (xInputDir == 0 && zInputDir == 0)
        {
            PauseWalkSound();
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && allowDash)
        {
            Dash();
        }
        if ((int)dashCooldown > 0) dashCooldown -= Time.deltaTime;

        HandleWallHits();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        AddCounterForce(GroundFriction);
        if (!isGrounded)
        {
            AddCounterForce(AirFriction);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 3)
        {
            soundManager.GetSoundByName("Walk3").PlaySound();
        }
    }

    Vector3 camRotation = new Vector3(0, 0, 0);
    public void MoveCam()
    {
        float MouseX = Input.GetAxis("Mouse X");
        float MouseY = Input.GetAxis("Mouse Y");

        camRotation.x -= MouseY * smoothCamSpeed * foo.camSpeed;

        camRotation.x = Mathf.Clamp(camRotation.x, -90, 90);
        Vector3 newCamRot = new Vector3(camRotation.x, 0, 0);
        PlayerCamera.transform.localRotation = Quaternion.Slerp(PlayerCamera.transform.localRotation, Quaternion.Euler(newCamRot.x, newCamRot.y, newCamRot.z), Time.deltaTime * smoothCamSpeed);

        PlayerRotation.y += MouseX * smoothCamSpeed * foo.camSpeed;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(PlayerRotation), Time.deltaTime * smoothCamSpeed);
    }

    public void Dash ()
    {
        GameObject enemyObject = null;
        Enemy enemy = null;
        
        Ray ray = new Ray(transform.position, transform.forward*10);
        RaycastHit hit;
        
        if (isGrounded && (int)dashCooldown == 0)
        {
            if (Physics.Raycast(ray, out hit, minDashKillDistance))
            {
                Debug.Log(hit.transform.tag);
                if (hit.transform.tag == "Enemy")
                {
                    enemyObject = hit.transform.root.gameObject;
                    enemy = enemyObject.GetComponent<Enemy>();
                    enemy.hitsToDie = 0;
                    enemy.OnDeath();
                }
            }

            rb.AddForce(DashForce * transform.forward, ForceMode.Impulse);
            dashCooldown = savedDashCoolDown;
        }
    }

    public void MovePlayer()
    {
        Vector3 moveDir = (transform.forward * zInputDir + transform.right * xInputDir) * speed * Time.fixedDeltaTime;
        rb.AddForce(moveDir);
    }

    string[] walkSounds = { "Walk1", "Walk2", "Walk3"};
    public void PlayWalkSound ()
    {
        if (Time.time > nextTimeToWalk)
        {
            nextTimeToWalk = Time.time + walkSoundRate;
            soundManager.GetSoundByName(walkSounds[(int)Random.Range(0, walkSounds.Length)]).PlaySound();
        }
    }
    public void PauseWalkSound ()
    {
        soundManager.GetSoundByName(walkSounds[(int)Random.Range(0, walkSounds.Length)]).PauseSound();
    }

    public void AddCounterForce(float mu)
    {
        Vector3 rbVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        Vector3 counterForce = -rbVelocity * rb.mass * 10 * mu;
        rb.AddForce(counterForce, ForceMode.Force);
    }

    public void Jump()
    {
        soundManager.GetSoundByName("Jump").PlaySound();
        float jumpForce = (rb.mass * 100) / 2 * JumpHeight;
        rb.AddForce(new Vector3(0, jumpForce, 0));
    }

    public void HandleWallHits()
    {
        if (!isGrounded && rb.velocity.magnitude < .05f)
        {
            speed = 0;
        }
        else if (isGrounded)
        {
            speed = savedSpeed;
        }
    }
}
