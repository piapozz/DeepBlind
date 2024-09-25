using UnityEngine;

public interface PlayerState
{
    // プレイヤーのステータス値
    public struct PlayerStatus
    {
        public float stamina;                                       // スタミナ量
        public float speed;                                         // 動く速さ
        public float fear;                                          // 怖気度
        public float soundRange;                                    // プレイヤーが出してしまう音の範囲
    }

    // 動き
    public void Move();


}
