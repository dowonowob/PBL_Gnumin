using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
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

    public Text textUI_1; //건물 이름 표시
    public Text textUI_2; //남은 거리 표시
    public Text textUI_3; //건물 설명 표시

    public Button classInfobtn;

    public Text gpsUI; 
    public Text infoUI;
    public Text campusUI;

    public GameObject panel; 
    public Image img; 
    public GameObject canvas;
    public GameObject placeObject;

    IEnumerator Start()
    {
        StartCoroutine(WaitForFirstPlane());

        canvas.SetActive(false);

        min = -1;
        classInfobtn.gameObject.SetActive(false); 
        panel.SetActive(false); 

        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation)) //위치 권한 요청
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
            if (Input.location.status == LocationServiceStatus.Running) //GPS 작동 중이면
            {
                gpsUI.text = "<color=green>●</color> GPS 동작중";

                List<double> distanceList = new List<double>();

                for (int i = 0; i < campus.Sheet1.Count; i++) //
                {
                    float buildingLat = campus.Sheet1[i].latitude;
                    float buildingLong = campus.Sheet1[i].longitude;

                    double myLat = Input.location.lastData.latitude;
                    double myLong = Input.location.lastData.longitude;

                    double remainDistance = distance(myLat, myLong, (double)buildingLat, (double)buildingLong);

                    distanceList.Add(remainDistance);
                }

                min = distanceList.Min();
                int minIndex = distanceList.IndexOf(min);

                for (int i = 0; i < campus.Sheet1.Count; i++)
                {
                    if (i == minIndex)
                    {
                        if (min <= 100f)
                        {
                            if (campus.Sheet1[i].campusName == "가좌")
                            {
                                isDistance = true;
                                campusUI.text = campus.Sheet1[i].campusName + " 캠퍼스";
                                panel.SetActive(true);
                                infoUI.text = $"근처 ({(int)Math.Round(min)}m) 이내에\n{campus.Sheet1[i].buildingName}이(가) 있습니다.\n\n * 장소 정보를 확인하려면 지누를 눌러주세요. *";

                                if (isFirst)
                                {
                                    updateCenterObject(); // 건물과의 거리가 50m이내면 캔버스 활성화
                                }

                                string displayNumber = campus.Sheet1[i].buildingNumber;
                                textUI_1.text = $"{campus.Sheet1[i].buildingName} ({displayNumber})";
                                textUI_2.text = $"까지 남은 거리:\n{(int)Math.Round(min)}m";

                                string rawName = campus.Sheet1[i].pictureName;
                                string trimmedName = rawName.Substring(rawName.IndexOf('_') + 1);  // "_" 이후만 추출

                                string path = "이미지_동번호/" + trimmedName;
                                Sprite sprite = Resources.Load<Sprite>(path);

                                if (sprite != null)
                                {
                                    img.sprite = sprite;
                                    img.color = Color.white;
                                }
                                else
                                {
                                    img.sprite = null;
                                    img.color = Color.black;
                                }

                                textUI_3.text = campus.Sheet1[i].buildingDescription;
                            }

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

    private IEnumerator WaitForFirstPlane()
    {
        // 평면이 감지될 때까지 대기
        while (FindObjectsOfType<ARPlane>().Length == 0)
        {
            yield return null;
        }
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
            canvas.gameObject.SetActive(false);

            Pose placePose = hits[0].pose;
            placePose.position.y = arCamera.position.y - 0.5f;
            placeObject.transform.position = placePose.position;

            // 수평 방향으로만 카메라 바라보기
            Vector3 lookDir = arCamera.position - placePose.position;
            lookDir.y = 0;

            if (lookDir != Vector3.zero)
            {
                placeObject.transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }
    //private void updateCenterObject(int minIndex)
    //{
    //    Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
    //    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    //    raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

    //    if (hits.Count > 0)
    //    {
    //        isFirst = false;

    //        // ginuCharacter 기반 프리팹 로드
    //        string ginuCharacter = campus.Sheet1[minIndex].ginuCharacter;
    //        string prefabPath = "Characters/" + ginuCharacter;
    //        GameObject ginuPrefab = Resources.Load<GameObject>(prefabPath);

    //        if (ginuPrefab != null)
    //        {
    //            if (placeObject != null) Destroy(placeObject);
    //            placeObject = Instantiate(ginuPrefab, Vector3.zero, Quaternion.identity);

    //        }
    //        else
    //        {
    //            Debug.LogWarning($"[TourManager] 캐릭터 프리팹 로드 실패: {prefabPath}");
    //            placeObject = Instantiate(ginuPrefab, Vector3.zero, Quaternion.identity);

    //        }

    //        placeObject.SetActive(true);

    //        // 위치 설정
    //        Pose placePose = hits[0].pose;
    //        placePose.position.y = arCamera.position.y - 1.5f;
    //        placeObject.transform.position = placePose.position;

    //        // 수평 방향으로 카메라 바라보도록
    //        Vector3 lookDir = arCamera.position - placePose.position;
    //        lookDir.y = 0;

    //        if (lookDir != Vector3.zero)
    //            placeObject.transform.rotation = Quaternion.LookRotation(lookDir);
    //    }
    //}

    public void positionAdjustment()
    {
        Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        if (hits.Count > 0)
        {
            Pose placePose = hits[0].pose;
            placePose.position.y = arCamera.position.y - 0.5f;
            placeObject.transform.position = placePose.position;

            // 수평 방향으로만 카메라 바라보기
            Vector3 lookDir = arCamera.position - placePose.position;
            lookDir.y = 0;

            if (lookDir != Vector3.zero)
            {
                placeObject.transform.rotation = Quaternion.LookRotation(lookDir);
            }
            float minDistance = 1.0f; // 최소 거리 (미터)

            Vector3 directionToCamera = arCamera.position - placePose.position;
            float distance = directionToCamera.magnitude;

            // 너무 가까우면 보정
            if (distance < minDistance)
            {
                Vector3 safeDirection = directionToCamera.normalized;
                placePose.position = arCamera.position - safeDirection * minDistance;
                placePose.position.y = arCamera.position.y - 1.5f; // 높이 재보정
            }
        }
    }

    //public void positionAdjustmentCanvas()
    //{
    //    Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
    //    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    //    raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

    //    if (hits.Count > 0)
    //    {
    //        Pose placePose = hits[0].pose;
    //        placePose.position.y = arCamera.position.y - 1.5f;
    //        placeObject.transform.position = placePose.position;

    //        Vector3 directionToCamera = arCamera.position - placePose.position;
    //        float distance = directionToCamera.magnitude;

    //        float minDistance = 1.0f; // 최소 거리 (미터)

    //        // 너무 가까우면 보정
    //        if (distance < minDistance)
    //        {
    //            Vector3 safeDirection = directionToCamera.normalized;
    //            placePose.position = arCamera.position - safeDirection * minDistance;
    //            placePose.position.y = arCamera.position.y - 1.5f; // 높이 재보정
    //        }
    //        canvas.transform.SetPositionAndRotation(placePose.position, Quaternion.LookRotation(directionToCamera));
    //    }
    //}

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

    private double Deg2Rad(double deg) => deg * Mathf.PI / 180.0f;
    private double Rad2Deg(double rad) => rad * 180.0f / Mathf.PI;
}
