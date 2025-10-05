using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private Transform target;
    public float smoothSpeed = 5f;
    public Camera camera;
    void Start()
    {
        GameObject player = GameObject.Find("Player");
        target = player.transform;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, -10);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            camera.orthographicSize = 50;
        }
        else camera.orthographicSize = 10;
    }
}
