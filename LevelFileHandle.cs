using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SRLE.Models;

namespace SRLE
{
    /// <summary>
    /// Base class for level serializers. Other mods can subclass this and register
    /// via <see cref="LevelFileHandle.Register"/> to add support for new file formats.
    /// </summary>
    public abstract class LevelSerializer
    {
        /// <summary>File extension this serializer handles, e.g. ".srle" or ".myformat".</summary>
        public abstract string Extension { get; }

        public abstract string Serialize(LevelData data);
        public abstract LevelData Deserialize(string content);
    }

    /// <summary>
    /// Routes level save/load through the appropriate registered <see cref="LevelSerializer"/>
    /// based on file extension. Falls back to the built-in JSON serializer for ".srle" files.
    /// </summary>
    public static class LevelFileHandle
    {
        private static readonly List<LevelSerializer> s_Serializers = new List<LevelSerializer>
        {
            new JsonLevelSerializer()
        };

        /// <summary>
        /// Register a custom serializer. Registrations are checked before the built-in ones,
        /// so a mod can override the default ".srle" format by registering its own serializer
        /// for the same extension.
        /// </summary>
        public static void Register(LevelSerializer serializer)
        {
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            s_Serializers.Insert(0, serializer);
        }

        /// <summary>Serialize <paramref name="data"/> using the serializer matching the path's extension.</summary>
        public static string Serialize(LevelData data, string path)
        {
            var ext = Path.GetExtension(path);
            var serializer = FindSerializer(ext);
            if (serializer == null)
                throw new NotSupportedException($"[SRLE] No serializer registered for extension '{ext}'");
            return serializer.Serialize(data);
        }

        /// <summary>Deserialize level data from <paramref name="content"/> using the serializer matching the path's extension.</summary>
        public static LevelData Deserialize(string content, string path)
        {
            var ext = Path.GetExtension(path);
            var serializer = FindSerializer(ext);
            if (serializer == null)
                throw new NotSupportedException($"[SRLE] No serializer registered for extension '{ext}'");
            return serializer.Deserialize(content);
        }

        private static LevelSerializer FindSerializer(string extension)
        {
            foreach (var s in s_Serializers)
                if (string.Equals(s.Extension, extension, StringComparison.OrdinalIgnoreCase))
                    return s;
            return null;
        }
    }

    /// <summary>Built-in JSON serializer for ".srle" files.</summary>
    public sealed class JsonLevelSerializer : LevelSerializer
    {
        public override string Extension => ".srle";

        public override string Serialize(LevelData data)
            => JsonConvert.SerializeObject(data);

        public override LevelData Deserialize(string content)
            => JsonConvert.DeserializeObject<LevelData>(content);
    }
}
