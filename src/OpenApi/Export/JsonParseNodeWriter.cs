﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Tavis.OpenApi
{
    public class JsonParseNodeWriter : IParseNodeWriter
    {
        enum ParseState
        {
            Initial,
            InList,
            InMap
        };

        Stack<ParseState> state = new Stack<ParseState>();
        StreamWriter writer;

        public JsonParseNodeWriter(Stream stream)
        {
            this.writer = new StreamWriter(stream);
        }

        public void Flush()
        {
            this.writer.Flush();
        }

        string Indent = "";
        bool first = true;
        void IncreaseIndent()
        {
            Indent += "  ";
        }
        void DecreaseIndent()
        {
            Indent = Indent.Substring(0,Indent.Length -2 );
        }
        public void WriteStartDocument() { }
        public void WriteEndDocument() { }
        public void WriteStartList() {
            writer.WriteLine(Indent + "[");
            state.Push(ParseState.InList);
            IncreaseIndent();
            first = true;
        }
        public void WriteEndList() {
            writer.WriteLine(Indent + "]");
            state.Pop();
            DecreaseIndent();
        }

        public void WriteStartMap() {
            writer.WriteLine(Indent + "{");
            state.Push(ParseState.InMap);
            IncreaseIndent();
            first = true;
        }
        public void WriteEndMap() {
            writer.WriteLine();
            writer.WriteLine(Indent + "}");
            state.Pop();
            DecreaseIndent();
        }

        public void WritePropertyName(string name) {
            if (!first)
            {
                writer.WriteLine(",");
            } else
            {
                first = false;
            }
            writer.Write(Indent + "\"" + name + "\": " );
        }

        public void WriteValue(string value) {
            writer.Write("\"" + value + "\"");
        }

        public void WriteValue(Decimal value) {
            writer.WriteLine(value.ToString());  //TODO deal with culture issues
        }

        public void WriteValue(int value) {
            writer.WriteLine(value.ToString());  //TODO deal with culture issues
       }

        public void WriteValue(bool value) {
            writer.WriteLine(value.ToString().ToLower());  //TODO deal with culture issues
        }
    }
}
