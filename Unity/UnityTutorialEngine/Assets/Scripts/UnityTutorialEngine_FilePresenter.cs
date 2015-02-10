using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TutorialEngine;
using UnityEditor;
using UnityEngine;


class UnityFileSystemPresenter : IFileSystemPresenter
{
    private bool _isSceneOpen = false;

    public void CreateDefaultProject()
    {
        if (!_isSceneOpen)
        {

            EditorApplication.NewScene();

            // TODO: Add stuff to default scene

            EditorApplication.SaveScene("Tutorial", false);

            _isSceneOpen = true;
        }
    }

    public string GetFile(string filePath)
    {
        var fullPath = Application.dataPath + "/" + filePath;

        if (System.IO.File.Exists(fullPath))
        {
            return System.IO.File.ReadAllText(fullPath);
        }
        else
        {
            return "";
        }
    }

    public void SetFile(string filePath, string contents)
    {
        var fullPath = Application.dataPath + "/" + filePath;
        System.IO.File.WriteAllText(fullPath, contents);
    }
}
