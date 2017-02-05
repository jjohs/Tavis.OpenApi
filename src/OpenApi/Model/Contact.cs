﻿using System;
using System.Collections.Generic;
using SharpYaml.Serialization;

namespace Tavis.OpenApi.Model
{

    public class Contact
    {
        public string Name { get; set; }
        public Uri Url { get; set; }
        public string Email { get; set; }
        public Dictionary<string,string> Extensions { get; set; }

        private static FixedFieldMap<Contact> fixedFields = new FixedFieldMap<Contact> {
            { "name", (o,n) => { o.Name = n.GetScalarValue(); } },
            { "email", (o,n) => { o.Email = n.GetScalarValue(); } },
            { "url", (o,n) => { o.Url = new Uri(n.GetScalarValue()); } },
            };

        private static PatternFieldMap<Contact> patternFields = new PatternFieldMap<Contact>
        {
            { (s)=> s.StartsWith("x-"), (o,k,n)=> o.Extensions.Add(k, n.GetScalarValue()) }
        };

        public Contact()
        {
            Extensions = new Dictionary<string, string>();
        }


        public static Contact Load(ParseNode node)
        {
            var contactNode = node as MapNode;
            if (contactNode == null)
            {
                throw new Exception("Contact node should be a map");
            }
            var contact = new Contact();

            foreach (var propertyNode in contactNode)
            {
                propertyNode.ParseField(contact, fixedFields, patternFields);
            }

            return contact;
        }
    }
}