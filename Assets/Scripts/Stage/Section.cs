using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section
{
    // •ûŠp
    public enum Direction
    {
        Up = 0,
        Right,
        Down,
        Left,
        Max
    }

    // ‹æ‰æ‚Ìí—Ş
    public enum SectionType
    {
        Invalid = -1,
        Room,           // •”‰®
        StartRoom,      // ƒXƒ^[ƒg•”‰®
        KeyRoom,        // Œ®•”‰®
        Corridor,       // ˜L‰º
        Max
    }

    // ˜L‰º‚Ìí—Ş
    public enum CorridorForm
    {
        Invalid = -1,
        I,
        L,
        T,
        X,
        Max
    }

    private int _ID = -1;
    private Vector2Int _position = -Vector2Int.one;
    private SectionType _type = SectionType.Invalid;
    private bool[] _isConnect = new bool[(int)Direction.Max];

    public void Initialize(int setID, Vector2Int setPos, SectionType setType)
    {
        _ID = setID;
        _position = setPos;
        _type = setType;
        for (int i = 0, max = _isConnect.Length; i < max; i++)
        {
            _isConnect[i] = false;
        }
    }
}
