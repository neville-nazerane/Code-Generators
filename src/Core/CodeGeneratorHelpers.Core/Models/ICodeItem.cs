﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Models
{
    public interface ICodeItem
    {

        public ClassMetadata ParentClass { get; }

        public string SourceFilePath { get; }

    }
}
