using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Suddath.Helix.JobMgmt.Infrastructure
{
    /// <summary>
    /// Extension methods for objects to allow easy serialization and deserialization
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// Method to serialize the given object
        /// </summary>
        /// <param name="input">The object to be serialized</param>
        /// <returns>A JSON representation of the object</returns>
        public static string Serialize<T>(this T input)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(input);
        }

        /// <summary>
        /// Deserializes a JSON string and casts it into the object
        /// </summary>
        /// <param name="input">The JSON string to be changed into an object</param>
        /// <returns>The object the JSON input represented</returns>
        public static void Deserialize<T>(this T serializable, string input)
        {
            Newtonsoft.Json.JsonConvert.PopulateObject(input, serializable);
        }
    }
}