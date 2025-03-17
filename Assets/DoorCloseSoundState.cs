using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCloseSoundState : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.instance.PlaySE(SE.DOOR_CLOSE);
    }
}
