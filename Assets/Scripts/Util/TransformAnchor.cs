using UnityEngine;
using System.Collections;
using System.Linq;

[ExecuteInEditMode]
public class TransformAnchor : MonoBehaviour
{
    public enum HorizontalAnchor { LEFT, RIGHT }
    public enum VerticalAnchor { TOP, BOTTOM }

    public HorizontalAnchor horizontalAnchor;
    public float horizontalMargin;
    public VerticalAnchor verticalAnchor;
    public float verticalMargin;

    void Start()
    {
        // NOTE: running this in Start() instead of Update() means we're assuming the camera doesn't move
        updatePosition();
    }

    // Only run on Update() in editor mode because we want immediate feedback at design time
    // But we don't need the constant update overhead at runtime.
#if UNITY_EDITOR
    void Update()
    {
        if (!Application.isPlaying)
        {
            updatePosition();
        }
    }
#endif

    void updatePosition()
    {
        // Get the anchored screen edge
        float vx, vy;
        vx = (horizontalAnchor == HorizontalAnchor.LEFT) ? 0f : 1f;
        vy = (verticalAnchor == VerticalAnchor.BOTTOM) ? 0f : 1f;

        Vector3 worldPoint = Camera.main.ViewportToWorldPoint(new Vector3(vx, vy, Camera.main.nearClipPlane));

        // Add margin
        worldPoint = worldPoint + new Vector3(horizontalMargin, verticalMargin, 0f);

        // Apply position to transform.
        transform.position = worldPoint;
    }
}
