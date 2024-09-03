using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    bool canMove = true;

    // プレイヤーのステータス値
    public struct PlayerStatus
    {
        public float stamina;               // スタミナ量
        public float speed;                 // 動く速さ
        public float fear;                  // 怖気度
        public float soundRange;            // プレイヤーが出してしまう音の範囲
    }

    [SerializeField] IMove iMove;

    // 初期化
    void Start()
    {
        iMove = new PlayerWalk();
    }

    // 処理
    private void FixedUpdate()
    {
        if (canMove)
        {
            // 動く
            iMove.Move();

            // もし～キーが押されている状態ならダッシュに切り替える
            // iMove = new PlayerDash();
        }

        // 怖気を減らす





    }
}
