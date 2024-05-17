﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public class InterfaceMetadata : ICodeItem
    {

        public string InterfaceName { get; init; }

        public ClassMetadata ParentClass { get; init; }
        public string SourceFilePath { get; init; }

    }
}
