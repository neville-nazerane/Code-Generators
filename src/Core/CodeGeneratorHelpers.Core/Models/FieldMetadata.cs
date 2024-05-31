﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public class FieldMetadata : ICodeItem
    {

        public string Name { get; internal set; }

        public TypeMetadata Type { get; set; }

        public ClassMetadata ParentClass { get; internal set; }

        public string SourceFilePath { get; internal set; }
        public IEnumerable<AttributeMetadata> Attributes { get; internal set; }
        public IEnumerable<string> Modifiers { get; internal set; }
        public IEnumerable<string> Usings { get; internal set; }
    }
}
