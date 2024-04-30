using CodeGeneratorHelpers.Core.Internals;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core.Tests
{

    public class CodeGenerator_GetFullTargetAppPathTests
    {

        readonly Mock<IFileService> _mockedFileService;
        readonly CodeGenerator _codeGenerator;

        public CodeGenerator_GetFullTargetAppPathTests()
        {
            _mockedFileService = new();
            _codeGenerator = new CodeGenerator(_mockedFileService.Object);
        }

        [Fact]
        public void CurrentPathIsTarget_ProvidedOnlyDirName_ReturnsCurrentPath()
        {
            // ARRANGE
            string[] locations = [
                "C:",
                Path.Combine("C:", "data"),
                Path.Combine("C:", "data", "code"),
                Path.Combine("C:", "data", "code", "myproject")
            ];

            string currentPath = "C:/data/code/myproject";

            _mockedFileService.Setup(f => f.GetCurrentDirectory())
                             .Returns(currentPath);
            _mockedFileService.Setup(f => f.DirectoryExists(It.IsAny<string>()))
                             .Returns((string d) => locations.Contains(d));

            _codeGenerator.TargetAppPath = "myproject";

            // ACT
            var res = _codeGenerator.GetFullTargetAppPath();

            // ASSERT
            Assert.Equal(Path.GetFullPath(currentPath), Path.GetFullPath(res));
        }

        [Fact]
        public void CurrentPathIsTarget_ProvidedFullPath_ReturnsCurrentPath()
        {
            // ARRANGE
            string[] locations = [
                "C:",
                Path.Combine("C:", "data"),
                Path.Combine("C:", "data", "code"),
                Path.Combine("C:", "data", "code", "myproject")
            ];

            string currentPath = "C:/data/code/myproject";

            _mockedFileService.Setup(f => f.GetCurrentDirectory())
                             .Returns(currentPath);
            _mockedFileService.Setup(f => f.DirectoryExists(It.IsAny<string>()))
                             .Returns((string d) => locations.Contains(d));

            _codeGenerator.TargetAppPath = "C:/data/code/myproject";

            // ACT
            var res = _codeGenerator.GetFullTargetAppPath();

            // ASSERT
            Assert.Equal(Path.GetFullPath(currentPath), Path.GetFullPath(res));
        }

        [Fact]
        public void CurrentPathIsTarget_ProvidedPartialPath_ReturnsCurrentPath()
        {
            // ARRANGE
            string[] locations = [
                "C:",
                Path.Combine("C:", "data"),
                Path.Combine("C:", "data", "code"),
                Path.Combine("C:", "data", "code", "myproject")
            ];

            string currentPath = "code/myproject";

            _mockedFileService.Setup(f => f.GetCurrentDirectory())
                             .Returns(currentPath);
            _mockedFileService.Setup(f => f.DirectoryExists(It.IsAny<string>()))
                             .Returns((string d) => locations.Contains(d));

            _codeGenerator.TargetAppPath = "C:/data/code/myproject";

            // ACT
            var res = _codeGenerator.GetFullTargetAppPath();

            // ASSERT
            Assert.Equal(Path.GetFullPath("C:/data/code/myproject"), Path.GetFullPath(res));
        }

        [Fact]
        public void InvalidDirectoryPath_ThrowsException()
        {
            // ARRANGE
            string[] locations = [
                "C:",
                Path.Combine("C:", "data"),
                Path.Combine("C:", "data", "code"),
                Path.Combine("C:", "data", "code", "myproject")
            ];

            string currentPath = "C:/data/code/myproject";

            _mockedFileService.Setup(f => f.GetCurrentDirectory())
                             .Returns(currentPath);
            _mockedFileService.Setup(f => f.DirectoryExists(It.IsAny<string>()))
                             .Returns((string d) => locations.Contains(d));

            _codeGenerator.TargetAppPath = "notmyproject";

            // ACT & ASSERT
            Assert.Throws<DirectoryNotFoundException>(_codeGenerator.GetFullTargetAppPath);

        }

        [Fact]
        public void ParentDirectory_ProvidedOnlyDirName_ReturnsParentDirectory()
        {
            // ARRANGE
            string[] locations = [
                "C:",
                Path.Combine("C:", "data"),
                Path.Combine("C:", "data", "code"),
                Path.Combine("C:", "data", "code", "myproject")
            ];

            string currentPath = "C:/data/code/myproject";

            _mockedFileService.Setup(f => f.GetCurrentDirectory())
                             .Returns(currentPath);
            _mockedFileService.Setup(f => f.DirectoryExists(It.IsAny<string>()))
                             .Returns((string d) => locations.Contains(d));

            _codeGenerator.TargetAppPath = "code";

            // ACT
            var res = _codeGenerator.GetFullTargetAppPath();

            // ASSERT
            Assert.Equal(Path.GetFullPath("C:/data/code"), Path.GetFullPath(res));
        }

        [Fact]
        public void NestedOtherDirectory_ProvidedPath_ReturnsTargetPath()
        {
            // ARRANGE
            string[] locations = [
                "C:",
                Path.Combine("C:", "data"),
                Path.Combine("C:", "data", "code"),
                Path.Combine("C:", "data", "code", "myproject"),
                Path.Combine("C:", "data", "src"),
                Path.Combine("C:", "data", "src", "realProject"),
            ];

            string currentPath = "C:/data/code/myproject";

            _mockedFileService.Setup(f => f.GetCurrentDirectory())
                             .Returns(currentPath);
            _mockedFileService.Setup(f => f.DirectoryExists(It.IsAny<string>()))
                             .Returns((string d) => locations.Contains(d));

            _codeGenerator.TargetAppPath = "src/realProject";

            // ACT
            var res = _codeGenerator.GetFullTargetAppPath();

            // ASSERT
            Assert.Equal(Path.GetFullPath("C:/data/src/realProject"), Path.GetFullPath(res));
        }

    }
}
