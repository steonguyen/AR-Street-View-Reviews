using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using UnityEngine.Networking;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine.UI;
using Google.XR.ARCoreExtensions.Samples.Geospatial;

public class DetailsPopUp : MonoBehaviour
{

    public DetailsResponse placeDetails;

    public Text DisplayName;

    public Text OpeningHours;  
    public Text EditorialSummary;
    public Text FormattedAddress;
    public Text NationalPhoneNumber;
    public Text WebsiteUri;
    public Text Rating;
    public Text UserRatingCount;
    public Text OpenStatus;
    

    public Text Reviews;

    private string websitelink;
    private string place_idd;

    public GeospatialController controller;
    public GameObject ARTouch;


    void Start()
    {

    }

    // on-click method to show details page
    public void showDetails(string place_id)
    {
        place_idd = place_id;
        StartCoroutine(performDetailsSearch(place_id));
    }


    // send request to Google Places API
    IEnumerator performDetailsSearch(string place_id)
    {
        
        string url = createDetailsRequest(place_id);

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
                Debug.Log(jsonResponse);
                placeDetails = JsonUtility.FromJson<DetailsResponse>(jsonResponse);
                changeDetails();
            }
        }
    }


    // creates URL for Google Places API request
    private string createDetailsRequest(string place_id)
    {

        string APIkey = "AIzaSyBAyn3y0awsDMe0x7oUZ9i_iiu61lDWCR0";

        string fields = "displayName,primaryTypeDisplayName,editorialSummary,currentOpeningHours,formattedAddress,nationalPhoneNumber,websiteUri,googleMapsUri,priceLevel,rating,userRatingCount,reviews";


        string url = "https://places.googleapis.com/v1/places/"
            + place_id + "?"
            + "fields=" + fields
            + "&key=" + APIkey;

        return url;
    }

    private void changeDetails()
    {

        DisplayName.text = "";
        EditorialSummary.text = "";
        FormattedAddress.text = "";
        NationalPhoneNumber.text = "";
        WebsiteUri.text = "";
        Rating.text = "0.0";
        UserRatingCount.text = "(0 ratings)";
        OpeningHours.text = "";
        OpenStatus.text = "Closed";
        Reviews.text = "";


        if (placeDetails.displayName.text != null)
        {
            DisplayName.text = placeDetails.displayName.text;
        }

        if(placeDetails.editorialSummary.text != null)
        {
            EditorialSummary.text = placeDetails.editorialSummary.text;
        } else
        {
            EditorialSummary.text = placeDetails.primaryTypeDisplayName.text;
        }

        if(placeDetails.formattedAddress != null)
        {
            string dir_text = placeDetails.formattedAddress;
            int index = dir_text.IndexOf(",");

            if (index != -1)
            {
                string dir_text1 = dir_text.Substring(0, index + 1);
                string dir_text2 = dir_text.Substring(index + 2);
                dir_text = dir_text1 + "\n" + dir_text2;
            }

            FormattedAddress.text = dir_text;
        }

        if (placeDetails.nationalPhoneNumber != null)
        {
            NationalPhoneNumber.text = placeDetails.nationalPhoneNumber;
        }

        if (placeDetails.websiteUri != null)
        {
            websitelink = placeDetails.websiteUri;

            string websitetext = websitelink.Replace("https://", "");
            websitetext = websitetext.Replace("http://", "");

            string[] website_split = websitetext.Split(new string[] { "/" }, StringSplitOptions.None);

            WebsiteUri.text = website_split[0];
        }

        if (placeDetails.rating != null)
        {
            Rating.text = placeDetails.rating.ToString("F1");
        }

        if (placeDetails.userRatingCount != null)
        {
            UserRatingCount.text = "(" + placeDetails.userRatingCount.ToString() + " ratings)";
        }

        if (placeDetails.currentOpeningHours.weekdayDescriptions != null)
        {
            string weeklyHours = "";

            foreach (string dayHours in placeDetails.currentOpeningHours.weekdayDescriptions)
            {
                weeklyHours = weeklyHours + dayHours + "\n";
            }


            string[] resultie = weeklyHours.Split(new string[] { "y:" }, StringSplitOptions.None);

            string hoursOutput = resultie[0] + "y:\t\t" + resultie[1] + "y:\t\t" + resultie[2] + "y:" + resultie[3] + "y:\t" + resultie[4] + "y:\t\t\t" + resultie[5] + "y:\t\t" + resultie[6] + "y:\t\t" + resultie[7];

            OpeningHours.text = hoursOutput;
        }

        if (placeDetails.currentOpeningHours.openNow)
        {
            OpenStatus.text = "Open";
        }
        else
        {
            OpenStatus.text = "Closed";
        }

        if(placeDetails.reviews != null)
        {
            string review_text = "";

            foreach (DetailsResponse.Review reviewObj in placeDetails.reviews)
            {
                review_text = review_text + reviewObj.authorAttribution.displayName + " [" + reviewObj.rating.ToString() + "]\n";
                review_text = review_text + reviewObj.text.text + "\n\n\n";
            }

            Reviews.text = review_text;
        }



        //GoogleMapsUri.text = placeDetails.googleMapsUri;
    }


    public void closeTab()
    {
        controller.SwitchToARView(true);
        ARTouch.SetActive(true);
    }

    public void goToWebsite()
    {
        Application.OpenURL(placeDetails.websiteUri);
    }

    public void goToGooglePage()
    {
        Application.OpenURL(placeDetails.googleMapsUri);
    }

    public void goToMaps()
    {
        string mapsurl = "https://www.google.com/maps/dir/?api=1&destination=" + placeDetails.formattedAddress;
        Application.OpenURL(mapsurl);
    }

    public void goToReviews()
    {
        string googleReviewsURL = "https://www.google.com/maps/place/?q=place_id:" + place_idd + "&hl=en&view=lr";
        Application.OpenURL(googleReviewsURL);
    }
}
