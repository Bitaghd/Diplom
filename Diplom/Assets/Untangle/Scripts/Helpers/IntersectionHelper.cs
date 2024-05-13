using UnityEngine;

public class IntersectionHelper
{


    public bool AreIntersecting(EdgeObject line1, EdgeObject line2)
    {
        Vector2 endPointA1 = line1.Vertex1;
        Vector2 endPointA2 = line1.Vertex2;
        Vector2 endPointB1 = line2.Vertex1;
        Vector2 endPointB2 = line2.Vertex2;

        double orientedArea1 = CalculateTriangleOrientedArea(endPointA1, endPointA2, endPointB1);
        double orientedArea2 = CalculateTriangleOrientedArea(endPointA1,endPointA2, endPointB2);


        if (orientedArea1 * orientedArea2 >= 0.0)
            return false;

        double orientedArea3 = CalculateTriangleOrientedArea(endPointB1, endPointB2, endPointA1);
        double orientedArea4 = CalculateTriangleOrientedArea(endPointB1, endPointB2, endPointA2);
        return (orientedArea3 * orientedArea4 < 0.0);

    }

    public bool AreIntersecting(Vector2 endPointA1, Vector2 endPointA2, Vector2 endPointB1, Vector2 endPointB2) 
    {

        double orientedArea1 = CalculateTriangleOrientedArea(endPointA1, endPointA2, endPointB1);
        double orientedArea2 = CalculateTriangleOrientedArea(endPointA1,endPointA2, endPointB2);


        if (orientedArea1 * orientedArea2 >= 0.0)
            return false;

        double orientedArea3 = CalculateTriangleOrientedArea(endPointB1, endPointB2, endPointA1);
        double orientedArea4 = CalculateTriangleOrientedArea(endPointB1, endPointB2, endPointA2);
        return (orientedArea3 * orientedArea4 < 0.0);
    }

    private double CalculateTriangleOrientedArea(Vector2 vertex1, Vector2 vertex2, Vector2 vertex3)
    {
        double x1 = vertex2.x - vertex1.x;
        double y1 = vertex2.y - vertex1.y;
        double x2 = vertex3.x - vertex1.x;
        double y2 = vertex3.y - vertex1.y;

        return (x1 * y2 - x2 * y1) / 2.0;
    }
}
