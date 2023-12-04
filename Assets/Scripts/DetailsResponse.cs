using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DetailsResponse
{
    public LocalizedText displayName; // displayName.text
    public LocalizedText primaryTypeDisplayName; // primaryTypeDisplayName.text
    public LocalizedText editorialSummary;

    public OpeningHours currentOpeningHours;

    public string formattedAddress;
    public string nationalPhoneNumber;
    public string websiteUri;
    public string googleMapsUri;

    public double rating;
    public int userRatingCount;
    public List<Review> reviews;

    [System.Serializable]
    public class LocalizedText
    {
        public string text;
        public string languageCode;
    }

    [System.Serializable]
    public class OpeningHours
    {
        public List<string> weekdayDescriptions;
        public bool openNow;
    }

    [System.Serializable]
    public class AuthorAttribution
    {
        public string displayName, uri, photouri;
    }

    [System.Serializable]
    public class Review
    {
        public LocalizedText text;
        public double rating;
        public string relativePublishTimeDescription;
        public AuthorAttribution authorAttribution;
        public string publishTime;
    }


}


