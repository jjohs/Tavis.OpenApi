﻿using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavis.OpenApi.Model
{
    public class RequestBody
    {
        public string Description { get; set; }
        public Boolean Required { get; set; }
        public Dictionary<string, MediaType> Content { get; set; }
        public Dictionary<string,string> Extensions { get; set; }

        public void Write(IParseNodeWriter writer)
        {
            writer.WriteStartMap();

            writer.WriteStringProperty("description", Description);
            writer.WriteBoolProperty("required", Required);
            writer.WriteMap("content", Content, MediaType.Write);

            writer.WriteEndMap();
        }

        public static void Write(IParseNodeWriter writer, RequestBody requestBody)
        {
            requestBody.Write(writer);
        }


    }
}
