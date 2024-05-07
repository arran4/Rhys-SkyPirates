using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexSelect : MonoBehaviour
{
    public GameObject HighLightSelect { get; private set; } = null;
    public Tile CSTile;
    public Material BaseMat, HighlightMat;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //bellow is clunky, refactor to use events?
        if (Physics.Raycast(ray, out hit, 100000f))
        {
            if (HighLightSelect != hit.transform.gameObject)
            {
                if(HighLightSelect != null)
                {
                    HighLightSelect.GetComponent<MeshRenderer>().material = BaseMat;
                }
                HighLightSelect = hit.transform.gameObject;
                HighLightSelect.GetComponent<MeshRenderer>().material = HighlightMat;
            }
            //for clarity in testing, remove later.
            Debug.Log(hit.transform.name);
            Debug.Log("hit");
        }
    }
}
