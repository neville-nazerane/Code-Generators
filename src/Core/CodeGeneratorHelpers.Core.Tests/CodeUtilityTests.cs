using CodeGeneratorHelpers.Core.Internals;
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

            // ARRANGE

            string data = @"

                enum ElectronicNum {


        } somethingNice
        class Batman
public notNice
{

public class SuperMan {
}
internal static class WonderWoman { 

    class WonderGirl {}

}

interface SpiderMan {

}

";

            // ACT
            var res = CodeUtility.GetCodeMetaData(data);

            // ASSERT
            Assert.NotNull(res);
            Assert.NotNull(res.Classes);
            Assert.Equal(3, res.Classes.Count());

            var wonderWoman = res.Classes.SingleOrDefault(c => c.ClassName == "WonderWoman");
            Assert.NotNull(wonderWoman);
            Assert.Single(wonderWoman.Classes);

            Assert.Single(res.Interfaces);

            Assert.Single(res.Enums);
            Assert.Equal("ElectronicNum", res.Enums.Single().EnumName);

        }

    }
}
