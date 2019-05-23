﻿using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PbiTools.FileSystem;
using Serilog;

namespace PbiTools.Serialization
{
    public class JsonPartSerializer : IPowerBIPartSerializer<JObject>
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<JsonPartSerializer>();
        private readonly IProjectFile _file;

        public JsonPartSerializer(IProjectRootFolder folder, string label)
        {
            if (folder == null) throw new ArgumentNullException(nameof(folder));
            if (string.IsNullOrEmpty(label))
                throw new ArgumentException("Value cannot be null or empty.", nameof(label));

            _file = folder.GetFile($"{label}.json");
        }


        public void Serialize(JObject content)
        {
            if (content == null) return;
            _file.Write(content);
        }

        public bool TryDeserialize(out JObject part)
        {
            if (_file.TryGetFile(out var stream))
            {
                using (var reader = new JsonTextReader(new StreamReader(stream)))
                {
                    try
                    {
                        part = JToken.ReadFrom(reader) as JObject;
                        return (part != null);
                    }
                    catch (JsonException e)
                    {
                        Log.Error(e, "Json file is invalid: {Path}", _file.Path);
                    }
                }
            }

            part = null;
            return false;
        }
    }
}