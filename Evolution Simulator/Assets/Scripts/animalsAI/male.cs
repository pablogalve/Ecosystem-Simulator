using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class male : MonoBehaviour
{
    public DataHolder.Species specie = DataHolder.Species.UNDEFINED;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void RequestMate(GameObject female)
    {
        bool mateAccepted = false;

        if (female.GetComponent<female>() != null)
            mateAccepted = female.GetComponent<female>().MateRequestReceived(this.gameObject);
    }

    void OnCollisionEnter(Collision female)
    {
        var multiTag = female.gameObject.GetComponent<CustomTag>();

        if (multiTag == null)
            return;

        if (female.gameObject.GetComponent<female>() == null)
            return; //It's not a female

        if (female.gameObject.GetComponent<female>().specie == specie)
        {
            if (multiTag.HasTag("Female")) //No need 
            {
                RequestMate(female.gameObject);
            }
        }
    }
}
