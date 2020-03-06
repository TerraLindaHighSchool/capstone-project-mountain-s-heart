using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private struct PointInSpace
    {
        public Vector3 Position;
        public float Time;
    }

    private Vector3 spot = new Vector2();

    [SerializeField]
    [Tooltip("The transform to follow")]
    private Transform target;
    [SerializeField] private Vector2 minBoundary, maxBoundary;

    [SerializeField]

    [Tooltip("The offset between the target and the camera")]
    private Vector3 offset;

    [Tooltip("The delay before the camera starts to follow the target")]
    [SerializeField]
    private float delay = 0.5f;

    [SerializeField]
    [Tooltip("The speed used in the lerp function when the camera follows the target")]
    private float speed = 5;

    ///<summary>
    /// Contains the positions of the target for the last X seconds
    ///</summary>
    private Queue<PointInSpace> pointsInSpace = new Queue<PointInSpace>();

    void LateUpdate()
    {
        spot.x = Mathf.Clamp(transform.position.x, minBoundary.x, maxBoundary.x);
        spot.y = Mathf.Clamp(transform.position.y, minBoundary.y, maxBoundary.y);

        // Add the current target position to the list of positions
        pointsInSpace.Enqueue(new PointInSpace() { Position = target.position, Time = Time.time });

        // Move the camera to the position of the target X seconds ago 
        while (pointsInSpace.Count > 0 && pointsInSpace.Peek().Time <= Time.time - delay + Mathf.Epsilon)
        {
            transform.position = Vector3.Lerp(spot + offset, pointsInSpace.Dequeue().Position + offset, Time.deltaTime * speed);
        }
    }
}
