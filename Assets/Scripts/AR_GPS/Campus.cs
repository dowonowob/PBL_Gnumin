using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CampusData", menuName = "ScriptableObjects/CampusData", order = 1)]
public class Campus : ScriptableObject
{
    public List<BuildingData> Sheet1 = new List<BuildingData>();
}
