using UnityEditor;
using UnityEngine;

namespace Snake
{
    [CustomEditor(typeof(Path))]
    public class LineInspector: Editor
    {
        
        // this is only for testing purposes as long as there is no real snake
        private void OnSceneGUI()
        {
            var line = target as Path;

            if (line.path != null)
            {
                // draw Line
                Handles.color = Color.white;

                for (int i = 0; i < line.path.Count; i++)
                {
                    if (i+1 <= line.path.Count -1)
                    {
                        Handles.DrawLine(line.path[i], line.path[i+1]);
                    }
                }
            }

            
        }
    }
}