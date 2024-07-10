using System.Xml;

namespace NBU_Currency_Rate_Monitor;

/// <summary>
/// Represents a single currency data entry.
/// </summary>
public struct CurrencyData
{
    public string Code;
    public double Rate;
    public string Date;
    public string Time;

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrencyData"/> struct.
    /// </summary>
    /// <param name="code">The currency abbreviation.</param>
    /// <param name="rate">The exchange rate.</param>
    /// <param name="date">The date of the exchange rate in format dd.mm.YYYY.</param>
    /// <param name="time">The time of the exchange rate in format HH:MM:SS.</param>
    public CurrencyData(string code, double rate, string date, string time)
    {
        Code = code;
        Rate = rate;
        Date = date;
        Time = time;
    }
    public CurrencyData() {}
}

/// <summary>
/// Represents a collection of currency data entries.
/// </summary>
public class CurrenciesData : List<CurrencyData>
{
    public CurrenciesData() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="CurrenciesData"/> class with data parsed from XML.
    /// </summary>
    /// <param name="xmlInput">The XML input containing currency data.</param>
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