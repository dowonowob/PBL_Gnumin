using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using System.Linq;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;

public class TourManager : MonoBehaviour
{
    public Transform arCamera;
    public ARRaycastManager raycastManager;

    public Campus campus;
    private Vector2 gpsCoor;
    public bool isFirst = true;
    public bool isDistance = false;
    public double min;

    public Text textUI_1; // 건물 이름 표시하는 텍스트 오브젝트
    public Text textUI_2; // 남은 거리 표시하는 텍스트 오브젝트
    public Text textUI_3; // 건물 설명 표시하는 텍스트 오브젝트

    public Button classInfobtn;

    public Text gpsUI;
    public Text infoUI;
    public Text campusUI;

    public GameObject panel;

    public Image img; // 건물 사진을 표시하는 이미지 오브젝트
    public GameObject canvas; // 정보를 표시하는 캔버스 오브젝트
    public GameObject placeObject; // 하트 --> 지누로 변경

    public Transform placeRoot; // 생성될 오브젝트의 부모 위치
    public GameObject defaultGinuPrefab; // 기본 프리팹 (미지정 시 대체용)
    private GameObject currentPlacedObject = null;


    IEnumerator Start() // 스마트폰 GPS 받아오는 코드
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
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);


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


                List<double> distanceList = new List<double>(); // 건물들과의 거리를 저장할 리스트 생성

                for (int i = 0; i < campus.Sheet1.Count; i++)
                {

                    float buildingLat = campus.Sheet1[i].latitude; // 각 건물 위도를 엑셀에서 가져옴
                    float buildingLong = campus.Sheet1[i].longitude; // 각 건물 경도 엑셀에서 가져옴

                    double myLat = Input.location.lastData.latitude;
                    double myLong = Input.location.lastData.longitude;


                    double remainDistance = distance(myLat, myLong, (double)buildingLat, (double)buildingLong);
                    // 플레이어와 건물과의 거리를 계산

                    // 리스트에 거리 추가
                    distanceList.Add(remainDistance);
                }

                min = distanceList.Min(); // 리스트에서 가장 가까운(=작은) 거리를 min에 저장
                int minIndex = distanceList.IndexOf(min); // min의 인덱스를 minIndex에 저장

                for (int i = 0; i < campus.Sheet1.Count; i++)
                {
                    if (i == minIndex)
                    {
                        if (min <= 50f)
                        {
                            if (campus.Sheet1[i].campusName == "가좌")
                            {
                                switch (campus.Sheet1[i].buildingNumber)
                                {
                                    case "601":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("601"));
                                        break;
                                    case "201":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("201"));
                                        break;
                                    case "401":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("401"));
                                        break;
                                    case "402":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("402"));
                                        break;
                                    case "403":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("403"));
                                        break;
                                    case "404":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("404"));
                                        break;
                                    case "405":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("405"));
                                        break;
                                    case "406":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("406"));
                                        break;
                                    case "407":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("407"));
                                        break;
                                    case "351":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("351"));
                                        break;
                                    case "352":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("352"));
                                        break;
                                    case "353":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("353"));
                                        break;
                                    case "354":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("354"));
                                        break;
                                    case "451":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("451"));
                                        break;
                                    case "452":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("452"));
                                        break;
                                    case "455":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("455"));
                                        break;
                                    case "456":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("456"));
                                        break;
                                    case "251":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("251"));
                                        break;
                                    case "151":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("151"));
                                        break;
                                    case "501":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("501"));
                                        break;
                                    case "28":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("28"));
                                        break;
                                    case "101":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("101"));
                                        break;
                                    case "102":
                                        classInfobtn.gameObject.SetActive(true);
                                        classInfobtn.onClick.RemoveAllListeners();
                                        classInfobtn.onClick.AddListener(() => ClassInfoChange("102"));
                                        break;
                                }
                            }

                            isDistance = true;
                            campusUI.text = campus.Sheet1[i].campusName + " 캠퍼스";
                            panel.SetActive(true);
                            infoUI.text = "근처 (" + (int)Math.Round(min) + "m) 이내에\n" + campus.Sheet1[i].buildingName + "이(가) 있습니다.\n\n * 장소 정보를 확인하려면 하트를 눌러주세요. *";

                            if (isFirst)
                            {
                                updateCenterObject(); // 건물과의 거리가 50m이내면 캔버스 활성화
                            }

                            string displayNumber = " - ";

                            if (int.TryParse(campus.Sheet1[i].buildingNumber, out int result))
                            {
                                displayNumber = campus.Sheet1[i].buildingNumber;
                            }
                            // "7-2"와 같은 특정 패턴은 그대로 사용
                            else if (campus.Sheet1[i].buildingNumber.Contains("-") && campus.Sheet1[i].buildingNumber.Split('-').Length == 2 && int.TryParse(campus.Sheet1[i].buildingNumber.Split('-')[0], out _) && int.TryParse(campus.Sheet1[i].buildingNumber.Split('-')[1], out _))
                            {
                                displayNumber = campus.Sheet1[i].buildingNumber;
                            }

                            // 최종 텍스트 설정
                            textUI_1.text = campus.Sheet1[i].buildingName + " (" + displayNumber + ")";
                            textUI_2.text = "까지 남은 거리: \n" + (int)Math.Round(min) + "m"; // 텍스트2에 남은 거리를 표시함

                            string spritePath = campus.Sheet1[i].pictureName; // 이미지 경로를 pictureName로 정함
                            img.sprite = Resources.Load<Sprite>(spritePath); // 현재 이미지를 spritePath 경로에 있는 이미지로 교체

                            textUI_3.text = campus.Sheet1[i].buildingDescription; // 텍스트3에 건물 설명을 표시함
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
                            canvas.gameObject.SetActive(false); // 건물을 벗어나면 캔버스 비활성화
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
            canvas.gameObject.SetActive(false);
        }
    }

    public void ClassInfoChange(string buildingNumber)
    {
        switch (buildingNumber)
        {
            case "601":
                SceneManager.LoadScene("Class601");
                break;
            case "201":
                SceneManager.LoadScene("Class201");
                break;
            case "401":
                SceneManager.LoadScene("Class401");
                break;
            case "402":
                SceneManager.LoadScene("Class402");
                break;
            case "403":
                SceneManager.LoadScene("Class403");
                break;
            case "404":
                SceneManager.LoadScene("Class404");
                break;
            case "405":
                SceneManager.LoadScene("Class405");
                break;
            case "406":
                SceneManager.LoadScene("Class406");
                break;
            case "407":
                SceneManager.LoadScene("Class407");
                break;
            case "351":
                SceneManager.LoadScene("Class351");
                break;
            case "352":
                SceneManager.LoadScene("Class352");
                break;
            case "353":
                SceneManager.LoadScene("Class353");
                break;
            case "354":
                SceneManager.LoadScene("Class354");
                break;
            case "451":
                SceneManager.LoadScene("Class451");
                break;
            case "452":
                SceneManager.LoadScene("Class452");
                break;
            case "455":
                SceneManager.LoadScene("Class455");
                break;
            case "456":
                SceneManager.LoadScene("Class456");
                break;
            case "251":
                SceneManager.LoadScene("Class251");
                break;
            case "151":
                SceneManager.LoadScene("Class151");
                break;
            case "501":
                SceneManager.LoadScene("Class501");
                break;
            case "28":
                SceneManager.LoadScene("Class28");
                break;
            case "101":
                SceneManager.LoadScene("Class101");
                break;
            case "102":
                SceneManager.LoadScene("Class102");
                break;

        }
    }

    private void updateCenterObject()
    {
        Vector3 screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        if (hits.Count == 0) return;

        Pose placePose = hits[0].pose;
        placePose.position.y = arCamera.position.y - 0.5f;
        isFirst = false;

        // 기존 오브젝트 삭제
        if (currentPlacedObject != null)
            Destroy(currentPlacedObject);

        // 가장 가까운 건물 정보 찾기
        double myLat = Input.location.lastData.latitude;
        double myLon = Input.location.lastData.longitude;

        double closestDistance = double.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < campus.Sheet1.Count; i++)
        {
            double dist = distance(myLat, myLon, campus.Sheet1[i].latitude, campus.Sheet1[i].longitude);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestIndex = i;
            }
        }

        if (closestIndex == -1) return;

        // ginuCharacter 프리팹 로드
        string prefabPath = campus.Sheet1[closestIndex].ginuCharacter; // 예: "Characters/EngineerGinu"
        GameObject prefab = Resources.Load<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogWarning($"프리팹 {prefabPath} 를 Resources에서 찾을 수 없습니다. 기본 프리팹으로 대체합니다.");
            prefab = defaultGinuPrefab;
        }

        // 오브젝트 생성
        currentPlacedObject = Instantiate(prefab, placePose.position, placePose.rotation, placeRoot);
        currentPlacedObject.transform.LookAt(arCamera);
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

            canvas.gameObject.transform.SetPositionAndRotation(placePose.position, placePose.rotation);
            canvas.gameObject.transform.LookAt(arCamera);
        }
    }

    // 아래 코드는 건물과의 남은 거리를 계산하는 코드
    private double distance(double lat1, double lon1, double lat2, double lon2)
    {
        double theta = lon1 - lon2;
        double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));
        dist = Math.Acos(dist);
        dist = Rad2Deg(dist);
        dist = dist * 60 * 1.1515;
        dist = dist * 1609.344;
        return dist;
    }
    private double Deg2Rad(double deg)
    {
        return (deg * Mathf.PI / 180.0f);
    }
    private double Rad2Deg(double rad)
    {
        return (rad * 180.0f / Mathf.PI);
    }
}