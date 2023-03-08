using System.Globalization;

namespace CsvHandler.Utils;

public static class Util
{
    /** Parse datetime like 2023-02-08_19-25-53
     * @param dateTimeStr - date like 2023-02-08_19-25-53
     * @return DateTime object
     */
    public static DateTime ParseDateTime(string dateTimeStr)
    {
        var year =   int.Parse(dateTimeStr[..4]); 
        var month =  int.Parse(dateTimeStr[5..7]); 
        var day =    int.Parse(dateTimeStr[8..10]);
        var hour =   int.Parse(dateTimeStr[11..13]); 
        var minute = int.Parse(dateTimeStr[14..16]); 
        var second = int.Parse(dateTimeStr[17..]); 
        
        return new DateTime(year, month, day,
            hour, minute, second
        );
    }

    /** Parse double like 111,111
     * 
     */
    public static double ParseDouble(string s)
    {
        s = s.Replace(",", ".");
        return double.Parse(s, CultureInfo.InvariantCulture);
    }
}