﻿using CodeGeneratorHelpers.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Internals
{
    internal class GenerationState
    {

        public string RootFullPath { get; init; }

        public string GenerationFullPath { get; init; }

        internal readonly ConcurrentDictionary<string, CodeMetadata> _fileMetaCache = [];

    }
}
