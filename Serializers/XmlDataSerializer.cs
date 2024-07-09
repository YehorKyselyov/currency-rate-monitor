
using System.Xml.Serialization;
namespace NBU_Currency_Rate_Monitor.Serializers;

public class XmlDataSerializer : IDataSerializer
{
    public string Serialize(CurrenciesData data)
    {
        var xmlSerializer = new XmlSerializer(typeof(List<CurrencyData>));
        using (var stringWriter = new StringWriter())
        {
            xmlSerializer.Serialize(stringWriter, data);
            return stringWriter.ToString();
        }
    }
}