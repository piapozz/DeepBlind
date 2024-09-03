using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GenerateStage generateStage;
    [SerializeField] CharacterController characterController;

    Vector3 offsetgenPos = new Vector3(0, 1.5f, 0);
    float moveSpeed = 5.0f;
    Vector3 moveVec;
    Vector2 rotateVec;

    void Start()
    {
        transform.position = generateStage.GetStartPos() + offsetgenPos;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Move();

        Rotate();
    }

    void Move()
    {
        Vector3 verMove = new Vector3(transform.forward.x, 0, transform.forward.z) * Input.GetAxis("Vertical");
        Vector3 horiMove = transform.right * Input.GetAxis("Horizontal");

        moveVec = horiMove + verMove;
        moveVec.Normalize();
        moveVec = moveVec * moveSpeed;

        characterController.Move(moveVec * Time.deltaTime);
    }

    void Rotate()
    {
        rotateVec.x = Input.GetAxis("Mouse X");
        rotateVec.y = Input.GetAxis("Mouse Y");

        transform.rotation = 
            Quaternion.Euler(0, rotateVec.x, 0) * 
            transform.rotation * 
            Quaternion.Euler(-rotateVec.y, 0, 0);
    }

    // Ç«ÇÃãÊâÊÇ…Ç¢ÇÈÇ©Çï‘Ç∑ä÷êî
    public Vector2Int GetNowSection()
    {
        float width = transform.position.x / generateStage.GetSectionSize() + 0.5f;
        float height = transform.position.z / generateStage.GetSectionSize() + 0.5f;

        return new Vector2Int(Mathf.FloorToInt(width), Mathf.FloorToInt(height));
    }

    // åªç›ÇÃç¿ïWÇï‘Ç∑ä÷êî
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    // ÉJÉÅÉâÇï‘Ç∑ä÷êî
    public Camera GetCamera()
    {
        return Camera.main;
    }
}
