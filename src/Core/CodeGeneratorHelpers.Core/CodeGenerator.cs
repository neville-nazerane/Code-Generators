using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorHelpers.Core
{
    public class CodeGenerator
    {
        /// <summary>
        /// Path of target app to generate code on
        /// </summary>
        public string TargetAppPath { get; set; }

        /// <summary>
        /// 
        /// Path within the target app to place generated files by default
        /// 
        /// Default: "Generated"
        /// </summary>
        public string GeneratedFilesPath { get; set; } = "Generated";

        /// <summary>
        /// Path of target app to generate code on
        /// </summary>
        public CodeGenerator SetTargetAppPath(string path)
        {
            TargetAppPath = path;
            return this;
        }

        /// <summary>
        /// 
        /// Path within the target app to place generated files by default
        /// 
        /// Default: "Generated"
        /// </summary>
        public CodeGenerator SetGeneratedFilesPath(string path)
        {
            GeneratedFilesPath = path;
            return this;
        }

        public string GetFullTargetAppPath()
        {
            char[] dirSeperators = ['/', '\\'];

            var cleanedAppPath = Path.Combine(TargetAppPath.Split(dirSeperators));

            var dirs = Directory.GetCurrentDirectory().Split(dirSeperators);

            var triedPaths = new List<string>();

            for (int i = dirs.Length - 1; i >= 0; i--)
            {
                string path = Path.Combine(dirs.Take(i).ToArray());
                if (path.EndsWith(cleanedAppPath) && Directory.Exists(path))
                    return path;
                triedPaths.Add(path);
                path = Path.Combine(path, cleanedAppPath);
                if (Directory.Exists(path))
                    return path;
                triedPaths.Add(path);
            }

            throw new DirectoryNotFoundException(
                $"Failed to find app path. Tried {string.Join('\n', triedPaths.Select(t => $"'{t}'").ToArray())}");
        }

        


    }
}
