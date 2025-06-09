using CsvHelper.Configuration;

public sealed class BuildingDataMap : ClassMap<BuildingData>
{
    public BuildingDataMap()
    {
        Map(m => m.latitude).Name("latitude");
        Map(m => m.longitude).Name("longitude");
        Map(m => m.buildingName).Name("buildingName");
        Map(m => m.buildingDescription).Name("buildingDescription");
        Map(m => m.buildingNumber).Name("buildingNumber");
        Map(m => m.pictureName).Name("pictureName");
        Map(m => m.campusName).Name("campusName");
        Map(m => m.ginuCharacter).Name("ginuCharacter");
    }
}
