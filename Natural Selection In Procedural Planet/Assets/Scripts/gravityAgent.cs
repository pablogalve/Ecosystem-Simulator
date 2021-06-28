using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravityAgent : MonoBehaviour
{
    public float distanceToCenter;
    public GameObject planet;

    // Start is called before the first frame update
    void Start()
    {
        planet = GameObject.FindWithTag("Planet");

        if (gameObject.tag == "Moon")
        {
            Rigidbody rb;
            rb = GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = new Vector3(0, 0, 30);
        }
    }

    // Update is called once per frame
    void Update()
    {
        distanceToCenter = GetDistance(gameObject, planet);

        if (distanceToCenter >= 1000.0f)
            entityManager.KillEntity(this.gameObject);
    }

    float GetDistance(GameObject obj1, GameObject obj2)
    {
        return Vector3.Distance(obj1.transform.position, obj2.transform.position);
    }
}
