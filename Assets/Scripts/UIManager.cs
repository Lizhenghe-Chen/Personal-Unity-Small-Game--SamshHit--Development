using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public bool isPlayer = true;
    public Transform Player;
    public GameObject InGameUI;
    public Image KernelState;
    public Color chargedColor, unchargedColor;

    public particleAttractorLinear kernelParticle;

    public Image holdAim;
    public Transform aimRay, HoldTarget, MainCamera;


    [Serializable]
    public struct Range
    {
        public float min;
        public float max;
    }
    public Range holdRange;
    [SerializeField] Rigidbody holdingObject;
    [SerializeField] private float value = 0f;
    [SerializeField] private bool isHoldKeyPressing = false;
    public LayerMask HoldRaycastIgnore;




    void Start()
    {
        holdAim.enabled = false;


    }

    // Update is called once per frame
    void Update()
    {

        HoldObjectCommand();
        //Debug.Log(vcam.m_Lens.FieldOfView);
    }

    private void FixedUpdate()
    {
        //if (!postProcessVolume)
        //{
        //    postProcessVolume = transform.parent.Find("Global Volume").GetComponent<Volume>();
        //    postProcessVolume.sharedProfile.TryGet<DepthOfField>(out df);
        //    df.focusDistance.value = focusDistanceSlider.value;
        //    df.aperture.value = aptureSlider.value;
        //    df.focalLength.value = focalLengthSlider.value;
        //    Debug.Log("postProcessVolume not found");
        //}
        if (isPlayer) { UpdateKernelStateBar(); HoldObject(); }
        else
        {
            SpectatorHoldObjectCommand();
            SpectatorHoldObject();
        }

    }

    private float originalDrag;
    private Transform originalKernelParticleTarget;

    public void HoldObjectCommand()
    {
        if (CenterRotate.shootEnergy <= 0) { return; }

        if (Input.GetKeyDown(GlobalRules.instance.HoldObject) && CenterRotate.shootEnergy > 1)
        {
            holdAim.enabled = true;
            isHoldKeyPressing = holdAim.enabled;

        }
        else { holdAim.enabled = false; }
        if (Input.GetKeyUp(GlobalRules.instance.HoldObject))
        {
            holdAim.enabled = false;
            isHoldKeyPressing = holdAim.enabled;

            if (holdingObject)
            {
                // holdingObject.isKinematic = false;
                holdingObject.drag = originalDrag;//change back it's drag
                kernelParticle.target = originalKernelParticleTarget;
                holdingObject = null;
            }

        }
    }
    public void SpectatorHoldObjectCommand()
    {

        if (Input.GetKey(GlobalRules.instance.HoldObject))
        {
            holdAim.enabled = true;
            isHoldKeyPressing = holdAim.enabled;

        }
        else
        {
            holdAim.enabled = false;
            isHoldKeyPressing = holdAim.enabled;

            if (holdingObject)
            {
                // holdingObject.isKinematic = false;
                holdingObject.drag = originalDrag;//change back it's drag

                holdingObject = null;
            }

        }
    }
    [SerializeField] float holdDistance = 10;
    public void HoldObject()
    {
        if (!isHoldKeyPressing) { return; }
        if (Input.GetKey(GlobalRules.instance.ExtemdHoldObjectDist) && (holdDistance <= holdRange.max)) { holdDistance += Time.deltaTime * 5f; }
        else if (Input.GetKey(GlobalRules.instance.CloseHoldObjectDist) && (holdDistance >= holdRange.min)) { holdDistance -= Time.deltaTime * 5f; }
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(MainCamera.position, MainCamera.TransformDirection(Vector3.forward), out RaycastHit hit, holdRange.max, ~HoldRaycastIgnore))
        {
            if (hit.rigidbody && holdingObject == null)
            {
                holdAim.enabled = false;
                HoldTarget.position = hit.point;
                HoldTarget.LookAt(MainCamera);
                holdDistance = Vector3.Distance(MainCamera.position, hit.point);
                holdingObject = hit.collider.gameObject.GetComponent<Rigidbody>();

                originalDrag = holdingObject.drag;
                originalKernelParticleTarget = kernelParticle.target;

                holdingObject.drag = 5f;
                kernelParticle.target = holdingObject.transform;
                //holdingObject.isKinematic = true;
                // Debug.DrawRay(MainCamera.transform.position, MainCamera.transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
                Debug.Log(hit.transform.tag);
            }
        }
        else
        {
            Debug.DrawRay(MainCamera.transform.position, MainCamera.transform.TransformDirection(Vector3.forward) * holdRange.max, Color.red);
        }
        if (holdingObject)
        {
            CenterRotate.shootEnergy -= Time.deltaTime * GlobalRules.instance.holdConsume;
            //holdingObject.transform.position = Vector3.Lerp(holdingObject.transform.position, HoldTarget.position, Time.deltaTime * 1000f);
            HoldTarget.position = ((MainCamera.forward).normalized * holdDistance + MainCamera.position);
            holdingObject.AddForce(50f * holdingObject.mass * (HoldTarget.position - holdingObject.position));
            //holdingObject.MovePosition((HoldTarget.position - holdingObject.position) * 50f * Time.deltaTime + holdingObject.position);

        }
    }
    public void SpectatorHoldObject()
    {
        if (!isHoldKeyPressing) { return; }
        if (Input.GetKey(GlobalRules.instance.ExtemdHoldObjectDist) && (holdDistance <= holdRange.max)) { holdDistance += Time.deltaTime * 5f; }
        else if (Input.GetKey(GlobalRules.instance.CloseHoldObjectDist) && (holdDistance >= holdRange.min)) { holdDistance -= Time.deltaTime * 5f; }
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(MainCamera.position, MainCamera.TransformDirection(Vector3.forward), out RaycastHit hit, holdRange.max, ~HoldRaycastIgnore))
        {
            if (hit.rigidbody && holdingObject == null)
            {
                holdAim.enabled = false;
                HoldTarget.position = hit.point;
                HoldTarget.LookAt(MainCamera);
                holdDistance = Vector3.Distance(MainCamera.position, hit.point);
                holdingObject = hit.collider.gameObject.GetComponent<Rigidbody>();

                originalDrag = holdingObject.drag;
                holdingObject.drag = 5f;

            }
        }
        else
        {
            Debug.DrawRay(MainCamera.transform.position, MainCamera.transform.TransformDirection(Vector3.forward) * holdRange.max, Color.red);
        }
        if (holdingObject)
        {
            CenterRotate.shootEnergy -= Time.deltaTime * GlobalRules.instance.holdConsume;
            //holdingObject.transform.position = Vector3.Lerp(holdingObject.transform.position, HoldTarget.position, Time.deltaTime * 1000f);
            HoldTarget.position = ((MainCamera.forward).normalized * holdDistance + MainCamera.position);
            holdingObject.AddForce(50f * holdingObject.mass * (HoldTarget.position - holdingObject.position));
            //holdingObject.MovePosition((HoldTarget.position - holdingObject.position) * 50f * Time.deltaTime + holdingObject.position);

        }
    }
    void UpdateKernelStateBar()
    {
        value = CenterRotate.shootEnergy / 100f;
        KernelState.color = (value >= 1) ? chargedColor : unchargedColor;
        KernelState.fillAmount = value;
    }





}
