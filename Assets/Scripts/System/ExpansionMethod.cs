using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExpansionMethod
{
    public static Vector2Int Vector2(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector2Int.up;
            case Direction.Right:
                return Vector2Int.right;
            case Direction.Down:
                return Vector2Int.down;
            case Direction.Left:
                return Vector2Int.left;
        }
        return Vector2Int.zero;
    }

    public static Direction ReverseDir(this Direction direction)
    {
        int result = (int)direction + 2;
        if (result >= (int)Direction.Max) result -= (int)Direction.Max;

        return (Direction)result;
    }
}
