using UnityEngine;
using UnityEngine.U2D;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private float speed = 5.0F;

    [SerializeField]
    private Transform target;
    public BoxCollider2D boundary;

    private PixelPerfectCamera pixelPerfectCamera;
    private Vector3 minBounds;
    private Vector3 maxBounds;
    private float cameraHalfWidth;
    private float cameraHalfHeight;

    private void Awake()
    {
        if (!target) target = FindObjectOfType<Character>().transform;
        boundary = GameObject.Find("CameraTrigger").GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
        minBounds = boundary.bounds.min;
        maxBounds = boundary.bounds.max;
        cameraHalfHeight = pixelPerfectCamera.refResolutionY / (2f * pixelPerfectCamera.assetsPPU);
        cameraHalfWidth = pixelPerfectCamera.refResolutionX / (2f * pixelPerfectCamera.assetsPPU);
    }

    private void FixedUpdate()
    {
        Vector3 position = target.position;
        position.z = -10.0F;
        position.x = Mathf.Clamp(target.position.x, minBounds.x + cameraHalfWidth, maxBounds.x - cameraHalfWidth);
        position.y = Mathf.Clamp(target.position.y, minBounds.y + cameraHalfHeight, maxBounds.y - cameraHalfHeight);
        transform.position = Vector3.Lerp(transform.position, position, speed * Time.deltaTime);
    }
}
