using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{

    public static List<Vector3> GetPointsToScreen(int row, int colume)
    {
        List<Vector3> points = new List<Vector3>();
        float height = 2f * Camera.main.orthographicSize;
        float width = height * Camera.main.aspect;

        float stepX = width / colume;
        float stepY = height / row;

        for (int i = 1; i < row; i++)
        {
            for (int j = 1; j < colume; j++)
            {
                Vector3 p = Constants.BottomLeft + new Vector3(j * stepX, i * stepY);
                points.Add(new Vector3(p.x,p.y,0));
            }
        }

        return points;
    }

}
