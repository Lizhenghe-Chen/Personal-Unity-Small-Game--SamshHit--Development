using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
//using ui
using UnityEngine.UI;
/* Copyright (c) [2023] [Lizhneghe.Chen https://github.com/Lizhenghe-Chen]
* Please do not use these code directly without permission.
*/
public class GunScript : MonoBehaviour
{
    [Header("Need Assign in Inspector")]
    public Transform Player;
    public Transform Plane;
    public Transform muzzle;
    public GameObject AimPoint;
    public Transform shootTraget;
    public PlayerBrain BuckyBall;

    //================================================================
    [Header("\n")]
    public Transform PlayerKernelTarget;
    public float PlayerKernelSpeed = 3f;
    public float cameraOffset;
    public Camera Camera;
    public bool isLookAt;

    public float bulletSpeed;
    public GameObject bullet;


    [SerializeField] GameObject GreenAim, RedAim;
    private void Start()
    {
        Camera = CharacterCtrl._CharacterCtrl.Camera;
        GreenAim = AimPoint.transform.Find("GreenAim").gameObject;
        RedAim = AimPoint.transform.Find("RedAim").gameObject;
        GreenAim.SetActive(false);
        RedAim.SetActive(false);
        // AimPoint.enabled = false;
    }
    void OnEnable()
    {



    }

    // Update is called once per frame
    void Update()
    {
        MovePlayerKernel();
        if (Player == null) { return; }
        PreShoot();
        // var position = new Vector3(Player.position.x + cameraOffset, Player.position.y, Player.position.z);

        // this.transform.position = Vector3.Lerp(this.transform.position, position, 1f * Time.deltaTime);
    }
    // private void FixedUpdate()
    // {

    // }
    void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
           PlayerBrain.instance.shootEnergy -= 25;
            //  Instantiate(bullet, muzzle.position, muzzle.rotation).GetComponent<Rigidbody>().AddForce(muzzle.forward * bulletSpeed);
            var temp = Instantiate(bullet, transform.position, Quaternion.identity);
            //shoot bullet from screen center

            Physics.IgnoreCollision(temp.GetComponent<Collider>(), Player.GetComponent<Collider>(), true);
            if (CharacterCtrl._CharacterCtrl.PlayerAnimator.GetBool("ToPlane"))
            {
                temp.GetComponent<Rigidbody>().velocity = Plane.transform.forward * bulletSpeed;
            }
            else { temp.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed; }

        }
    }
    void PreShoot()//!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    {
        if (!CharacterCtrl._CharacterCtrl.shootAbility) { return; }

        // this.transform.forward = Camera.transform.forward;
        if (Input.GetKey(GlobalRules.instance.PreShoot) || Input.GetKey(GlobalRules.instance.HoldObject))
        {
            // if (shootTraget.gameObject.activeSelf) { transform.LookAt(shootTraget); }
            transform.LookAt(shootTraget);
            // if (isLookAt) { transform.LookAt(Camera.transform); } else { transform.forward = new(-Camera.transform.forward.x, 0, -Camera.transform.forward.z); }
            AimPoint.SetActive(true);

            if (PlayerBrain.instance.shootEnergy > 25)
            {
                //BuckyBall.selfRotateSpeed = 10f;
                GreenAim.SetActive(true);
                RedAim.SetActive(false);
                Shoot();
            }
            else
            {
                // BuckyBall.selfRotateSpeed = 1f;
                GreenAim.SetActive(false);
                RedAim.SetActive(true);
            }
        }
        else
        {
            // BuckyBall.selfRotateSpeed = 1f;
            AimPoint.SetActive(false);
        }

    }
    void MovePlayerKernel()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, PlayerKernelTarget.position, PlayerKernelSpeed * Time.deltaTime);
    }
}
