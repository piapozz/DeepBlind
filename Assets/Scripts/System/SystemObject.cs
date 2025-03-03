/*
 * @file SystemObject.cs
 * @brief 
 * @author sein
 * @date 2025/2/5
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SystemObject : MonoBehaviour
{
    public abstract void Initialize();
    public virtual void Proc() { }
}
