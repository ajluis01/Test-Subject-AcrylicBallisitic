using UnityEngine;

public class PaintingMovementArea : MonoBehaviour
{
    [SerializeField] Vector3 bounds; // full size of the area
    [SerializeField] float padding = 3.0f;

    Vector3 center;
    Vector3 o;
    Ray ray;

    // Vector3 randomPosition;
    // Vector3 randomNormal;
    // float timer = 0.5f;

    public Vector3 GetRandomPosition(out Vector3 outNormal)
    {
        outNormal = Vector3.zero;
        Vector3 randomPosition = center + new Vector3(
            Random.Range(-bounds.x / 2 + padding, bounds.x / 2 - padding),
            Random.Range(-bounds.y / 2, bounds.y / 2),
            Random.Range(-bounds.z / 2 + padding, bounds.z / 2 - padding)
        );
        int randomAxisIndex = Random.Range(0, 2);
        if (randomAxisIndex == 1) // project to z plane
        {
            outNormal = Vector3.forward;
            randomAxisIndex = 2;
        }
        else
        {
            outNormal = Vector3.right;
        }
        int randomBoundIndex = Random.Range(0, 2);
        randomPosition[randomAxisIndex] = randomBoundIndex == 0 ? -bounds[randomAxisIndex] / 2 : bounds[randomAxisIndex] / 2;
        outNormal = randomBoundIndex == 0 ? outNormal : -outNormal;
        return randomPosition;
    }

    public Vector3 GetCrossingPositionFromRay(Vector3 origin, Vector3 direction, out Vector3 outNormal)
    {
        Bounds areaBounds = new Bounds(center, bounds);
        Ray ray = new Ray(origin, direction);
        Vector3 min = areaBounds.min;
        Vector3 max = areaBounds.max;
        float closestDist = float.MaxValue;
        Vector3 closestPoint = ray.origin;
        outNormal = Vector3.zero;
    
        // Right face (+X)
        if (TryIntersectFace(ray, new Plane(Vector3.left, max), min.y, max.y, min.z, max.z, 
            out float dist, out Vector3 point) && dist < closestDist)
        {
            closestDist = dist;
            closestPoint = point;
            outNormal = Vector3.left;
            closestPoint.z = Mathf.Clamp(closestPoint.z, min.z + padding, max.z - padding);
        }
        
        // Left face (-X)
        if (TryIntersectFace(ray, new Plane(Vector3.right, min), min.y, max.y, min.z, max.z, 
            out dist, out point) && dist < closestDist)
        {
            closestDist = dist;
            closestPoint = point;
            outNormal = Vector3.right;
            closestPoint.z = Mathf.Clamp(closestPoint.z, min.z + padding, max.z - padding);
        }
        
        // Forward face (+Z)
        if (TryIntersectFace(ray, new Plane(Vector3.back, max), min.x, max.x, min.y, max.y, 
            out dist, out point) && dist < closestDist)
        {
            closestDist = dist;
            closestPoint = point;
            outNormal = Vector3.back;
            closestPoint.x = Mathf.Clamp(closestPoint.x, min.x + padding, max.x - padding);
        }
        
        // Back face (-Z)
        if (TryIntersectFace(ray, new Plane(Vector3.forward, min), min.x, max.x, min.y, max.y, 
            out dist, out point) && dist < closestDist)
        {
            closestDist = dist;
            closestPoint = point;
            outNormal = Vector3.forward;
            closestPoint.x = Mathf.Clamp(closestPoint.x, min.x + padding, max.x - padding);
        }
        return closestPoint;
    }

    bool TryIntersectFace(Ray ray, Plane plane, float u1, float u2, float v1, float v2, 
        out float distance, out Vector3 point)
    {
        point = Vector3.zero;
        
        if (!plane.Raycast(ray, out distance) || distance < 0)
            return false;
        
        point = ray.GetPoint(distance);
        
        // Check if point is within face bounds (depends on which plane)
        // This is simplified - you'd need to check the appropriate coordinates
        return true;
    }

    public bool IsOutOfBounds(Vector3 position)
    {
        Vector3 localPos = position - center;
        return Mathf.Abs(localPos.x) > bounds.x / 2 - padding && Mathf.Abs(localPos.z) > bounds.z / 2 - padding;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        center = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // timer -= Time.deltaTime;
        // if (timer <= 0.0f)
        // {
        //     randomPosition = GetRandomPosition(out randomNormal);
        //     timer = 0.5f;
        // }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, bounds);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(o, 1.0f);
        Gizmos.DrawRay(ray.origin, o + ray.direction * 1000.0f);

        // Gizmos.color = Color.blue;
        // Gizmos.DrawSphere(randomPosition, 1.0f);
        // Gizmos.color = Color.green;
        // Gizmos.DrawLine(randomPosition, randomPosition + randomNormal * 3.0f);
    }
}