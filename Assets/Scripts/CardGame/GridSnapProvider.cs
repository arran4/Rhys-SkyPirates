using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSnapProvider : MonoBehaviour
{
    [SerializeField] private Transform[] snapPoints; // size 9

    public int GetClosestSnapIndex(Vector3 worldPos, out float distance)
    {
        int bestIndex = -1;
        distance = float.MaxValue;

        for (int i = 0; i < snapPoints.Length; i++)
        {
            float d = Vector3.Distance(worldPos, snapPoints[i].position);
            if (d < distance)
            {
                distance = d;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    public Vector3 GetSnapWorldPosition(int index)
    {
        return snapPoints[index].position;
    }

    public Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        // Mapping must match your visual index contract
        int index = gridPos switch
        {
            { x: 0, y: 2 } => 0,
            { x: 1, y: 2 } => 1,
            { x: 2, y: 2 } => 2,

            { x: 0, y: 1 } => 3,
            { x: 1, y: 1 } => 4,
            { x: 2, y: 1 } => 5,

            { x: 0, y: 0 } => 6,
            { x: 1, y: 0 } => 7,
            { x: 2, y: 0 } => 8,

            _ => -1
        };

        return index >= 0 ? snapPoints[index].position : Vector3.zero;
    }
}
