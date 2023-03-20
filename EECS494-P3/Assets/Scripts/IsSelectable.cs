using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class IsSelectable : MonoBehaviour
{
    [SerializeField] private float bobDistance = 0.5f;
    [SerializeField] GameObject itemDescription;
    private Vector3 origin;
    private Vector3 destination;
    
    
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
        destination = new Vector3(origin.x, origin.x + 1, origin.z);
        

    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(BobUp());
        }
                
    
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(BobDown());
        }
    }

    private IEnumerator BobUp()
    {
        itemDescription.SetActive(true);
        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime);
            yield return null;
        }

        transform.position = destination;
    }
    
    private IEnumerator BobDown()
    {
        itemDescription.SetActive(false);
        while (Vector3.Distance(transform.position, origin) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, origin, Time.deltaTime);
            yield return null;
        }
        transform.position = origin;
    }
}
