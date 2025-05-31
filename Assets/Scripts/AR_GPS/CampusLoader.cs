using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CampusLoader : MonoBehaviour
{
    public TextAsset csvFile;
    public TourManager tourManager;

    void Awake()
    {
        Campus campus = new Campus();
        using (StringReader reader = new StringReader(csvFile.text))
        {
            string headerLine = reader.ReadLine(); // 헤더 무시
            while (reader.Peek() > -1)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',');

                if (values.Length < 7) continue;

                BuildingData data = new BuildingData
                {
                    buildingNumber = values[0],
                    buildingName = values[1],
                    buildingDescription = values[2],
                    latitude = float.Parse(values[3]),
                    longitude = float.Parse(values[4]),
                    campusName = values[5],
                    pictureName = values[6]
                };

                campus.Sheet1.Add(data);
            }
        }

        tourManager.campus = campus;
    }
}
