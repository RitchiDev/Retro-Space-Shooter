using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    //--------------------------------------------------------------------------------
    // Make sure this class is a singleton
    //--------------------------------------------------------------------------------
    private static BoundaryManager instance;
    public static BoundaryManager Instance { get => instance; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    //--------------------------------------------------------------------------------
    // Class implementation
    //--------------------------------------------------------------------------------

    // References to the screen bounds. Used to ensure the player is not able to leave the screen.
    [Header("Boundary objects:")]
    public GameObject leftBoundary;                
    public GameObject rightBoundary;               
    public GameObject topBoundary;                 
    public GameObject bottomBoundary;

    // Actual boundary values
    [Header("Runtime boundary values:")]
    public float maxX = 0.0f;
    public float minX = 0.0f;
    public float maxY = 0.0f;
    public float minY = 0.0f;

    // Start is called before the first frame update
    public void Refresh()
    {
        if (leftBoundary != null)
            minX = leftBoundary.transform.position.x;
        if (rightBoundary != null)
            maxX = rightBoundary.transform.position.x;
        if (topBoundary != null)
            maxY = topBoundary.transform.position.y;
        if (bottomBoundary != null)
            minY = bottomBoundary.transform.position.y;
    }

    // Convenience methods
    public bool WithinBoundaryX(float x)
    {
        return (minX <= x && x <= maxX);
    }

    public bool WithinBoundaryY(float y)
    {
        return (minY <= y && y <= maxY);
    }

    public bool WithinBoundary(float x, float y)
    {
        return WithinBoundaryX(x) && WithinBoundaryY(y);
    }

    public Vector3 Clamp(Vector3 p)
    {
        return new Vector3(Mathf.Clamp(p.x, minX, maxX), Mathf.Clamp(p.y, minY, maxY), p.z);
    }

    public Vector2 Clamp(Vector2 p)
    {
        return new Vector2(Mathf.Clamp(p.x, minX, maxX), Mathf.Clamp(p.y, minY, maxY));
    }

    public Vector3 TopLeft()
    {
        return new Vector3(minX, maxY, 0.0f);
    }

    public Vector3 TopRight()
    {
        return new Vector3(maxX, maxY, 0.0f);
    }

    public Vector3 BottomLeft()
    {
        return new Vector3(minX, minY, 0.0f);
    }

    public Vector3 BottomRight()
    {
        return new Vector3(maxX, minY, 0.0f);
    }
}
