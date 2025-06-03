using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class CampusDataImporter : MonoBehaviour
{
    [MenuItem("Tools/Import Campus CSV")]
    public static void ImportCampusCSV()
    {
        string path = EditorUtility.OpenFilePanel("Select Campus CSV File", Application.dataPath, "csv");

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("CSV import cancelled.");
            return;
        }

        string[] lines = File.ReadAllLines(path);
        if (lines.Length < 2)
        {
            Debug.LogError("CSV file has no data.");
            return;
        }

        Campus campusAsset = ScriptableObject.CreateInstance<Campus>();

        string[] headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            if (values.Length != headers.Length)
            {
                Debug.LogWarning($"Line {i + 1} 항목 수가 부족합니다. 건너뜁니다.");
                continue;
            }

            BuildingData data = new BuildingData();
            for (int j = 0; j < headers.Length; j++)
            {
                string header = headers[j].Trim().ToLower();
                string value = values[j].Trim().Trim('"');

                switch (header)
                {
                    case "latitude":
                        if (!float.TryParse(value, out data.latitude))
                            Debug.LogWarning($"Line {i + 1} - 잘못된 위도 값: {value}");
                        break;
                    case "longitude":
                        if (!float.TryParse(value, out data.longitude))
                            Debug.LogWarning($"Line {i + 1} - 잘못된 경도 값: {value}");
                        break;
                    case "buildingname":
                        data.buildingName = value;
                        break;
                    case "buildingdescription":
                        data.buildingDescription = value;
                        break;
                    case "buildingnumber":
                        data.buildingNumber = value;
                        break;
                    case "picturename":
                        data.pictureName = value;
                        break;
                    case "campusname":
                        data.campusName = value;
                        break;
                    case "ginucharacter":
                        data.ginuCharacter = value;
                        break;
                }
            }

            campusAsset.Sheet1.Add(data);
        }

        string assetPath = "Assets/Resources/DataContainer.asset";
        AssetDatabase.CreateAsset(campusAsset, assetPath);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = campusAsset;

        Debug.Log("Campus data imported and asset created at " + assetPath);
    }
}
