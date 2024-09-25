using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientManager : MonoBehaviour
{
    // 受け手と送り手を分けて判定をboolで返す bool = 感知した
    // Receiverに音を聞く側の情報を入れる Senderに音を鳴らしている側の情報を入れる
    public bool CheckHitAmbient(Vector3 receiverPosition, float receiverRange, Vector3 senderPos, float senderRange)
    {
        // 計算用の変数
        Vector2 receiverTemp = new Vector2(receiverPosition.x, receiverPosition.z);
        Vector2 senderTemp = new Vector2(senderPos.x, senderPos.z);

        // 三平方の定理を使って距離を出す
        float width = Mathf.Pow(receiverTemp.x - senderTemp.x, 2);
        float height = Mathf.Pow(receiverTemp.y - senderTemp.y, 2);

        // 距離を計算
        float radius = Mathf.Sqrt(width) + Mathf.Sqrt(height);

        // 距離を比較する
        if(radius <= receiverRange + senderRange)
        {
            // 範囲に入っていたらtrue
            return true;
        }

        // 範囲外だったらfalse
        return false;
    }
}
