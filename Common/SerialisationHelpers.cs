using System.Globalization;

namespace StudioManette
{
    namespace Utils
    {
        public static class SerialisationHelpers
        {
            // Serialise all given float values into a string
            // Decimal count is variable to guarantee the full round trip (float value -> text -> same exact float value)
            public static string SerialiseFloatVector(float[] values)
            {
                string result = "";
                for (int i = 0; i < values.Length; ++i)
                {
                    if (i != 0)
                    {
                        result += ",";
                    }
                    // Using "round-trip" format specifier for the output formatting
                    result += string.Format(CultureInfo.InvariantCulture, "{0:r}", values[i]);
                }
                return result;
            }

            // Parse all given string values into floats
            public static float[] ParseFloatVector(string[] values)
            {
                float[] result = new float[values.Length];
                for (int i = 0; i < values.Length; ++i)
                {
                    float current = float.Parse(values[i], NumberStyles.Float, CultureInfo.InvariantCulture);
                    result[i] = current;
                }
                return result;
            }
        }
    }
}
