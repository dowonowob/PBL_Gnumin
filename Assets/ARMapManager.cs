using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using System;

public class ARMapManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text locationText;
    public Text fidText;

    [Header("CSV & GPS Center")]
    public string csvFileName = "GNU_MIDDLE_GPS_OB";
    private double mapCenterLat = 35.154980;
    private double mapCenterLon = 128.099211;

    [Header("Zone Object Settings")]
    public GameObject infoPanel;
    public Text titleText;
    public Text descriptionText;

    private List<ZoneInfo> zones = new List<ZoneInfo>();
    private GameObject currentObject;

    void Start()
    {
        LoadZoneData();
        RequestLocationPermission();
    }

    private void LoadZoneData()
    {
        TextAsset csvData = Resources.Load<TextAsset>(csvFileName);

        if (csvData != null)
        {
            string[] lines = csvData.text.Split('\n');
            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(',');
                if (values.Length < 6) continue;

                int fid = int.Parse(values[0]);
                double lat = double.Parse(values[1]);
                double lon = double.Parse(values[2]);
                float radius = float.Parse(values[3]);
                string prefabName = values[4];
                string desc = values[5];

                var prefab = Resources.Load<GameObject>("GNU_Prefabs/Characters/" + prefabName);
                if (prefab != null)
                {
                    zones.Add(new ZoneInfo(fid, lat, lon, radius, prefab, prefabName, desc));
                }
            }
        }
        else
        {
            Debug.LogError("CSV not found in Resources: " + csvFileName);
        }
    }

    private IEnumerator MonitorZones()
    {
        while (true)
        {
            double currentLat = Input.location.lastData.latitude;
            double currentLon = Input.location.lastData.longitude;

            foreach (var zone in zones)
            {
                float dist = Haversine(currentLat, currentLon, zone.latitude, zone.longitude);

                if (dist <= zone.radius && (currentObject == null || currentObject.name != zone.prefabName))
                {
                    if (currentObject != null) Destroy(currentObject);
                    Vector3 worldPos = GetWorldPosition(zone.latitude, zone.longitude);
                    currentObject = Instantiate(zone.prefab, worldPos, Quaternion.identity);

                    var info = currentObject.GetComponent<ObjectInfo>();
                    if (info != null)
                    {
                        info.title = zone.prefabName;
                        info.description = zone.description;
                    }
                }
            }

            yield return new WaitForSeconds(1);
        }
    }

    private Vector3 GetWorldPosition(double lat, double lon)
    {
        float x = (float)(lon - mapCenterLon) * 100000f;
        float z = (float)(lat - mapCenterLat) * 100000f;
        return new Vector3(x, 0, z);
    }

    private float Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371000; // meters
        double dLat = (lat2 - lat1) * Mathf.Deg2Rad;
        double dLon = (lon2 - lon1) * Mathf.Deg2Rad;

        double phi1 = lat1 * Mathf.Deg2Rad;
        double phi2 = lat2 * Mathf.Deg2Rad;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(phi1) * Math.Cos(phi2) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return (float)(R * c);
    }

    private void RequestLocationPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            StartCoroutine(RequestLocation());
        }
    }

    private IEnumerator RequestLocation()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("GPS 사용 불가");
            yield break;
        }

        Input.location.Start();
        int maxWait = 20;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.Log("위치 서비스 시작 실패");
            yield break;
        }

        StartCoroutine(MonitorZones());

        while (Input.location.status == LocationServiceStatus.Running)
        {
            double currentLat = Input.location.lastData.latitude;
            double currentLon = Input.location.lastData.longitude;

            locationText.text = $"현재 위치: {currentLat}, {currentLon}";
            yield return new WaitForSeconds(1);
        }
    }

    [System.Serializable]
    public class ZoneInfo
    {
        public int fid;
        public double latitude;
        public double longitude;
        public float radius;
        public GameObject prefab;
        public string prefabName;
        public string description;

        public ZoneInfo(int fid, double lat, double lon, float radius, GameObject prefab, string name, string desc)
        {
            this.fid = fid;
            this.latitude = lat;
            this.longitude = lon;
            this.radius = radius;
            this.prefab = prefab;
            this.prefabName = name;
            this.description = desc;
        }
    }

    void OnDisable()
    {
        Input.location.Stop();
    }
}
