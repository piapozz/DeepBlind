using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorMove : MonoBehaviour
{
    [SerializeField] private GameObject[] corridors;
    [SerializeField] private float corridorMoveSpeed = 1.0f;

    void Update()
    {
        for (int i = 0, max = corridors.Length; i < max; i++)
        {
            // ˆÊ’u‚ðXV
            Vector3 corridorPos = new Vector3(0.0f, 0.0f, 0.0f);
            corridorPos = corridors[i].transform.position;
            corridorPos.z -= Time.deltaTime * corridorMoveSpeed;
            if (corridorPos.z <= -5.0f) { corridorPos.z += max * 10.0f; }
            corridors[i].transform.position = corridorPos;
        }
    }
}
