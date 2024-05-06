using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexSelect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100000f))
        {
            Debug.Log(hit.transform.name);
            Debug.Log("hit");
        }
    }
}
