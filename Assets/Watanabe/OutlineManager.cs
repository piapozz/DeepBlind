using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    public Camera mainCamera;  // �v���C���[�̃J����
    public float maxDistance = 10f; // ���C�̍ő勗��
    public float maxOutlineStrength = 1.5f; // �ő�A�E�g���C�����x
    public float fadeDistance = 3f; // �A�E�g���C������܂鋗��
    public string targetTag = "ItemObject"; // �^�O��ݒ�

    private Material lastHitMaterial; // �Ō�Ƀq�b�g�����I�u�W�F�N�g�̃}�e���A��

    void Update()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider.CompareTag(targetTag)) // ����̃^�O�����I�u�W�F�N�g�̂ݓK�p
            {
                Debug.Log("bbb");
                Renderer rend = hit.collider.GetComponent<Renderer>();
                if (rend != null)
                {
                    Material mat = rend.material;
                    lastHitMaterial = mat;

                    float distance = hit.distance;
                    float outlineStrength = Mathf.Lerp(maxOutlineStrength, 0, distance / fadeDistance);
                    mat.SetFloat("_OutlineStrength", outlineStrength);
                }
            }
        }
        else
        {
            // �q�b�g���Ȃ�������A�E�g���C��������
            if (lastHitMaterial != null)
            {
                lastHitMaterial.SetFloat("_OutlineStrength", 0);
                lastHitMaterial = null;
            }
        }
    }
}
