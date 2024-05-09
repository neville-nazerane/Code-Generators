using CodeGeneratorHelpers.Core.Internals;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Tests
{
    public class CodeGenerator_AddProcessForEachFileInFolderTests
    {


        readonly Mock<IFileService> _mockedFileService;
        readonly CodeGenerator _codeGenerator;

        public CodeGenerator_AddProcessForEachFileInFolderTests()
        {
            _mockedFileService = new();
            _codeGenerator = new CodeGenerator(_mockedFileService.Object);
        }

        [Fact]
        public void NoneAddedYet_EnsureProcessesIsNotNullAndEmpty()
        {

            // ARRANGE
            // Nothing yet Jon Snow

            // ACT
            // Nothing yet Jon Snow


            // ASSERT
            Assert.NotNull(_codeGenerator.ProcessesForFilesInFolder);
            Assert.Empty(_codeGenerator.ProcessesForFilesInFolder);
        }

        [Fact]
        public void AddedOne_EnsureOnlyOnePresent()
        {

            // ARRANGE
            // Nothing Jon Snow

            // ACT
            _codeGenerator.AddProcessForEachFileInFolder("Something nice", c => Task.CompletedTask);

            // ASSERT
            Assert.Single(_codeGenerator.ProcessesForFilesInFolder);

        }


        [Fact]
        public void AddedThree_EnsureThreePresent()
        {

            // ARRANGE
            // Nothing Jon Snow

            // ACT
            _codeGenerator.AddProcessForEachFileInFolder("Something strange", c => Task.CompletedTask);
            _codeGenerator.AddProcessForEachFileInFolder("In the neighborhood", c => Task.CompletedTask);
            _codeGenerator.AddProcessForEachFileInFolder("Who you gonna call", c => Task.CompletedTask);

            // ASSERT
            Assert.Equal(3, _codeGenerator.ProcessesForFilesInFolder.Count);

        }

        [Fact]
        public void AddingDirectlyToProcess_AddedTwo_EnsureTwoPresent()
        {
            // ARRANGE
            // Nothing Jon Snow

            // ACT
            _codeGenerator.ProcessesForFilesInFolder.Add(new("I see dead people", c => Task.CompletedTask));
            _codeGenerator.ProcessesForFilesInFolder.Add(new("They don't know they're dead", c => Task.CompletedTask));

            // ASSERT
            Assert.Equal(2, _codeGenerator.ProcessesForFilesInFolder.Count);
        }
    }
}
