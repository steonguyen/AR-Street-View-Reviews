using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions.GeospatialCreator.Internal;
using UnityEngine.EventSystems;
using Google.XR.ARCoreExtensions.Samples.Geospatial;

public class SpawnerScript : MonoBehaviour
{
    public GameObject schoolPrefab;
    public GameObject foodPrefab;
    public GameObject cubePrefab;

    public NearbySearch nearbySearch;

    private List<GameObject> instantiatedObjects = new List<GameObject>();
    private int previousCount = 0; // Move this variable outside of the method

    // Start is called before the first frame update
    void Start()
    {
        nearbySearch = FindObjectOfType<NearbySearch>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the filteredPlaceList is updated
        if (IsFilteredPlacesUpdated())
        {
            GenerateObjectsFromFilteredPlaces();
        }
    }

    // Method to check if the filteredPlaceList is updated
    bool IsFilteredPlacesUpdated()
    {
        int currentCount = nearbySearch.filteredPlaceList != null ? nearbySearch.filteredPlaceList.Count : 0;

        // Compare the current count with the previous count
        if (currentCount != previousCount)
        {
            // Update the previous count for the next check
            previousCount = currentCount;

            // List is updated
            return true;
        }

        // List is not updated
        return false;
    }

    // Method to generate objects from the filteredPlaceList
    void GenerateObjectsFromFilteredPlaces()
    {
        // Access the filteredPlaceList from the NearbySearch script
        List<(string, string, Vector3)> filteredPlaces = nearbySearch.filteredPlaceList;

        // Check if the list is not empty
        if (filteredPlaces != null && filteredPlaces.Count > 0)
        {
            for(int i = 0; i < filteredPlaces.Count; i++)
            {
                // Check if an object is already instantiated at this position
                if (!IsPositionOccupied(filteredPlaces[i].Item3))
                {
                    if (filteredPlaces[i].Item2 == "food")
                    {
                        GameObject newObject = Instantiate(foodPrefab, filteredPlaces[i].Item3, Quaternion.identity, null);
                        newObject.name = filteredPlaces[i].Item1;
                        ARGeospatialCreatorAnchor anch = newObject.GetComponent<ARGeospatialCreatorAnchor>();
                        if (anch != null)
                        {
                            anch.Longitude = filteredPlaces[i].Item3.x;
                            anch.Latitude = filteredPlaces[i].Item3.z;
                        }

                        instantiatedObjects.Add(newObject);
                    }
                    else if (filteredPlaces[i].Item2 == "school")
                    {
                        GameObject newObject = Instantiate(schoolPrefab, filteredPlaces[i].Item3, Quaternion.identity, null);
                        newObject.name = filteredPlaces[i].Item1;
                        ARGeospatialCreatorAnchor anch = newObject.GetComponent<ARGeospatialCreatorAnchor>();
                        if (anch != null)
                        {
                            anch.Longitude = filteredPlaces[i].Item3.x;
                            anch.Latitude = filteredPlaces[i].Item3.z;
                        }

                        instantiatedObjects.Add(newObject);
                    }
                    else
                    {
                        GameObject newObject = Instantiate(cubePrefab, filteredPlaces[i].Item3, Quaternion.identity, null);
                        newObject.name = filteredPlaces[i].Item1;
                        ARGeospatialCreatorAnchor anch = newObject.GetComponent<ARGeospatialCreatorAnchor>();
                        if (anch != null)
                        {
                            anch.Longitude = filteredPlaces[i].Item3.x;
                            anch.Latitude = filteredPlaces[i].Item3.z;
                        }

                        instantiatedObjects.Add(newObject);
                    }

                }
            }
            Debug.Log("filteredPlaces is not empty");
        }
        else
        {
            Debug.Log("filteredPlaces is empty");
        }
    }

    // Method to check if a position is already occupied by an instantiated object
    bool IsPositionOccupied(Vector3 position)
    {
        foreach (GameObject obj in instantiatedObjects)
        {
            if (obj != null && obj.transform.position == position)
            {
                return true;
            }
        }
        return false;
    }
}