using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxSync : MonoBehaviour
{
    BoxCollider boxCollider;
    Bounds bounds;
    public GameObject Bounds;
    [SerializeField] GameObject parent;

    private Quaternion initialLocalRotation;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        bounds = Bounds.GetComponent<SkinnedMeshRenderer>().bounds;// �q�I�u�W�F�N�g�̏������[�J����]��ۑ�
        initialLocalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        // �e�I�u�W�F�N�g�̉�]���擾
        Quaternion parentRotation = transform.parent.rotation;

        // �q�I�u�W�F�N�g�̃��[�J����]���A�e�̋t��]�Ə�����]�ɐݒ�
        transform.localRotation = Quaternion.Inverse(parentRotation) * initialLocalRotation;

        // SkinnedMeshRenderer��Bounds���擾
        var bounds = Bounds.GetComponent<SkinnedMeshRenderer>().bounds;

        // BoxCollider�̃��[�J�����W�ł̒��S���v�Z
        var localCenter = Quaternion.Inverse(boxCollider.gameObject.transform.rotation) *
            new Vector3(
            (bounds.center.x - boxCollider.gameObject.transform.position.x) / boxCollider.gameObject.transform.lossyScale.x,
            (bounds.center.y - boxCollider.gameObject.transform.position.y) / boxCollider.gameObject.transform.lossyScale.y,
            (bounds.center.z - boxCollider.gameObject.transform.position.z) / boxCollider.gameObject.transform.lossyScale.z);

        // BoxCollider�̒��S��ݒ�
        boxCollider.center = localCenter;

        // BoxCollider�̃��[�J�����W�ł̃T�C�Y���v�Z
        var localSize = new Vector3(
            bounds.size.x / boxCollider.gameObject.transform.lossyScale.x,
            bounds.size.y / boxCollider.gameObject.transform.lossyScale.y,
            bounds.size.z / boxCollider.gameObject.transform.lossyScale.z);

        // BoxCollider�̃T�C�Y��ݒ�
        boxCollider.size = localSize;
    }
}
