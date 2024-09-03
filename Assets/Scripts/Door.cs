using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        if (animator.GetBool("open") == true)
            animator.SetBool("open", false);
        else
            animator.SetBool("open", true);
    }

    public bool GetOpen()
    {
        return animator.GetBool("open");
    }
}
