using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEngine.Networking;
using System.Net.Http;
using System.Threading.Tasks;



public class NearbySearch : MonoBehaviour
{

    public UserCoord userCoords;

    public List<PlaceObject> placeList = new List<PlaceObject>();

    public List<(string, string, Vector3)> filteredPlaceList = new List<(string, string, Vector3)>();

    private string next_token;



    // Start is called before the first frame update
    void Start()
    {
        //searchPlaces();
    }

    private float timer = 0.0f;
    private float interval = 5.0f; // 5 seconds

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        // Check if the timer has reached the interval
        if (timer >= interval)
        {
            // Call your method here
            filterList();

            // Reset the timer
            timer = 0.0f;
        }
    }

    /*  --------------------------------------------------------------------------------------------------------------------------------------------------------------------------
     * 
     *      THIS FIRST SECTION IS WHERE WE MAKE A GOOGLE PLACES NEARBY SEARCH REQUEST AND PARSE THE RESPONSE INTO A LIST OF PLACE OBJECTS
     * 
     *  --------------------------------------------------------------------------------------------------------------------------------------------------------------------------
     */

    public void searchPlaces()
    {
        placeList.Clear();
        next_token = null;

        StartCoroutine(PerformNearbySearch(createURL()));
    }


    IEnumerator PerformNearbySearch(string url)
    {

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                // Parse and process the JSON response               
                string jsonResponse = webRequest.downloadHandler.text;
                yield return new WaitForSeconds(2.0f);
                ProcessJSONResponse(jsonResponse);

            }
        }

    }

    [System.Serializable]
    public class PlaceResult
    {
        public string name;
        public string place_id;
        public List<string> types;
        public Geometry geometry;
    }

    [System.Serializable]
    public class Geometry
    {
        public Location location;
    }

    [System.Serializable]
    public class Location
    {
        public float lat;
        public float lng;
    }

    [System.Serializable]
    public class PlacesResponse
    {
        public List<PlaceResult> results;
        public string next_page_token;
    }

    public class PlaceObject
    {
        public string Name;
        public string Place_id;
        public string Type;
        public float Latitude;
        public float Longitude;

        public PlaceObject(string name, string place_id, string type, float latitude, float longitude)
        {
            Name = name;
            Place_id = place_id;
            Type = type;
            Latitude = latitude;
            Longitude = longitude;
        }
    }

    void ProcessJSONResponse(string jsonString)
    {
        PlacesResponse placesResponse = JsonUtility.FromJson<PlacesResponse>(jsonString);

        foreach (PlaceResult result in placesResponse.results)
        {
            string name = result.name;
            string type;
            string place_id = result.place_id;
            List<string> types = result.types;
            float latitude = result.geometry.location.lat;
            float longitude = result.geometry.location.lng;
            //List<string> types = result.

            List<string> food_words = new List<string>
                {
                    "bar",
                    "restaurant",
                    "food",
                    "cafe"
                };

            bool found_food = false;
            foreach (string food_tag in food_words)
            {
                if(types.Contains(food_tag))
                {
                    found_food = true;
                    break;
                }
            }

            List<string> school_words = new List<string>
                {
                    "book_store",
                    "library",
                    "primary_school",
                    "secondary_school",
                    "university",
                    "school"
                };

            bool found_school = false;
            foreach (string school_tag in school_words)
            {
                if (types.Contains(school_tag))
                {
                    found_school = true;
                    break;
                }
            }



            if (found_food)
            {
                 type = "food";
            }
            else if(found_school)
            {
                type = "school";
            }
            else{
                type = "none";
            }

            PlaceObject placeItem = new PlaceObject(name, place_id, type, latitude, longitude);
            placeList.Add(placeItem);
        }

        // if there are more results, call for more
        next_token = placesResponse.next_page_token;
        if (next_token != null)
        {
            StartCoroutine(PerformNearbySearch(nextPageURL(next_token)));
        }

    }

    // create URL for Google Places API search
    private string createURL()
    {
        string APIkey = "AIzaSyBAyn3y0awsDMe0x7oUZ9i_iiu61lDWCR0";

        string latitude = userCoords.getLatitude().ToString("F6");
        string longitude = userCoords.getLongitude().ToString("F6");
        //string latitude = "30.337106";
        //string longitude = "-97.717723";

        string radius = "200";

        string url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?"
            + "location=" + latitude + "," + longitude
            + "&radius=" + radius
            + "&key=" + APIkey;


        return url;
    }

    private string nextPageURL(string token)
    {
        string APIkey = "AIzaSyBAyn3y0awsDMe0x7oUZ9i_iiu61lDWCR0";

        string url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?"
            + "pagetoken=" + token
            + "&key=" + APIkey;

        return url;
    }


    /*  --------------------------------------------------------------------------------------------------------------------------------------------------------------------------
     * 
     *      THIS SECOND SECTION IS WHERE WE FILTER THE LIST OF PLACE OBJECTS TO A VICINITY TO DISPLAY ON THE AR CAMERA VIEW
     * 
     *  --------------------------------------------------------------------------------------------------------------------------------------------------------------------------
     */



    private void filterList()
    {
        filteredPlaceList.Clear();

        PlaceObject user = new PlaceObject("AppUser", "AppUser", "none", (float) userCoords.getLatitude(), (float) userCoords.getLongitude());

        //double hardcode_lat = 30.337106;
        //double hardcode_long = -97.717723;
        //PlaceObject user = new PlaceObject("AppUser", "AppUser", (float)hardcode_lat, (float)hardcode_long);

        float maxDistance = 100;


        foreach (PlaceObject place in placeList)
        {
            double distance = CalculateDistance(user, place);

            if (distance <= maxDistance)
            {
                Vector3 locationVector = new Vector3(place.Longitude, 0f, place.Latitude);

                filteredPlaceList.Add((place.Place_id, place.Type, locationVector));

            }
        }


    }

    public static double CalculateDistance(PlaceObject user, PlaceObject placeItem)
    {
        // Radius of the Earth in meters
        double earthRadius = 6371000;

        double lat1 = user.Latitude * Math.PI / 180;
        double lon1 = user.Longitude * Math.PI / 180;
        double lat2 = placeItem.Latitude * Math.PI / 180;
        double lon2 = placeItem.Longitude * Math.PI / 180;

        double dLat = lat2 - lat1;
        double dLon = lon2 - lon1;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1) * Math.Cos(lat2) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double distance = earthRadius * c;
        return distance;
    }


}
