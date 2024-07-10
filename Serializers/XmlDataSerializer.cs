
using System.Xml.Serialization;
namespace NBU_Currency_Rate_Monitor.Serializers;

public class XmlDataSerializer : IDataSerializer
{
    public void Serialize(CurrenciesData data, string filePath)
    {
        CurrenciesData existingData;

        existingData = File.Exists(filePath) ? Deserialize(filePath) : new CurrenciesData();
        existingData.AddRange(data);

        var xmlSerializer = new XmlSerializer(typeof(CurrenciesData));
        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            xmlSerializer.Serialize(fileStream, existingData);
        }
    }

    public CurrenciesData Deserialize(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(CurrenciesData));
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            CurrenciesData result = (CurrenciesData)serializer.Deserialize(fileStream);
            return result;
        }
    }
}