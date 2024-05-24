using CodeGeneratorHelpers.Core.Internals;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
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

        Dictionary<int, string> mapping;

        public string PropertyName { get; set; }

        public ClassMetadata ParentClass { get; init; }
        public string SourceFilePath { get; init; }

        private void SampleFunction1(int i, string str)
        {

        }

        public string Function2(string str, bool b) => str;

}

internal static class WonderWoman { 

    int abc, def, ghi;

    [Hello(23, 21), hi]
    [Bye]
    string jkl;


    class WonderGirl {}

}

interface SpiderMan {

}

";

            // ACT
            var res = CodeUtility.GetCodeMetaData(data, "file.txt");

            // ASSERT
            Assert.NotNull(res);
            Assert.NotNull(res.Classes);
            Assert.Equal(3, res.Classes.Count());

            var wonderWoman = res.Classes.SingleOrDefault(c => c.Name == "WonderWoman");
            Assert.NotNull(wonderWoman);
            Assert.Single(wonderWoman.Classes);

            Assert.Single(res.Interfaces);

            Assert.Single(res.Enums);
            Assert.Equal("ElectronicNum", res.Enums.Single().Name);

            var superman = res.Classes.SingleOrDefault(c => c.Name == "SuperMan");
            Assert.NotNull(superman);
            Assert.NotNull(superman.Properties);
            Assert.Equal(3, superman.Properties.Count());
            Assert.NotNull(superman.Properties.First().ParentClass);

        }

    }
}
