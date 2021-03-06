﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line
{

    float gradient;
    float y_intercept;
    Vector2 pointOnLine_1;
    Vector2 pointOnLine_2;

    bool approachSide;

    const float deltaWhenZero = 1e5f;

    float gradientPerpendicular;

    public Line (Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        if (dx == 0)
        {
            gradientPerpendicular = deltaWhenZero;
        }
        else
        {
            gradientPerpendicular = dy / dx;
        }
        if (gradientPerpendicular == 0)
        {
            gradient = deltaWhenZero;
        }
        else
        {
            gradient = -1 / gradientPerpendicular;
        }
        y_intercept = pointOnLine.y - gradient * pointOnLine.x;
        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = pointOnLine + new Vector2 (1, gradient);

        approachSide = false;
        approachSide = GetSide (pointPerpendicularToLine);
    }

    bool GetSide (Vector2 p)
    {
        return (p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
    }

    public bool HasCrossedLine(Vector2 p)
    {
        return GetSide(p) != approachSide;
    }

    public void DrawWithGizmos (float lenght)
    {
        Vector3 lineDir = new Vector3(1, 0, gradient).normalized;
        Vector3 lineCentre = new Vector3(pointOnLine_1.x, 0, pointOnLine_1.y) + Vector3.up;
        Gizmos.DrawLine(lineCentre - lineDir * lenght / 2f, lineCentre + lineDir * lenght / 2f);
    }
}
