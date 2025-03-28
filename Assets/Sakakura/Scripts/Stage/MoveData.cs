/*
* @file MoveData.cs
* @brief à⁄ìÆÉfÅ[É^
* @author sakakura
* @date 2025/3/14
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveData
{
    public int sourceSectionID = -1;
    public int targetSectionID = -1;
    public Direction direction = Direction.Invalid;

    public MoveData(int setSourceID, int setTargetID, Direction setDireciton)
    {
        sourceSectionID = setSourceID;
        targetSectionID = setTargetID;
        direction = setDireciton;
    }
}
