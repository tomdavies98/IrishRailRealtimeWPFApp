
namespace IrishRailTimer
{
    public static class StringSplitter
    {
        public static string ExtractValue(string value)
        {
            int startPos = value.IndexOf('>');
            int endPos = value.IndexOf('<', value.IndexOf('<') + 1) - startPos;

            if (startPos > endPos && startPos >= 0 && endPos >= 0)
            {
                string substringValue = value.Substring(startPos + 1, endPos - 1);
                return substringValue;
            }

            return "";
        }

        public static string ExtractTrainDetail(string data)
        {
            string value = data.Split('<')[0];

            return value;
        }
    }
}
