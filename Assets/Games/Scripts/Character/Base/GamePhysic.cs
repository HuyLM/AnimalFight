using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePhysic {

    public static Vector3[] GraphPoint(Vector3 pointBegin, float force0, float angle, int numOfPoint, float density){
        Vector3[] array = new Vector3[numOfPoint];

        for (int i = 0; i < numOfPoint; i++)
        {
            Vector3 target = pointBegin + EndPointWithTime(force0, angle, i * density);
            array[i] = target;
        }

        return array;
    }

    private static Vector3 EndPointWithTime(float force0, float angle, float time)
    {
        float r = force0 * Mathf.Cos(angle * Mathf.Deg2Rad) * time;
        float h = force0 * Mathf.Sin(angle * Mathf.Deg2Rad) * time - (0.5f * Physics2D.gravity.magnitude * time * time);

        return new Vector3(r, h, 0);
    }

    public static Vector3 GetVelocityStartEnd(Vector3 pStart, Vector3 target, float angle){
        float Vi = GetForce0(pStart,target,angle);
        float h, r;

        h = Vi * Mathf.Sin(Mathf.Deg2Rad * angle);
        r = Vi * Mathf.Cos(Mathf.Deg2Rad * angle);

        Vector3 localVelocity = new Vector3(r, h, 0);

        return localVelocity;
    }

    public static float GetForce0(Vector3 pStart, Vector3 target, float angle){
        float dy = -pStart.y + target.y;
        float dx = -pStart.x + target.x;
        return Mathf.Sqrt(-Physics2D.gravity.y / (2 * (Mathf.Tan(Mathf.Deg2Rad * angle) * dx - dy))) * (dx / Mathf.Cos(Mathf.Deg2Rad * angle));
    }

    public static Vector3 GetVelocity0(float force0, float angle){
        float Vy, Vx;   // y,z components of the initial velocity

        Vy = force0 * Mathf.Sin(Mathf.Deg2Rad * angle);
        Vx = force0 * Mathf.Cos(Mathf.Deg2Rad * angle);

        // create the velocity vector in local space
        return new Vector3(Vx, Vy, 0);
    }
}
