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

        public CodeGenerator_GetFullTargetAppPathTests()
        {
            _mockedFileService = new();
        }

        [Fact]
        public void CurrentPathIsTarget_ProvidedOnlyDirName_ReturnsCurrentPath()
        {
            // ARRANGE
            string[] locations = [
                "C:",
                "C:/data",
                "C:/data/code",
                "C:/data/code/myproject"
            ];

            string currentPath = "C:/data/code/myproject";

            _mockedFileService.Setup(f => f.GetCurrentDirectory())
                             .Returns(currentPath);
            _mockedFileService.Setup(f => f.DirectoryExists(It.IsAny<string>()))
                             .Returns((string d) => locations.Contains(d));
            MockCombineFullPath();

            var codeGenerator = new CodeGenerator(_mockedFileService.Object, "myproject");

            // ACT
            var res = codeGenerator.FullAppTargetPath;

            // ASSERT
            Assert.Equal(Path.GetFullPath(currentPath), Path.GetFullPath(res));
        }

        [Fact]
        public void CurrentPathIsTarget_ProvidedFullPath_ReturnsCurrentPath()
        {
            // ARRANGE
            string[] locations = [
                "C:",
                "C:/data",
                "C:/data/code",
                "C:/data/code/myproject"
            ];

            string currentPath = "C:/data/code/myproject";

            _mockedFileService.Setup(f => f.GetCurrentDirectory())
                             .Returns(currentPath);
            _mockedFileService.Setup(f => f.DirectoryExists(It.IsAny<string>()))
                             .Returns((string d) => locations.Contains(d));
            MockCombineFullPath();

            var codeGenerator = new CodeGenerator(_mockedFileService.Object, "C:/data/code/myproject");

            // ACT
            var res = codeGenerator.FullAppTargetPath;

            // ASSERT
            Assert.Equal(Path.GetFullPath(currentPath), Path.GetFullPath(res));
        }

        [Fact]
        public void CurrentPathIsTarget_ProvidedPartialPath_ReturnsCurrentPath()
        {
            // ARRANGE
            string[] locations = [
                "C:",
                "C:/data",
                "C:/data/code",
                "C:/data/code/myproject"
            ];

            _mockedFileService.Setup(f => f.GetCurrentDirectory())
                             .Returns("C:/data/code/myproject");
            _mockedFileService.Setup(f => f.DirectoryExists(It.IsAny<string>()))
                             .Returns((string d) => locations.Contains(d));

            MockCombineFullPath();

            var codeGenerator = new CodeGenerator(_mockedFileService.Object, "C:/data/code/myproject");

            // ACT
            var res = codeGenerator.FullAppTargetPath;

            // ASSERT
            Assert.Equal(Path.GetFullPath("C:/data/code/myproject"), Path.GetFullPath(res));
        }

        [Fact]
        public void InvalidDirectoryPath_ThrowsException()
        {
            // ARRANGE
            string[] locations = [
                "C:",
                "C:/data",
                "C:/data/code",
                "C:/data/code/myproject"
            ];

            string currentPath = "C:/data/code/myproject";

            _mockedFileService.Setup(f => f.GetCurrentDirectory())
                             .Returns(currentPath);
            _mockedFileService.Setup(f => f.DirectoryExists(It.IsAny<string>()))
                             .Returns((string d) => locations.Contains(d));
            MockCombineFullPath();

            // ACT & ASSERT
            Assert.Throws<DirectoryNotFoundException>(() => new CodeGenerator(_mockedFileService.Object, "notmyproject"));

        }

        [Fact]
        public void ParentDirectory_ProvidedOnlyDirName_ReturnsParentDirectory()
        {
            // ARRANGE
            string[] locations = [
                "C:",
                "C:/data",
                "C:/data/code",
                "C:/data/code/myproject"
            ];

            string currentPath = "C:/data/code/myproject";

            _mockedFileService.Setup(f => f.GetCurrentDirectory())
                              .Returns(currentPath);
            _mockedFileService.Setup(f => f.DirectoryExists(It.IsAny<string>()))
                              .Returns((string d) => locations.Contains(d));
            MockCombineFullPath();

            var codeGenerator = new CodeGenerator(_mockedFileService.Object, "code");

            // ACT
            var res = codeGenerator.FullAppTargetPath;

            // ASSERT
            Assert.Equal(Path.GetFullPath("C:/data/code"), Path.GetFullPath(res));
        }

        [Fact]
        public void NestedOtherDirectory_ProvidedPath_ReturnsTargetPath()
        {
            // ARRANGE
            string[] locations = [
                "C:",
                "C:/data",
                "C:/data/code",
                "C:/data/code/myproject",
                "C:/data/src",
                "C:/data/src/realProject"
            ];

            string currentPath = "C:/data/code/myproject";

            _mockedFileService.Setup(f => f.GetCurrentDirectory())
                             .Returns(currentPath);
            _mockedFileService.Setup(f => f.DirectoryExists(It.IsAny<string>()))
                             .Returns((string d) => locations.Contains(d));
            MockCombineFullPath();

            var codeGenerator = new CodeGenerator(_mockedFileService.Object, "src/realProject");

            // ACT
            var res = codeGenerator.FullAppTargetPath;

            // ASSERT
            Assert.Equal(Path.GetFullPath("C:/data/src/realProject"), Path.GetFullPath(res));
        }

        private void MockCombineFullPath()
        {
            _mockedFileService.Setup(f => f.Combine(It.IsAny<string[]>()))
                              .Returns((string[] arr) => string.Join('/', arr));
            _mockedFileService.Setup(f => f.CombineToFullPath(It.IsAny<string[]>()))
                              .Returns((string[] arr) => string.Join('/', arr.Where(a => !string.IsNullOrEmpty(a))));
        }

    }
}
