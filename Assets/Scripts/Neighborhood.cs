using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighborhood
{
    static Point[] MooreOffsets = new Point[8]
    {
        new Point(0,1),
        new Point(1,1),
        new Point(1,0),
        new Point(1,-1),
        new Point(0,-1),
        new Point(-1,-1),
        new Point(-1,0),
        new Point(-1,1)
    };

    static Point[] vonNeumannOffsets = new Point[4]
    {
        new Point(0,1),
        new Point(1,0),
        new Point(0,-1),
        new Point(-1,0),
    };

    static Point[] noOffsets = new Point[0]
    {
    };

    static Point[] hybridOffsets = new Point[12]
    {
        new Point(0,1),
        new Point(1,1),
        new Point(1,0),
        new Point(1,-1),
        new Point(0,-1),
        new Point(-1,-1),
        new Point(-1,0),
        new Point(-1,1),
        new Point(0,2),
        new Point(2,0),
        new Point(0,-2),
        new Point(-2,0),
    };

    private NType neighborType;

    public NType NeighborType
    {
        get { return neighborType; }
        set { neighborType = value; }
    }

    public Neighborhood(NType type)
    {
        neighborType = type;
    }

    public List<Point> GetNeighbors(int x, int y)
    {
        return GetNeighbors(new Point(x, y));
    }

    public List<Point> GetNeighbors(Point cell)
    {
        switch (neighborType)
        {
            case NType.None:
                return GetNoNeighbors(cell);
            case NType.VonNeumann:
                return GetVonNeumannNeighbors(cell);
            case NType.Moore:
                return GetMooreNeighbors(cell);
            case NType.Hybrid:
                return GetHybridNeighbors(cell);
            default:
                throw new ArgumentException("Unknown Neighborhood type in GetNeighbors");
        }
    }

    private List<Point> GetMooreNeighbors(Point cell)
    {
        List<Point> neighbors = new List<Point>();
        foreach (Point p in MooreOffsets)
            neighbors.Add(cell + p);
        return neighbors;
    }

    private List<Point> GetVonNeumannNeighbors (Point cell)
    {
        List<Point> neighbors = new List<Point>();
        foreach (Point p in vonNeumannOffsets)
            neighbors.Add(cell + p);
        return neighbors;
    }

    private List<Point> GetHybridNeighbors(Point cell)
    {
        List<Point> neighbors = new List<Point>();
        foreach (Point p in hybridOffsets)
            neighbors.Add(cell + p);
        return neighbors;
    }

    private List<Point> GetNoNeighbors(Point cell)
    {
        List<Point> neighbors = new List<Point>();
        foreach (Point p in noOffsets)
            neighbors.Add(cell + p);
        return neighbors;
    }

    public int GetNeighborSize()
    {
        switch (neighborType)
        {
            case NType.None:
                return 0;
            case NType.VonNeumann:
                return 4;
            case NType.Moore:
                return 8;
            case NType.Hybrid:
                return 12;
            default:
                throw new ArgumentException("Unknown Neighborhood type in GetNeighborSize");
        }
    }

}

public enum NType
{
    None,
    VonNeumann,
    Moore,
    Hybrid,
}