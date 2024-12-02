using UnityEngine;

public class TreeEyes : MonoBehaviour
{
    public Transform leftEye;  // Reference to the left eye sphere
    public Transform rightEye; // Reference to the right eye sphere
    public LineRenderer leftLineRenderer;
    public LineRenderer rightLineRenderer;
    public float rotationAngle = 30f;  // Angle span for the rotation
    public float rotationSpeed = 1f;   // Speed of rotation
    public float rayLength = 20f;      // Length of the laser beam
    public float downwardAngle = 45f;  // Downward angle for the beams

    void Start()
    {
        // Initialize Line Renderers
        InitializeLineRenderer(leftLineRenderer);
        InitializeLineRenderer(rightLineRenderer);
    }

    void Update()
    {
        // Calculate the rotation angle
        float angle = Mathf.Sin(Time.time * rotationSpeed) * rotationAngle;

        // Rotate left eye sphere around its local Y axis and tilt downward
        leftEye.localRotation = Quaternion.Euler(downwardAngle, angle, 0);

        // Rotate right eye sphere around its local Y axis and tilt downward
        rightEye.localRotation = Quaternion.Euler(downwardAngle, angle, 0);

        // Cast rays and update Line Renderers
        UpdateLaserBeam(leftEye, leftLineRenderer);
        UpdateLaserBeam(rightEye, rightLineRenderer);
    }

    void InitializeLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Use a suitable shader
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void UpdateLaserBeam(Transform eye, LineRenderer lineRenderer)
    {
        RaycastHit hit;
        Vector3 direction = eye.forward;
        Vector3 endPosition = eye.position + direction * rayLength;

        if (Physics.Raycast(eye.position, direction, out hit, rayLength))
        {
            // If the ray hits something, shorten the beam
            endPosition = hit.point;
        }

        // Update Line Renderer positions
        lineRenderer.SetPosition(0, eye.position);
        lineRenderer.SetPosition(1, endPosition);

        // Optional: Debug visualization
        Debug.DrawRay(eye.position, direction * rayLength, Color.red);
    }
}
