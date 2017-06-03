﻿using Tavis.OpenApi.Model;
using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Tavis.OpenApi;

namespace OpenApiTests
{
    public class BasicTests
    {
        [Fact]
        public void TestYamlBuilding()
        {
            var doc = new YamlDocument(new YamlMappingNode(
                            new YamlScalarNode("a"), new YamlScalarNode("apple"),
                            new YamlScalarNode("b"), new YamlScalarNode("banana"),
                            new YamlScalarNode("c"), new YamlScalarNode("cherry")
                    ));

            var s = new YamlStream();
            s.Documents.Add(doc);
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            s.Save(writer);
            var output = sb.ToString();
        }

        [Fact]
        public void ParseSimplestOpenApiEver()
        {

            var stream = this.GetType().Assembly.GetManifestResourceStream("OpenApiTests.Samples.Simplest.yaml");

            var parser = new OpenApiParser();
             
            var openApiDoc = parser.Parse(stream); 

            Assert.Equal("1.0.0", openApiDoc.Version);
            Assert.Equal(0, openApiDoc.Paths.PathItems.Count());
            Assert.Equal("The Api", openApiDoc.Info.Title);
            Assert.Equal("0.9.1", openApiDoc.Info.Version);
            Assert.Equal(0, parser.ParseErrors.Count);

        }

        [Fact]
        public void ParseBrokenSimplest()
        {

            var stream = this.GetType().Assembly.GetManifestResourceStream("OpenApiTests.Samples.BrokenSimplest.yaml");

            var parser = new OpenApiParser();
            var openApiDoc = parser.Parse(stream);

            Assert.Equal(2, parser.ParseErrors.Count);
            Assert.NotNull(parser.ParseErrors.Where(s=> s.ToString() == "`openapi` property does not match the required format major.minor.patch").FirstOrDefault());
            Assert.NotNull(parser.ParseErrors.Where(s => s.ToString() == "title is a required property of #/info").FirstOrDefault());

        }

        [Fact]
        public void CheckOpenAPIVersion()
        {

            var stream = this.GetType().Assembly.GetManifestResourceStream("OpenApiTests.Samples.petstore30.yaml");
            var parser = new OpenApiParser();
            var openApiDoc = parser.Parse(stream);

            Assert.Equal("3.0.0", openApiDoc.Version);

        }




    }
}
