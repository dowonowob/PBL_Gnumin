using UnityEngine;

public class DeviceMotionTracker : MonoBehaviour
{
    public static DeviceMotionTracker Instance { get; private set; }

    public Vector3 Velocity { get; private set; }
    public float Speed => Mathf.Max(Velocity.magnitude, 4.0f);

    private Vector3 _prevPosition;
    private float _prevTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        _prevPosition = Camera.main.transform.position;
        _prevTime = Time.time;
    }

    void Update()
    {
        if (Camera.main == null) return;

        Vector3 currentPosition = Camera.main.transform.position;
        float currentTime = Time.time;
        float deltaTime = currentTime - _prevTime;

        if (deltaTime > 0f)
        {
            Velocity = (currentPosition - _prevPosition) / deltaTime;
        }

        _prevPosition = currentPosition;
        _prevTime = currentTime;
    }
}
