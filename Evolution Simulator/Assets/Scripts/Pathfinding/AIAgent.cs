using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgent : MonoBehaviour
{
    private List<Vector3Int> last_path = new List<Vector3Int>();
    public float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        int[] map = null;
        Pathfinding.CreateWalkabilityMap(10, 10, 10, ref map);
        Pathfinding.SetMap(10, 10, 10, ref map);
        //Pathfinding.CreateTestGrid(10, 10, 10);
        CalculatePath();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        //CalculatePath();
    }

    void CalculatePath()
    {
        Vector3 origin = transform.position; //TODO: Those are positions
        Vector3 destination = new Vector3(2.0f, 2.0f, 2.0f); //TODO: Those are positions

        Pathfinding.CreatePath(origin, destination, ref last_path);
    }

    void Move()
    {
        // Move our position a step closer to the target.
        float step = speed * Time.deltaTime; // calculate distance to move
        if(last_path.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, Pathfinding.indicesToPosition(last_path[0]), step);

            // Entity arrived to first tile, so we send the order to move to the next one
            if (Pathfinding.positionToIndices(transform.position) == last_path[0])
            {
                last_path.RemoveAt(0);
            }
        }            
    }
}
