using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.XR.ARCoreExtensions.Samples.Geospatial;

public class ClickScript : MonoBehaviour
{
    public GeospatialController controller;
    public DetailsPopUp details;
    public GameObject detailsVisual;
    public GameObject ARTouch;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                string name = hit.collider.gameObject.name;
                //controller.OnPlaceClicked();
                StartCoroutine(performClickAction(name));
            }
        }
    }

    IEnumerator performClickAction(string name)
    {
        yield return StartCoroutine(detailSearch(name));
        //yield return new WaitForSeconds(0.3f);
        detailsVisual.SetActive(true);
        ARTouch.SetActive(false);
    }

    IEnumerator detailSearch(string name)
    {
        details.showDetails(name);
        yield return new WaitForSeconds(0.2f);
    }
}
