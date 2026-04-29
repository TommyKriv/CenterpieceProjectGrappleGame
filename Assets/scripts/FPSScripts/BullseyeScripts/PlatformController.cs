using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] Transform waypointsParent;

    [SerializeField] float travelTime = 5f;
    [SerializeField] float endPointStall = 3f;
    [SerializeField] float midPointStall = 1f;

    float dwellTimer = 0f;

    bool movingForward = true;

    int currentPointIndex = 0;

    [SerializeField] bool loop = false;
    [SerializeField] bool stopAtEndingPointOnLoopTrack = false;

    float stoppingDistThreshold = 0.05f;

    float t = 0f;

    Vector3 startPoint;
    Vector3 endPoint;
    Vector3 targetPoint;

    List<Vector3> waypoints;

    private GameObject player;
    private Vector3 offset;


    private void Start()
    {
        waypoints = new List<Vector3>();

        for(int i = 0; i < waypointsParent.childCount; i++) //Lists out waypoints before moving as to not move the waypoints with the platform
        {
            waypoints.Add(waypointsParent.GetChild(i).position);
        }

        if(waypoints.Count < 1)
        {
            Debug.LogError("Needs more than 1 waypoint");
            enabled = false;
        }

        currentPointIndex = 0;

        startPoint = GetWaypoint();
        GetNextWaypointIndex();
        targetPoint = GetWaypoint();

        transform.position = startPoint;
    }
    private void Update()
    {
        if (Vector3.Distance(transform.position, targetPoint) <= stoppingDistThreshold)
        {
            dwellTimer += Time.deltaTime;

            bool continueTrack = false;

            if(IsAtEndPoint())
            {
                if(loop && !stopAtEndingPointOnLoopTrack && (currentPointIndex == waypoints.Count -1))
                {
                    continueTrack = true;
                }

                if(dwellTimer >= endPointStall)
                {
                    continueTrack = true;
                }
            }
            else
            {
                if(dwellTimer >= midPointStall)
                {
                    continueTrack = true;
                }
            }

            if(continueTrack)
            {
                startPoint = targetPoint;
                GetNextWaypointIndex();
                targetPoint = GetWaypoint();

                t = 0f;
                dwellTimer = 0f;
            }

            return;
        }

        t += (Time.deltaTime / travelTime);

        transform.position = Vector3.Lerp(startPoint, targetPoint, t);

        if (player != null)
        {
            player.transform.position = transform.position + offset;
        }
    }

    private Vector3 GetWaypoint()
    {
        return (waypoints[currentPointIndex]);
    }

    private void GetNextWaypointIndex()
    {
        if(!loop)
        {
            if(movingForward)
            {
                currentPointIndex++;
                if(currentPointIndex >= waypoints.Count)
                {
                    currentPointIndex = waypoints.Count - 2;
                    movingForward = false; 
                }
            }
            else
            {
                currentPointIndex--;
                if(currentPointIndex < 0)
                {
                    currentPointIndex = 1;
                    movingForward = true;
                }
            }
        }
        else
        {
            currentPointIndex++;
            if(currentPointIndex >= waypoints.Count)
            {
                currentPointIndex = 0;
            }
        }
    }

    private bool IsAtEndPoint()
    {
        if(currentPointIndex == 0 || currentPointIndex == (waypoints.Count - 1))
        {
            return true;
        }

        return false;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            player = other.gameObject;  //saves players last transform, sets player's parent to this transform while adding player movement
            offset = player.transform.position - transform.position;
            player.transform.parent = transform.parent;
        }
    }

    /*private void FixedUpdate()
    {
        if (player != null)
        {
            player.transform.position = transform.position + offset;
        }
    }*/

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            player = null;
            player.transform.parent = null;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (waypointsParent == null) return;

        Gizmos.color = Color.white;

        for(int i=0; i < waypointsParent.childCount; i++)
        {
            Gizmos.DrawSphere(waypointsParent.GetChild(i).position, 0.5f);

            int nextWaypoint = (i + 1);
            if (nextWaypoint >= waypointsParent.childCount) break;

            Gizmos.DrawLine(waypointsParent.GetChild(i).position, waypointsParent.GetChild(nextWaypoint).position);
        }
    }
}
