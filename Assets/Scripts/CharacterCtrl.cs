
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CharacterCtrl : MonoBehaviour
{
    public static CharacterCtrl _CharacterCtrl;
    public enum OutLookState
    {
        NORMAL,
        DIAMOND,

    }
    public enum ActionState
    {
        IDLE,
        NORMALSPEED,
        HIGHSPEED,
        BREAKING,
        AIMING
    }
    [Range(0, 100)] public float PlayerHealth = 100f;
    public OutLookState currentOutLookState = OutLookState.NORMAL;
    public ActionState playerActionState = ActionState.IDLE;
    public LayerMask groundLayer;
    public Transform Camera;
    public Transform PlayerKernel;
    public Vector3 CheckPoint;
    public bool towardWithCamera = true, moveAbility = true, climbAbility = true, shootAbility = true, catchObjAbility = true, jumpAbility = true, flyAbility = true;
    public float initial_torque, speedUp_torque, jumpForce, rushForce, sliteForce = 5f;
    public Material TramsparentMaterial;
    public Animator MaskAnimator, PlayerAnimator;
    public List<GameObject> playerSkinList = new();
    public ParticleSystem SwitchParticleSystem;
    public GunScript gunScript;
    public GameObject Player_Camera1, Player_Camera2;// cam1 is CinemachineFreeLook with Orbits, cam2 is CinemachineVirtualCamera with CinemachineTransposer
    public ParticleSystem landBendEffect;


    //public List<GameObject> HitObjects = new();
    public Queue<GameObject> HitObjectsQueue = new();

    [Header("For Status Debuging")]

    public bool isDoubleClick, isCliming, ableToJump = false;
    [SerializeField] MeshRenderer playerMeshRenderer;
    // [SerializeField] List<GameObject> TransparentChangeList = new();
    //[SerializeField] List<Material> OriginalMaterialList = new();

    Rigidbody rb; // player
    float horizontalInput, verticalInput;
    private void Awake()
    {
        Physics.IgnoreLayerCollision(GlobalRules.instance.playerLayerID, GlobalRules.instance.playerLayerID);
        foreach (GameObject temp in GameObject.FindGameObjectsWithTag("Respawn"))
        {
            if (temp != this.transform.parent.parent.gameObject) { Destroy(temp); }

        }

        //if (GameObject.FindGameObjectsWithTag("Respawn"))
        //{

        //    Destroy(this.transform.parent.parent.gameObject);
        //}
        //  Debug.Log("Awake");
        _CharacterCtrl = this;
        //DontDestroyOnLoad(this.transform.parent.parent);

    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (MaskAnimator != null) { MaskAnimator.Play("Enter"); }
    }
    void Start()
    {
        CheckPoint = new Vector3(PlayerPrefs.GetFloat("SavedCheckPoint_X"), PlayerPrefs.GetFloat("SavedCheckPoint_Y"), PlayerPrefs.GetFloat("SavedCheckPoint_Z"));
        PlayerPrefs.SetString("SavedCheckPointScene", SceneManager.GetActiveScene().name);//save player's current scene
        if (CheckPoint == Vector3.zero) { CheckPoint = this.transform.position; }
        else { this.transform.position = CheckPoint; }
        rb = GetComponent<Rigidbody>();
        //GlobalRules.instance.cam1 = Player_Camera1.GetComponent<Cinemachine.CinemachineFreeLook>();
        //GlobalRules.instance.cam2 = Player_Camera2.GetComponent<Cinemachine.CinemachineVirtualCamera>();
        PlayerKernel.parent = this.transform.parent.parent;
        gunScript = PlayerKernel.GetComponent<GunScript>();
        //gunScript.PlayerKernelTarget = this.gameObject.transform;

        StartCoroutine(AutoDestory());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ONBelowDeathAltitude();
        if (moveAbility) { TurningTorque(); }


        GiveForce();//swimming

    }
    void Update()
    {
        if (!moveAbility) { return; }
        PlayerHealth = Mathf.Clamp(PlayerHealth, 0, 100);
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        JumpCommand(); RushCommand();
        DestroyCommand();
        ChangeCamera();
        Break_Aim();
    }
    //public void OnGravityCubeHitted(GameObject other, Material hittedObjectMaterial)
    //{
    //    if (other.CompareTag("GravityCube"))
    //    {
    //        other.GetComponent<MeshRenderer>().material = hittedObjectMaterial;
    //        other.GetComponent<Light>().enabled = true;
    //        other.GetComponent<Rigidbody>().useGravity = false;
    //        if (HitObjects.Contains(other)) { return; }
    //        HitObjects.Add(other);

    //    }
    //}
    void TurningTorque()
    {
        rb.maxAngularVelocity = (Input.GetKey(GlobalRules.instance.SpeedUp) ? speedUp_torque : initial_torque);
        if (towardWithCamera)
        {

            rb.AddTorque(rb.maxAngularVelocity * verticalInput * Camera.transform.right);       //foward and back, rotate around Camera's red axis
            rb.AddTorque(rb.maxAngularVelocity * horizontalInput * -Camera.transform.forward);  //left and right,rotate around Camera's blue axis
        }
        else
        {
            rb.AddTorque(rb.maxAngularVelocity * verticalInput * Vector3.right);
            rb.AddTorque(rb.maxAngularVelocity * horizontalInput * -Vector3.forward);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (landBendEffect) landBendEffect.Emit(1);

        if (other.collider.name == "Diamond" && currentOutLookState != OutLookState.DIAMOND)
        {
            PlayerAnimator.SetBool("ToDiamond", true);
            PlayerAnimator.SetBool("ToNormal", false);
            currentOutLookState = OutLookState.DIAMOND;
        }
        if (other.collider.name == "Normal" && currentOutLookState != OutLookState.NORMAL)
        {
            PlayerAnimator.SetBool("ToNormal", true);
            PlayerAnimator.SetBool("ToDiamond", false);
            currentOutLookState = OutLookState.NORMAL;
            //if layer is ground

        }
        if (other.collider.name == "FinnishPoint" || other.collider.name == "CheckPoint")
        {
            other.gameObject.GetComponent<CheckPoint>().enabled = true;
        }
    }

    public void ResetAnimateParamater() { PlayerAnimator.SetBool("ToDiamond", false); PlayerAnimator.SetBool("ToNormal", false); }
    private void OnCollisionExit(Collision other)
    {
        isCliming = false;
        ableToJump = false;
        //if layer is ground
        //if (GlobalRules.IsGameObjInLayerMask(other.gameObject, GlobalRules.instance.GoundLayer))
        //{
        //    ableToJump = false;
        //}
    }


    private void OnCollisionStay(Collision collision)
    {
        ClimbWall(collision);
        // Debug.Log((transform.position - collision.GetContact(0).point).normalized);

        //if (GlobalRules.IsGameObjInLayerMask(collision.gameObject, GlobalRules.instance.GoundLayer))
        //{
        //    ableToJump = true;
        //}
        //var temp = (transform.position - collision.GetContact(0).point);
        // Debug.Log(temp.normalized + ", " + temp.normalized.y);
        if ((transform.position - collision.GetContact(0).point).normalized.y >= 0.2) { ableToJump = true; }
    }
    /*
    float doubleClickTime = 1, lastClickTime;

    private bool IsDoubleClick(KeyCode keyCode)
    {
        if (Input.GetKeyDown(keyCode))
        {
            isDoubleClick = ((Time.time - lastClickTime) <= doubleClickTime);
            lastClickTime = Time.time;
        }
        else { isDoubleClick = false; }

        return isDoubleClick;
    }*/
    private void ClimbWall(Collision collision)
    {
        //if (collision.gameObject.layer == GlobalRules.instance.groundLayerID) { isCliming = false; return; }
        if (!climbAbility) { return; }
        if (Input.GetKey(GlobalRules.instance.Climb) && PlayerBrain.shootEnergy > 0)
        {
            if (Input.GetKeyDown(GlobalRules.instance.Jump))
            {
                rb.AddForce((transform.position - collision.GetContact(0).point).normalized * jumpForce);
                isCliming = false;
                return;
            }
            isCliming = true;
            PlayerBrain.shootEnergy -= Time.deltaTime * GlobalRules.instance.holdConsume;
            rb.AddForce(2 * -Physics.gravity.y * (collision.GetContact(0).point - transform.position).normalized);
            rb.AddForce(-Physics.gravity);// print("First point that collided: " + collision.contacts[0].point);
        }
        else { isCliming = false; }

    }
    void GiveForce()
    {
        if (PlayerBrain.shootEnergy <= 0) { return; }
        //  Debug.Log("GiveForce");
        var force = (Input.GetKey(GlobalRules.instance.SpeedUp) ? sliteForce * 2 : sliteForce);
        rb.AddForce(force * verticalInput * Camera.transform.forward);
        rb.AddForce(force * horizontalInput * Camera.transform.right);
        if (Input.GetKey(GlobalRules.instance.MoveUp))
        {
            rb.AddForce(Vector3.up);
        }
        if (Input.GetKey(GlobalRules.instance.MoveDown))
        {
            rb.AddForce(-Vector3.up);
        }

        //PlayerBrain.shootEnergy -= Time.deltaTime * GlobalRules.instance.holdConsume;
    }
    void JumpCommand()
    {
        if (!jumpAbility) { return; }
        if (Input.GetKeyDown(GlobalRules.instance.Jump))
        {
            if (ableToJump)
            {
                Debug.Log("Jump");
                rb.AddForce(Vector3.up * jumpForce);
            }

        }
        else if (!isCliming && !ableToJump && PlayerBrain.shootEnergy > 0)
        {
            if (!flyAbility) { return; }
            if ((Input.GetKey(GlobalRules.instance.Jump)))
            {

                rb.useGravity = false;
                PlayerBrain.shootEnergy -= Time.deltaTime * GlobalRules.instance.flyCosume;

                // Debug.Log("Fly");
            }
            else { rb.useGravity = true; }
        }
        else { rb.useGravity = true; }

    }
    void RushCommand()
    {
        if (Input.GetKeyDown(GlobalRules.instance.Rush))
        {
            if (towardWithCamera)
            { rb.AddForce(Camera.transform.forward * rushForce); }
            else
            {
                rb.AddForce(Vector3.zero * rushForce);
            }
        }
    }
    void DestroyCommand()
    {
        if (Input.GetKey(KeyCode.C))
        {
            MenualCheckDestory();
        }
    }

    void MenualCheckDestory()
    {
        if (HitObjectsQueue.Count > 0) { Destroy(HitObjectsQueue.Dequeue()); }
        //foreach (var item in HitObjects)
        //{
        //    Destroy(item);

        //}
        //HitObjects.Clear();
        // Destroy(Test.Dequeue());
    }
    void Break_Aim()
    {
        if (Input.GetMouseButton(1)) { rb.angularVelocity = Vector3.zero; }
        if (!shootAbility) { return; }
        if (Input.GetKey(GlobalRules.instance.HoldObject))
        {
            // Debug.Log("GetKeyDown");
            playerActionState = ActionState.AIMING;
            if (PlayerBrain.is_Charging) { PlayerAnimator.SetBool("isAiming", true); } else { PlayerAnimator.SetBool("isAiming", false); }

            //ChangeMaterialsToTransparent();
        }
        if (Input.GetKeyUp(GlobalRules.instance.PreShoot) || Input.GetKeyUp(GlobalRules.instance.HoldObject))
        {
            playerActionState = ActionState.IDLE;
            PlayerAnimator.SetBool("isAiming", false);
            //ChangeMaterialsToOriginal();
        }



    }

    public void ChangeMaterialsToTransparent()
    {
        playerActionState = ActionState.AIMING;

        // Material[] newMaterials = new Material[] { TramsparentMaterial };

        if (PlayerBrain.is_Charging)
        {
            playerMeshRenderer = GetComponent<MeshRenderer>();

            playerMeshRenderer.materials = new Material[] { TramsparentMaterial };

            //for (int i = 0; i < playerMeshRenderer.materials.Length; i++)
            //{
            //    OriginalMaterialList.Add(playerMeshRenderer.materials[i]);
            //    playerMeshRenderer.materials[i] = new Material[] { TramsparentMaterial };
            //}
        }
    }
    //public void ChangeMaterialsToOriginal()
    //{
    //    playerActionState = ActionState.IDLE;
    //    playerMeshRenderer = GetComponent<MeshRenderer>();
    //    for (int i = 0; i < OriginalMaterialList.Count - 1; i++)
    //    {
    //        playerMeshRenderer.materials[i] = OriginalMaterialList[i];
    //    }
    //}
    void ONBelowDeathAltitude()
    {
        if (transform.position.y < GlobalRules.instance.DeathAltitude)
        {
            MaskAnimator.Play("Enter", 0, 0);
            Debug.LogWarning("Below Death Altitude");
            this.transform.position = CheckPoint;
            rb.velocity = Vector3.zero;

            // LoadScene(SceneManager.GetActiveScene().buildIndex);
            // this.transform.position = new Vector3(0, 5, 0);
        }
    }
    public void LoadScene(int sceneIndex)
    {
        //Time.timeScale = 1f;
        //Time.fixedDeltaTime = Time.timeScale * 0.02f;
        SceneManager.LoadScene(sceneIndex);
    }

    private void ChangeCamera()//cam1 is CinemachineFreeLook, cam2 is CinemachineTransposer
    {
        if (Input.GetKeyDown(GlobalRules.instance.SwitchCamera))
        {
            if (Player_Camera1.activeSelf == true)//cam1 to cam2
            {
                Player_Camera2.SetActive(true);
                //  Camera = Player_Camera2.transform.parent.Find("Main Camera").GetComponent<Transform>();
                GlobalRules.instance.FitCameraDirection(true);
                Player_Camera1.SetActive(false);

            }
            else//cam2 to cam1
            {
                Player_Camera1.SetActive(true);
                //Camera = Player_Camera2.transform.parent.Find("Main Camera").GetComponent<Transform>();
                GlobalRules.instance.FitCameraDirection(false);
                Player_Camera2.SetActive(false);
            }
        }

    }
    public void BackToStartMenu()
    {

        StartCoroutine(DelayBackToStartMenu());

        //  DelayLoadLevel(0);
    }
    public IEnumerator DelayLoadLevel(int leveID)
    {
        PlayMaskLeaveClip();
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene(leveID);
    }
    public IEnumerator DelayBackToStartMenu()
    {
        PlayMaskLeaveClip();
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene(GlobalRules.instance.StartSceneName);
        Destroy(this.transform.parent.parent.gameObject);
    }
    public IEnumerator AutoDestory()
    {
        while (true)
        {
            MenualCheckDestory();
            //Debug.Log("AutoDestory");
            yield return new WaitForSeconds(3f);
        }
    }
    public void PlayerParticle()//play the particle cover
    {
        SwitchParticleSystem.Play();
        //OriginalMaterialList.Clear();
        //foreach (var item in playerMeshRenderer.materials)
        //{
        //    OriginalMaterialList.Add(item);
        //}
    }
    public void SetPlayerSkin(int targetSkinIndex)
    {
        for (int i = 0; i < playerSkinList.Count; i++)
        {
            if (i == targetSkinIndex)
            {
                playerSkinList[i].SetActive(true);
            }
            else { playerSkinList[i].SetActive(false); }

        }
    }
    public void PlayMaskLeaveClip()
    {
        MaskAnimator.Play("Leave");
    }
}
