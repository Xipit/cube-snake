using System.Linq;
using UnityEngine;
using UnityEditor;

namespace TutorialInfo.Scripts.Snake
{
    [CustomEditor(typeof(Line))]
    public class LineInspector: Editor
    {
        private void OnSceneGUI()
        {
            var line = target as Line;

            if (line.path != null)
            {
                var handleTransform = line.transform;
                var p0 = handleTransform.TransformPoint(line.p0);
            
                // draw Line
                Handles.color = Color.white;

                for (int i = 0; i < line.path.Count; i++)
                {
                    if (i+1 <= line.path.Count -1)
                    {
                        Handles.DrawLine(line.path[i], line.path[i+1]);
                    }
                }
                
                // Handles.DrawLine(line.path.First(), line.path.Last());
            }

            
        }
    }
}