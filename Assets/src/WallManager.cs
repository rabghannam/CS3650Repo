using UnityEngine;

public class WallManager : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    [SerializeField] private Transform topWall;
    [SerializeField] private Transform bottomWall;
    [SerializeField] private Transform leftWall;
    [SerializeField] private Transform rightWall;

    [SerializeField] private float wallThickness = 1.0f;

    private void Start()
    {
        float height = targetCamera.orthographicSize * 2.0f;
        float width = height * targetCamera.aspect;

        Vector3 center = targetCamera.transform.position;

        // Horizontal walls
        topWall.position = center + Vector3.up * (height / 2.0f + wallThickness / 2.0f);
        bottomWall.position = center + Vector3.down * (height / 2.0f + wallThickness / 2.0f);

        topWall.localScale = new Vector3(width + wallThickness * 2.0f, wallThickness, 1.0f);
        bottomWall.localScale = topWall.localScale;

        // Vertical walls
        leftWall.position = center + Vector3.left * (width / 2.0f + wallThickness / 2.0f);
        rightWall.position = center + Vector3.right * (width / 2.0f + wallThickness / 2.0f);

        leftWall.localScale = new Vector3(wallThickness, height, 1.0f);
        rightWall.localScale = leftWall.localScale;
    }
}
