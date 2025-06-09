using UnityEngine;
using UnityEditor;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;

public class CampusCsvImporter
{
    [MenuItem("Tools/Import Campus CSV (with CsvHelper Map)")]
    public static void Import()
    {
        string path = EditorUtility.OpenFilePanel("Select Campus CSV", "", "csv");
        if (string.IsNullOrEmpty(path)) return;

        var campus = ScriptableObject.CreateInstance<Campus>();

        try
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                //  명시적으로 매핑 설정
                csv.Context.RegisterClassMap<BuildingDataMap>();

                var records = csv.GetRecords<BuildingData>();
                int count = 0;

                foreach (var record in records)
                {
                    if (string.IsNullOrEmpty(record.buildingDescription))
                        Debug.LogWarning($" {record.buildingName} - description 비어 있음");
                    else
                        Debug.Log($" {record.buildingName} - OK");

                    campus.Sheet1.Add(record);
                    count++;
                }

                Debug.Log($" 총 {count}개의 데이터가 불러와졌습니다.");
            }

            string assetPath = "Assets/CampusData.asset";
            AssetDatabase.CreateAsset(campus, assetPath);
            AssetDatabase.SaveAssets();

            Debug.Log("CampusData.asset 생성 완료!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError(" CSV 파싱 실패: " + ex.Message);
        }
    }
}
