using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenSoundState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.instance.PlaySE(SE.DOOR_OPEN);
    }
}
