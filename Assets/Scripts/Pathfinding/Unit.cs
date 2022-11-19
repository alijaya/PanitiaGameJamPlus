using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour// , IPointerClickHandler
{
    public Transform target;
    public float speed = 20;

    Vector2[] path;
    int targetIndex;

    void Start() {
        StartCoroutine (RefreshPath ());
    }

    void Update() {
        //Check for mouse click 
         if (Input.GetMouseButtonDown(0))
         {
            print("t");
            GameObject hitObject = null;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D raycastHit = Physics2D.Raycast(ray.origin, Vector2.zero, Mathf.Infinity);
            if (raycastHit != null && raycastHit.collider != null) {
                print("s");
                if(raycastHit.transform.gameObject.tag=="Target")
                     {  
                        print("c");
                        hitObject = raycastHit.collider.gameObject;
                        target = hitObject.transform;
                     }
                
            }
         }
    }

    IEnumerator RefreshPath() {
        Vector2 targetPositionOld = (Vector2)target.position + Vector2.up; // ensure != to target.position initially
            
        while (true) {
            if (targetPositionOld != (Vector2)target.position) {
                targetPositionOld = (Vector2)target.position;

                path = Pathfinding.RequestPath (transform.position, target.position);
                StopCoroutine ("FollowPath");
                StartCoroutine ("FollowPath");
            }

            yield return new WaitForSeconds (.25f);
        }
    }
        
    IEnumerator FollowPath() {
        if (path.Length > 0) {
            targetIndex = 0;
            Vector2 currentWaypoint = path [0];

            while (true) {
                if ((Vector2)transform.position == currentWaypoint) {
                    targetIndex++;
                    if (targetIndex >= path.Length) {
                        yield break;
                    }
                    currentWaypoint = path [targetIndex];
                }

                transform.position = Vector2.MoveTowards (transform.position, currentWaypoint, speed * Time.deltaTime);
                yield return null;

            }
        }
    }

    public void OnDrawGizmos() {
        if (path != null) {
            for (int i = targetIndex; i < path.Length; i ++) {
                Gizmos.color = Color.black;
                //Gizmos.DrawCube((Vector3)path[i], Vector3.one *.5f);

                if (i == targetIndex) {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else {
                    Gizmos.DrawLine(path[i-1],path[i]);
                }
            }
        }
    }
}
