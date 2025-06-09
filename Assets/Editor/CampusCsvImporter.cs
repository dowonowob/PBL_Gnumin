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
                //  ��������� ���� ����
                csv.Context.RegisterClassMap<BuildingDataMap>();

                var records = csv.GetRecords<BuildingData>();
                int count = 0;

                foreach (var record in records)
                {
                    if (string.IsNullOrEmpty(record.buildingDescription))
                        Debug.LogWarning($" {record.buildingName} - description ��� ����");
                    else
                        Debug.Log($" {record.buildingName} - OK");

                    campus.Sheet1.Add(record);
                    count++;
                }

                Debug.Log($" �� {count}���� �����Ͱ� �ҷ��������ϴ�.");
            }

            string assetPath = "Assets/CampusData.asset";
            AssetDatabase.CreateAsset(campus, assetPath);
            AssetDatabase.SaveAssets();

            Debug.Log("CampusData.asset ���� �Ϸ�!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError(" CSV �Ľ� ����: " + ex.Message);
        }
    }
}
