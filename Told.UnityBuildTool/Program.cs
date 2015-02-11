using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Told.UnityBuildTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("UnityBuildTool");

            if (args.Length < 2)
            {
                Console.WriteLine("You must provide source and destination directories as args");

                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine("args[" + i + "] = " + args[i]);
                }

                //throw new ArgumentException("You must provide source and destination directories as args");
                // TESTING
                args = new string[] { 
                @"D:\UserData\Projects\Products\Frameworks\TutorialEngine\Told.TutorialEngine.Unity\bin\Debug",
                @"D:\UserData\Projects\Products\Frameworks\TutorialEngine\Unity\UnityTutorialEngine\Assets\Assemblies",
                @"Release"
                };
            }

            var sourceDir = args[0].TrimEnd('\\');
            var destDir = args[1].TrimEnd('\\');
            var isRelease = args.Length > 2 ? (args[2] == "Release") : false;

            // FROM: http://forum.unity3d.com/threads/video-tutorial-how-to-use-visual-studio-for-all-your-unity-development.120327/
            // echo f | xcopy "$(TargetPath)" "C:\MyProject\MyProject.Unity\Assets\Assemblies\" /Y
            // echo f | xcopy "$(TargetDir)$(TargetName).pdb" "C:\MyProject\MyProject.Unity\Assets\Assemblies\" /Y
            //"C:\Program Files (x86)\Unity\Editor\Data\Mono\lib\mono\2.0\pdb2mdb.exe" "C:\MyProject\MyProject.Unity\Assets\Assemblies\$(T  argetFileName)"

            var filesToCopy = new List<FileInfo>();

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var fileInfo = new FileInfo(file);

                if (fileInfo.Extension == ".dll" || fileInfo.Extension == ".pdb")
                {
                    filesToCopy.Add(fileInfo);

                    Console.WriteLine("Found: " + fileInfo.Name);
                }
            }

            // In place sort
            filesToCopy.Sort((a, b) => a.FullName.CompareTo(b.FullName));


            if (!isRelease)
            {
                // Copy files
                foreach (var fileInfo in filesToCopy)
                {
                    File.Copy(fileInfo.FullName, destDir + "\\" + fileInfo.Name, true);
                    Console.WriteLine("Copied: " + fileInfo.Name);
                }
            }
            else
            {
                var mergedFileDir = sourceDir;
                var mergedFileName = "ilmerged.dll";
                var mergedFilePath = mergedFileDir + "\\" + mergedFileName;
                var obfFileDir = sourceDir + "\\" + "obfuscated";
                var obfFilePath = sourceDir + "\\" + "obfuscated\\ilmerged.dll";
                var copiedFilePath = destDir + "\\" + "obfuscated.dll";

                // Merge
                //var merger = new ILMerging.ILMerge();
                //merger.SetInputAssemblies(filesToCopy.Where(f => f.Extension == ".dll").Select(f => f.FullName).ToArray());
                //merger.OutputFile = sourceDir + "\\" + "ilmerged.dll";
                //merger.Merge();

                var mergeToolPath = Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\..\Tools\ILMerge.exe");
                var mergeToolArgs = "";
                foreach (var f in filesToCopy.Where(f => f.Extension == ".dll"))
                {
                    mergeToolArgs += "\"" + f.FullName + "\" ";
                }

                mergeToolArgs += "/out:\"" + mergedFilePath + "\" ";

                var pMerge = System.Diagnostics.Process.Start(mergeToolPath, mergeToolArgs);
                pMerge.WaitForExit();
                if (!File.Exists(mergedFilePath)) { throw new Exception("Merging Failed"); }

                // Obfuscate
                var obfToolPath = Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\..\Tools\ConfuserEx_bin\Confuser.CLI.exe");

                var path = CreateProjFile(mergedFileDir, mergedFileName, obfFileDir);
                var obfToolArgs = "nopause "+ "\"" + path + "\"";

                var pObf = System.Diagnostics.Process.Start(obfToolPath, obfToolArgs);
                pObf.WaitForExit();
                if (!File.Exists(obfFilePath)) { throw new Exception("Obfuscation Failed"); }

                // Copy
                File.Copy(obfFilePath, copiedFilePath, true);
                if (!File.Exists(copiedFilePath)) { throw new Exception("Copying Failed"); }
            }

        }

        private static string CreateProjFile(string mergedFileDir, string mergedFileName, string obfFileDir)
        {
            var fileTemplate = @"
<project outputDir=""{OUTPUTDIR}"" baseDir=""{BASEDIR}"" xmlns=""http://confuser.codeplex.com"">
  <rule pattern=""true"" preset=""normal"" inherit=""false"" />
  {MODULES}
</project>";

            var moduleTemplate = @"<module path=""{MODULEFILENAME}"" />";

            var outputDir = obfFileDir;
            var sourceDir = mergedFileDir;
            var modules = moduleTemplate.Replace("{MODULEFILENAME}", mergedFileName);

            var projFile = fileTemplate
                .Replace("{OUTPUTDIR}", outputDir)
                .Replace("{BASEDIR}", sourceDir)
                .Replace("{MODULES}", modules)
                ;

            // Save projFile
            if (!Directory.Exists(outputDir)) { Directory.CreateDirectory(outputDir); }
            var tFile = outputDir + "\\" + "obfProj.crproj";
            File.WriteAllText(tFile, projFile);

            return tFile;
        }
    }
}
