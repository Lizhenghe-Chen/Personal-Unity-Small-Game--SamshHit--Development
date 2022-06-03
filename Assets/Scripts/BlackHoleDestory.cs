using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlackHoleDestory : MonoBehaviour
{
    private Vector3 objectDestory = new(0.2f, 0.2f, 0.2f);
    void OnTriggerStay(Collider other)
    {
        var rigidbody = other.gameObject.GetComponent<Rigidbody>();
        if (!rigidbody || rigidbody.CompareTag("BlackHole")) { return; }
        if (other.transform.localScale.x <= 0.01f)
        {
            if (other.CompareTag("Player")) { CharacterCtrl._CharacterCtrl.LoadScene(0); CenterRotate.shootEnergy = 0; return; }
            Destroy(other.gameObject);
        }
        else
        {
            if (other.CompareTag("Player")) { other.transform.localScale -= objectDestory; }
            else
                other.transform.localScale -= objectDestory;
            rigidbody.mass /= 2;
        }
    }
}
