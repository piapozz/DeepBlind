using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵の基本データ

[CreateAssetMenu]
public class EnemyPram : ScriptableObject
{
    public int id;                      // エネミーの識別番号
    public float speed;                 // エネミーの速さ
    public float speedDiameter;         // 見つけた時の速さの倍率
    public float animSpeed;             // アニメーションの速さ
    public float threatRange;           // 脅威範囲
    public float viewLength;            // 視界の長さ
    public float fieldOfView;           // 視野角
}
