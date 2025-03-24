using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayMovie : MonoBehaviour
{
    [SerializeField] VideoPlayer player;    // �I���҂�����r�f�I

    // Start is called before the first frame update
    void Start()
    {
        player.loopPointReached += ChangeTitle;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) ChangeTitle(null);
    }

    public void ChangeTitle(VideoPlayer SetPlayer)
    {
        SceneChanger.SceneChange("Title");
    }
}
