﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;

namespace Tavis.OpenApi.Model
{

    
    public class OpenApiDocument
    {
        public string Version { get; set; } // Swagger

        public Info Info { get; set; } = new Info();
        public List<Server> Servers { get; set; } = new List<Server>();
        public List<SecurityRequirement> SecurityRequirements { get; set; }

        public Paths Paths { get; set; }
        public Components Components { get; set; } = new Components();
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public ExternalDocs ExternalDocs { get; set; } = new ExternalDocs();
        public Dictionary<string, string> Extensions { get; set; } = new Dictionary<string, string>();

        public List<string> ParseErrors { get; set; } = new List<string>();

 
        private static FixedFieldMap<OpenApiDocument> fixedFields = new FixedFieldMap<OpenApiDocument> {
            { "openapi", (o,n) => { o.Version = n.GetScalarValue(); } },
            { "info", (o,n) => o.Info = Info.Load(n) },
            { "paths", (o,n) => o.Paths = Paths.Load(n) },
            { "components", (o,n) => o.Components = Components.Load(n) },
            { "tags", (o,n) => o.Tags = n.CreateList(Tag.Load)},
            { "externalDocs", (o,n) => o.ExternalDocs = ExternalDocs.Load(n) },
            { "security", (o,n) => o.SecurityRequirements = n.CreateList(SecurityRequirement.Load)}

            };

        private static PatternFieldMap<OpenApiDocument> patternFields = new PatternFieldMap<OpenApiDocument>
        {
                    // We have no semantics to verify X- nodes, therefore treat them as just values.
                   { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, ((ValueNode)n).GetScalarValue()) }
        };


        public static OpenApiDocument Parse(Stream stream)
        {
            var ctx = new ParsingContext();
            var rootNode = new RootNode(ctx,stream);
            return Load(rootNode);
        }

        public static OpenApiDocument Load(RootNode rootNode)
        {
            var openApidoc = new OpenApiDocument();

            var rootMap = rootNode.GetMap();
            
            foreach (var node in rootMap)
            {
                node.ParseField<OpenApiDocument>(openApidoc, OpenApiDocument.fixedFields, OpenApiDocument.patternFields);
            }

            openApidoc.ParseErrors = openApidoc.Validate();
            return openApidoc;
        }

        public List<string> Validate()
        {
            var errors = new List<string>();

            Validate(errors);
            return errors;
        }

        private Regex versionRegex = new Regex(@"\d+\.\d+\.\d+");

        private void Validate(List<string> errors)
        {
            if (!versionRegex.IsMatch(Version))
            {
                errors.Add("`openapi` property does not match the required format major.minor.patch");
            }

            Info.Validate(errors);

            if (Paths == null)
            {
                errors.Add("`paths` is a required property");
            } else
            {
                Paths.Validate(errors);
            }

        }
    }

    
    public class FixedFieldMap<T> : Dictionary<string, Action<T, ParseNode>>
    {
   
    }

    public class PatternFieldMap<T> : Dictionary<Func<string, bool>, Action<T, string, ParseNode>>
    {
    }

    public class ParsingContext
    {

    }

}
