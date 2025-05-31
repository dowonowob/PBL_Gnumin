using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class TourManager : MonoBehaviour
{
    public Transform arCamera;
    public ARRaycastManager raycastManager;

    public Campus campus;
    private Vector2 gpsCoor;
    public bool isFirst = true;
    public bool isDistance = false;
    public double min;

    public TextMeshProUGUI textUI_1; // 건물 이름 표시
    public TextMeshProUGUI textUI_2; // 남은 거리 표시
    public TextMeshProUGUI textUI_3; // 건물 설명 표시

    public Button classInfobtn;

    public TextMeshProUGUI gpsUI;
    public TextMeshProUGUI infoUI;
    public TextMeshProUGUI campusUI;

    public GameObject panel;
    public Image img;
    public GameObject canvas;
    public GameObject placeObject;
    public Transform placeRoot;
    public GameObject defaultGinuPrefab;

    IEnumerator Start()
    {
        min = -1;
        classInfobtn.gameObject.SetActive(false);
        panel.SetActive(false);

        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            yield return null;
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        if (!Input.location.isEnabledByUser)
            yield break;

        Input.location.Start(10, 1);

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude);
            while (true)
            {
                yield return null;
            }
        }
    }

    void Update()
    {
        if (Input.location.isEnabledByUser)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                gpsUI.text = "<color=green>●</color> GPS 동작중";

                List<double> distanceList = new List<double>();

                for (int i = 0; i < campus.Sheet1.Count; i++)
                {
                    float buildingLat = campus.Sheet1[i].latitude;
                    float buildingLong = campus.Sheet1[i].longitude;

                    double myLat = Input.location.lastData.latitude;
                    double myLong = Input.location.lastData.longitude;

                    double remainDistance = distance(myLat, myLong, buildingLat, buildingLong);
                    distanceList.Add(remainDistance);
                }

                min = distanceList.Min();
                int minIndex = distanceList.IndexOf(min);

                for (int i = 0; i < campus.Sheet1.Count; i++)
                {
                    if (i == minIndex)
                    {
                        if (min <= 50f)
                        {
                            if (campus.Sheet1[i].campusName == "가좌")
                            {
                                string bNum = campus.Sheet1[i].buildingNumber;
                                classInfobtn.gameObject.SetActive(true);
                                classInfobtn.onClick.RemoveAllListeners();
                                classInfobtn.onClick.AddListener(() => ClassInfoChange(bNum));
                            }

                            isDistance = true;
                            campusUI.text = campus.Sheet1[i].campusName + " 캠퍼스";
                            panel.SetActive(true);
                            infoUI.text = $"근처 ({(int)Math.Round(min)}m) 이내에\n{campus.Sheet1[i].buildingName}이(가) 있습니다.\n\n * 장소 정보를 확인하려면 하트를 눌러주세요. *";

                            if (isFirst) updateCenterObject();

                            string displayNumber = campus.Sheet1[i].buildingNumber;
                            textUI_1.text = $"{campus.Sheet1[i].buildingName} ({displayNumber})";
                            textUI_2.text = $"까지 남은 거리:\n{(int)Math.Round(min)}m";

                            img.sprite = Resources.Load<Sprite>(campus.Sheet1[i].pictureName);
                            textUI_3.text = campus.Sheet1[i].buildingDescription;
                        }
                        else
                        {
                            panel.SetActive(false);
                            infoUI.text = "";
                            isDistance = false;
                            isFirst = true;
                            campusUI.text = "";
                            placeObject.SetActive(false);
                            classInfobtn.gameObject.SetActive(false);
                            canvas.SetActive(false);
                        }
                    }
                }

                distanceList.Clear();
            }
            else if (Input.location.status == LocationServiceStatus.Stopped || Input.location.status == LocationServiceStatus.Failed)
            {
                Input.location.Start(10, 1);
            }
        }
        else
        {
            min = -1;
            gpsUI.text = "<color=red>●</color> GPS 꺼짐";
            panel.SetActive(true);
            infoUI.text = "* GPS가 꺼져있습니다. GPS를 켜주세요.*";
            classInfobtn.gameObject.SetActive(false);
            isDistance = false;
            isFirst = true;
            campusUI.text = "";
            placeObject.SetActive(false);
            canvas.SetActive(false);
        }
    }

    public void ClassInfoChange(string buildingNumber)
    {
        SceneManager.LoadScene($"Class{buildingNumber}");
    }

    private void updateCenterObject()
    {
        Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        if (hits.Count > 0)
        {
            isFirst = false;
            placeObject.SetActive(true);
            Pose placePose = hits[0].pose;
            placePose.position.y = arCamera.position.y - 0.5f;
            placeObject.transform.SetPositionAndRotation(placePose.position, placePose.rotation);
            placeObject.transform.LookAt(arCamera);
        }
    }

    public void positionAdjustment()
    {
        Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        if (hits.Count > 0)
        {
            Pose placePose = hits[0].pose;
            placePose.position.y = arCamera.position.y - 0.5f;
            placeObject.transform.SetPositionAndRotation(placePose.position, placePose.rotation);
            placeObject.transform.LookAt(arCamera);
        }
    }

    public void positionAdjustmentCanvas()
    {
        Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        if (hits.Count > 0)
        {
            Pose placePose = hits[0].pose;
            placePose.position.y = arCamera.position.y - 0.5f;
            canvas.transform.SetPositionAndRotation(placePose.position, placePose.rotation);
            canvas.transform.LookAt(arCamera);
        }
    }

    private double distance(double lat1, double lon1, double lat2, double lon2)
    {
        double theta = lon1 - lon2;
        double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) +
                      Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));
        dist = Math.Acos(dist);
        dist = Rad2Deg(dist);
        dist = dist * 60 * 1.1515 * 1609.344;
        return dist;
    }

    private double Deg2Rad(double deg) => deg * Mathf.PI / 180.0f;
    private double Rad2Deg(double rad) => rad * 180.0f / Mathf.PI;
}
