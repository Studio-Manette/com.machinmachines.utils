using System;
using System.Globalization;

using UnityEngine.Profiling;

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
                Profiler.BeginSample("utilities - ParseFloatVector");
                float[] result = new float[values.Length];
                for (int i = 0; i < values.Length; ++i)
                {
                    float current = float.Parse(values[i], NumberStyles.Float, CultureInfo.InvariantCulture);
                    result[i] = current;
                }
                Profiler.EndSample();
                return result;
            }

#if NET_STANDARD_2_1
            public static float[] ParseFloatVector(ReadOnlySpan<char> values, ReadOnlySpan<char> separators, int expectedValuesCount)
            {
                Profiler.BeginSample("utilities - ParseFloatVector - Span");

                float[] result = new float[expectedValuesCount];
                int i = 0;

                int currentIndex = 0;
                int sliceLength = values.IndexOfAny(separators);
                while (sliceLength > 0)
                {
                    ReadOnlySpan<char> slice = values.Slice(currentIndex, sliceLength);
                    result[i] = float.Parse(slice, NumberStyles.Float, CultureInfo.InvariantCulture);
                    i += 1;
                    currentIndex += sliceLength + 1;
                    sliceLength = values.Slice(currentIndex).IndexOfAny(separators);
                }
                // Last term
                ReadOnlySpan<char> lastSlice = values.Slice(currentIndex, values.Length - currentIndex);
                result[result.Length - 1] = float.Parse(lastSlice, NumberStyles.Float, CultureInfo.InvariantCulture);

                Profiler.EndSample();
                return result;
            }
#endif  // NET_STANDARD_2_1
        }
    }
}
