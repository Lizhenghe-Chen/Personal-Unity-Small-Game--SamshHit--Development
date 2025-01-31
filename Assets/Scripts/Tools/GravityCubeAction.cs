using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright (c) [2023] [Lizhneghe.Chen https://github.com/Lizhenghe-Chen]
* Please do not use these code directly without permission.
*/
public class GravityCubeAction : MonoBehaviour
{
    public Material hittedObjectMaterial;
    public List<string> takeActionHitterList = new();
    private void OnCollisionEnter(Collision other)
    {
        if (takeActionHitterList.Contains(other.gameObject.tag))
        {
            GetComponent<MeshRenderer>().material = hittedObjectMaterial;
            GetComponent<Light>().enabled = true;
            //  GetComponent<Rigidbody>().useGravity = false;
            if (CharacterCtrl._CharacterCtrl.HitObjectsQueue.Contains(gameObject)) { return; }
            CharacterCtrl._CharacterCtrl.HitObjectsQueue.Enqueue(gameObject);
            Destroy(gameObject, 4f);
            // CharacterCtrl._CharacterCtrl.Test.Enqueue(gameObject);
        }

    }
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (takeActionHitterList.Contains(other.gameObject.tag))
    //     {
    //         GetComponent<MeshRenderer>().material = hittedObjectMaterial;
    //         GetComponent<Light>().enabled = true;
    //         //  GetComponent<Rigidbody>().useGravity = false;
    //         if (CharacterCtrl._CharacterCtrl.HitObjectsQueue.Contains(gameObject)) { return; }
    //         CharacterCtrl._CharacterCtrl.HitObjectsQueue.Enqueue(gameObject);
    //         // CharacterCtrl._CharacterCtrl.Test.Enqueue(gameObject);
    //     }
    // }
}
