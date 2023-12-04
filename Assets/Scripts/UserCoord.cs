using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserCoord : MonoBehaviour
{
    private double latitude;
    private double longitude;
    // Start is called before the first frame update
    IEnumerator Start()
    {

        DontDestroyOnLoad(gameObject);

        if (!Input.location.isEnabledByUser)
        {
            // Location service is not enabled; request permission from the user.
            //Debug.LogWarning("Location services are not enabled");
        }

        Input.location.Start();


        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogWarning("Location service failed to initialize.");
        }

    }

    // Update is called once per frame
    void Update()
    {
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
    }

    public double getLatitude()
    {
        return latitude;
    }

    public double getLongitude()
    {
        return longitude;
    }

}
