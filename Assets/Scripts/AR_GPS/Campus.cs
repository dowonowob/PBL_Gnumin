using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CampusData", menuName = "ScriptableObjects/CampusData")]
public class Campus : ScriptableObject
{
    public List<BuildingData> Sheet1 = new List<BuildingData>();
}

[System.Serializable]
public class BuildingData
{
    public double latitude;
    public double longitude;
    public string buildingName;

    [TextArea(3, 10)]
    public string buildingDescription;

    public string buildingNumber;
    public string pictureName;
    public string campusName;
    public string ginuCharacter;
}
