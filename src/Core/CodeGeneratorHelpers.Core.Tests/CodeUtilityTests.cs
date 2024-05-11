﻿using CodeGeneratorHelpers.Core.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Tests
{

    public class CodeUtilityTests
    {

        [Fact]
        public void GetCodeMetaData_SingleTest_ReturnsClassData()
        {
            string data = @"

        somethingNice
        class Batman
public notNice
{

public class SuperMan {

internal static class WonderWoman {  }
";


            var res = CodeUtility.GetCodeMetaData(data);

        }

    }
}
