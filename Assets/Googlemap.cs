using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Googlemap : MonoBehaviour
{
    public RawImage mapRawImage;

    string url;

    public float lat = 37.480978f;
    public float lon = 126.8819525f;

    //LocationInfo li;

    public int zoom = 18;
    public int mapWidth = 300;
    public int mapHeight = 300;

    public enum mapType { roadmap, satellite, hybrid, terrain }
    public mapType mapSelected;
    public int scale;

    float monsterLat = 37.480978f;
    float monsterLon = 126.882637f;

    public GameObject monsterPrefab;

    static float UPDATE_TIME = 3f;
    float updateTime = UPDATE_TIME;

    IEnumerator Map()
    {
        url = "https://maps.googleapis.com/maps/api/staticmap?center=37.480978,126.882637&zoom=12&size=400x400&key=AIzaSyCtimkNwOp1CJkrF3VthJLmDtChLRaDmd8";
        //"https://maps.googleapis.com/maps/api/staticmap" + lat + "," + lon +
        //    "&zoom=" + zoom + "&size=" + mapWidth + "x" + mapHeight + "&scale=" + scale
        //    + "&maptype=" + mapSelected +
        //    "&key=AIzaSyCtimkNwOp1CJkrF3VthJLmDtChLRaDmd8" +
        //    "&markers=color:red%7Clabel:C%7C" + lat + "," + lon +
        //    "&markers=color:green%7Clabel:C%7C" + monsterLat + "," + monsterLon;

        WWW www = new WWW(url);
        yield return www;
        mapRawImage.texture = www.texture;
        //mapRawImage.SetNativeSize();

    }

    void Start()
    {
        StartCoroutine(StartGPS());

        mapRawImage = gameObject.GetComponent<RawImage>();
        StartCoroutine(Map());
    }

    private void Update()
    {
        if (updateTime >= 0)
        {
            updateTime = updateTime - Time.deltaTime;
        }
        else
        {
            updateTime = UPDATE_TIME;

            float latitude = Input.location.lastData.latitude;
            float longitude = Input.location.lastData.longitude;

            if (latitude != 0f && longitude != 0)
            {
                if (lat != latitude || lon != longitude)
                {
                    lat = latitude;
                    lon = longitude;

                    StartCoroutine(Map());

                    if (Mathf.Abs(lat - monsterLat) < 0.0001f && Mathf.Abs(lon - monsterLon) < 0.0001f)
                    {
                        GameObject monster = Instantiate(monsterPrefab);
                    }
                }
            }
        }
    }


    IEnumerator StartGPS()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stop service if there is no need to query location updates continuously
        //Input.location.Stop();
    }

    public void exitGame()
    {
        Application.Quit();
    }

    public void resetGPS()
    {
        lat = 37.480978f;
        lon = 126.882637f;
    }
}