using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDestory : MonoBehaviour
{
    public float explodeRadius = 5f;// should be the sphere collider trigger's radius
    public ParticleSystem playerHitParticleSystem, hitParticleSystem;
    //public GameObject ignoreCollisionObject;
    //public GameObject explosion;
    public float scaleOfTime = 0.01f;
    public bool ignorePlayerColletion, isPlayerBullet;
    private Rigidbody rb;
    //   private SphereCollider damageRange;
    //void Awake() { explosion.SetActive(false); }
    void OnEnable()
    {
        rb = this.GetComponent<Rigidbody>();
        if (ignorePlayerColletion)
        {//ignore collision by layer
            Physics.IgnoreLayerCollision(GlobalRules.instance.bulletLayerID, GlobalRules.instance.playerLayerID);
        }
        Destroy(this.gameObject, 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ignorePlayerColletion) { return; }
        //  if (!other.GetComponent<Rigidbody>()) { return; }
        try { other.gameObject.GetComponent<Rigidbody>().AddForce((other.transform.position - this.transform.position) * 1000); }
        catch (Exception) { }

        //  this.GetComponent<MeshRenderer>().enabled = false;




    }

    private void OnCollisionEnter(Collision other)
    {

        if (isPlayerBullet)
        {
            Time.timeScale = scaleOfTime;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            SetHitParticle(playerHitParticleSystem, other, 0.1f);
            Destroy(this.gameObject, 1f);
        }
        else
        {
            if (other.gameObject.layer == GlobalRules.instance.playerLayerID)
            {
                SetHitParticle(hitParticleSystem, other, 0.01f);
                
            }
            Destroy(this.gameObject,0.1f);
        }
        this.GetComponent<MeshRenderer>().enabled = false;

        if (other.gameObject.name == "Target") { Debug.Log("hit"); }
    }

    void SetHitParticle(ParticleSystem hitParticleSystem, Collision other, float scale)
    {
        var hitEffect = Instantiate(hitParticleSystem, other.GetContact(0).point, Quaternion.identity);
        hitEffect.transform.localScale = Vector3.one * 0.01f;
        hitEffect.transform.up = other.GetContact(0).normal;
        foreach (Transform item in hitEffect.transform)
        {
            item.localScale = Vector3.one * scale;
            item.up = other.GetContact(0).normal;
        }

        Destroy(hitEffect.gameObject, 2f);
        // Debug.Log("Player hit by bullet with " + other.relativeVelocity.magnitude);
    }

}
