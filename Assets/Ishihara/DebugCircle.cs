using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class DebugCircle : MonoBehaviour
{
    public static int segments = 50;

    public static List<Vector3> vertexList = new List<Vector3>();

    public static void AddCircle(Vector3 pos, float radius)
    {
        vertexList.Clear();
        float deltaTheta = (2f * Mathf.PI) / segments;
        float theta = 0f;

        for (int i = 0; i < segments + 1; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 temp = new Vector3(x + pos.x, 0f + pos.y, z + pos.z);
            vertexList.Add(temp);
            theta += deltaTheta;
        }

        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.zero;
        // ‰~‚ð•`‚­
        for (int i = 0; i < vertexList.Count; i++)
        {
            start = vertexList[i];
            if(i + 1 < vertexList.Count)end = vertexList[i + 1];
            else end = vertexList[0];

            Debug.DrawLine(start, end, Color.white);
        }
    }
}
