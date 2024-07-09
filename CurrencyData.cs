using System.Xml;

namespace NBU_Currency_Rate_Monitor;

public struct CurrencyData
{
    public string Code { get; }
    public double Rate { get; }
    public string Date { get; }
    public string Time { get; }

    public CurrencyData(string code, double rate, string date, string time)
    {
        Code = code;
        Rate = rate;
        Date = date;
        Time = time;
    }
    public CurrencyData() {}
}

public class CurrenciesData : List<CurrencyData>
{
    public CurrenciesData(string xmlInput)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlInput);

        var currencyNodes = xmlDoc.GetElementsByTagName("currency");
        foreach (XmlNode currencyNode in currencyNodes)
        {
            var code = currencyNode["cc"].InnerText;
            var rate = double.Parse(currencyNode["rate"].InnerText);
            var date = currencyNode["exchangedate"].InnerText;
            var time = DateTime.Now.ToString("HH:mm:ss");

            var currencyData = new CurrencyData(code, rate, date, time);
            this.Add(currencyData);
        }
    }
}