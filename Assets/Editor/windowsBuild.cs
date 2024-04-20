using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BuildEditors
{
    public class Builder
    {
        public static void BuildProject(string path, BuildTarget buildTarget)
        {
            var options = new BuildPlayerOptions()
            {
                scenes = new[]
                {
                    "Assets/Scenes/StartScreen.unity",
                    "Assets/Scenes/Greybox.unity",
                },
                target = buildTarget,
                locationPathName = path,
            };

            BuildPipeline.BuildPlayer(options);
        }
    }
}

