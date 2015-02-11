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

            if (args.Length != 2)
            {
                Console.WriteLine("You must provide source and destination directories as args");

                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine("args[" + i + "] = " + args[i]);
                }

                throw new ArgumentException("You must provide source and destination directories as args");
                // TESTING
                args = new string[] { 
                @"D:\UserData\Projects\Products\Frameworks\TutorialEngine\Told.TutorialEngine.Unity\bin\Debug",
                @"D:\UserData\Projects\Products\Frameworks\TutorialEngine\Unity\UnityTutorialEngine\Assets\Assemblies"
                };
            }

            var sourceDir = args[0].TrimEnd('\\');
            var destDir = args[1].TrimEnd('\\');

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

                }
            }

            // In place sort
            filesToCopy.Sort((a, b) => a.FullName.CompareTo(b.FullName));

            foreach (var fileInfo in filesToCopy)
            {
                // Copy
                File.Copy(fileInfo.FullName, destDir + "\\" + fileInfo.Name, true);

                // Unity does this automatically now
                //// Create mdb for debugging
                //if (fileInfo.Extension == ".pdb")
                //{
                //    System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Unity\Editor\Data\Mono\lib\mono\2.0\pdb2mdb.exe", fileInfo.DirectoryName + "\\" + fileInfo.Name);
                //}
            }

        }
    }
}
