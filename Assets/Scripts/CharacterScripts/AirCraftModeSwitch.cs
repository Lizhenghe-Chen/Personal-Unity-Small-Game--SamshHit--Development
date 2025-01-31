using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
/* Copyright (c) [2023] [Lizhneghe.Chen https://github.com/Lizhenghe-Chen]
* Please do not use these code directly without permission.
*/
public class AirCraftModeSwitch : MonoBehaviour
{
    [Header("For PlayerBrain PositionConstraint target")][SerializeField] private Transform PlayerBrain;
    [Header("For Player switch Positoin Rest")][SerializeField] private Transform Player, Plane;


    private void OnEnable()//when aircraft is enabled, 
    {
        // StartCoroutine(LockPosition());
        Player.GetComponent<Rigidbody>().isKinematic = true;
        //Plane.forward = this.transform.parent.parent.GetComponent<Rigidbody>().velocity = Player.GetComponent<Rigidbody>().velocity;
        Plane.forward = CharacterCtrl._CharacterCtrl.Camera.transform.forward;
        PlayerBrain.GetComponent<PositionConstraint>().SetSource(0, new ConstraintSource() { sourceTransform = this.transform, weight = 1 });
        Plane.position = Player.position;
    }
    private void OnDisable()//when aircraft is disabled, switch back to player
    {
        Player.GetComponent<Rigidbody>().isKinematic = false;
        Player.GetComponent<Rigidbody>().velocity = this.transform.parent.parent.GetComponent<Rigidbody>().velocity;
        PlayerBrain.GetComponent<PositionConstraint>().SetSource(0, new ConstraintSource() { sourceTransform = Player, weight = 1 });
        Player.position = Plane.position;
    }
    void SetBrainPositionConstraint()
    {
        PlayerBrain.GetComponent<PositionConstraint>().SetSource(0, new ConstraintSource() { sourceTransform = Player, weight = 1 });
    }
    public IEnumerator LockPosition()
    {
        while (true)
        {
            Player.position = PlayerBrain.position;
            //Debug.Log("AutoDestory");
            yield return new WaitForSeconds(0.01f);
        }
    }
    // void RestPosition()
    // {
    //     if (isPlayer) { Player.position = Plane.position; }
    //     if (isPlane) { Plane.position = Player.position; }
    // }
}
