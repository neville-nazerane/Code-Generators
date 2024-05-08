using CodeGeneratorHelpers.Core.Internals;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Tests
{
    public class CodeGenerator_AddProcessTests
    {


        readonly Mock<IFileService> _mockedFileService;
        readonly CodeGenerator _codeGenerator;

        public CodeGenerator_AddProcessTests()
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
            Assert.NotNull(_codeGenerator.Processes);
            Assert.Empty(_codeGenerator.Processes);
        }

        [Fact]
        public void AddedOne_EnsureOnlyOnePresent()
        {

            // ARRANGE
            // Nothing Jon Snow

            // ACT
            _codeGenerator.AddProcess(c => Task.CompletedTask);

            // ASSERT
            Assert.Single(_codeGenerator.Processes);

        }


        [Fact]
        public void AddedThree_EnsureThreePresent()
        {

            // ARRANGE
            // Nothing Jon Snow

            // ACT
            _codeGenerator.AddProcess(c => Task.CompletedTask);
            _codeGenerator.AddProcess(c => Task.CompletedTask);
            _codeGenerator.AddProcess(c => Task.CompletedTask);

            // ASSERT
            Assert.Equal(3, _codeGenerator.Processes.Count);

        }

        [Fact]
        public void AddingDirectlyToProcess_AddedTwo_EnsureTwoPresent()
        {
            // ARRANGE
            // Nothing Jon Snow

            // ACT
            _codeGenerator.Processes.Add(c => Task.CompletedTask);
            _codeGenerator.Processes.Add(c => Task.CompletedTask);

            // ASSERT
            Assert.Equal(2, _codeGenerator.Processes.Count);
        }

    }
}
