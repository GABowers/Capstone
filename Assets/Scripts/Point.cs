using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{

    private int y;
    private int x;

    public int X
    {
        get { return x; }
        set { x = value; }
    }

    public int Y
    {
        get { return y; }
        set { y = value; }
    }

    public Point()
        : this(0, 0)
    { }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    // Checks if a point is within a range of the Inclusive minimum and Exclusive maximum
    // Mainly useful for 2D arrays
    public bool WithinRange(int xMax, int yMax, int xMin = 0, int yMin = 0)
    {
        if (x < xMin || y < yMin)
            return false;
        if (x >= xMax || y >= yMax)
            return false;
        return true;
    }

    // adjust a point to wrap around the horizontal axis - Cylinder
    public static Point AdjustCylinderW(int width, Point p)
    {
        if (p.x < 0)
            p.x += width;
        if (p.x >= width)
            p.x -= width;
        return p;
    }

    // adjust a point to wrap around the vertical axis - Cylinder
    public static Point AdjustCylinderH(int height, Point p)
    {
        if (p.y < 0)
            p.y += height;
        if (p.y >= height)
            p.y -= height;
        return p;
    }

    // adjust a point to wrap around both axes - Torus
    public static Point AdjustTorus(int width, int height, Point p)
    {
        p = AdjustCylinderW(width, p);
        p = AdjustCylinderH(height, p);
        return p;
    }

    public static Point operator +(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y);
    }
}