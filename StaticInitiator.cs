using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;



public partial class StaticInitiator
{
  static StaticInitiator()
  {
        
        
  }
  
  [MenuItem("GameObject/NodeInfo", false, 0)]
  public static void Foo()
  {
    var gameObject = Selection.activeGameObject;
    UnityEngine.Debug.Log($@"
            CHILD={gameObject.transform.GetComponentsInChildren<Transform>().Length}
            ");
  }


  [MenuItem("Assets/Create/C# Editor Script", priority = 41, validate = false)]
  public static void CreateEditorScript()
  {
    /**
     * 選択したファイルがcsファイルのときのみ起動
     */
    string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
    if (Regex.IsMatch(selectedPath, "\\.cs$"))
    {
      string dir = Path.GetDirectoryName(selectedPath);
      string basename = Path.GetFileNameWithoutExtension(selectedPath);
      if (!Directory.Exists($"{dir}\\Editor"))
      {
        AssetDatabase.CreateFolder(dir, $"Editor");
      }
      var source = new CSSource()
      {
        className = basename
      };

      File.WriteAllText($"{dir}/Editor/{basename}Editor.cs",
        source.ToString(),
        Encoding.UTF8);
      AssetDatabase.Refresh();
    }
    else
    {
      UnityEngine.Debug.LogError($"Not CS File {selectedPath}");
    }

  }


#if UNITY_EDITOR


#if UNITY_STANDALONE_WIN
  [MenuItem("Assets/Open Git Bash", priority = 2, validate = false)]
  public static void BashOpenMenu()
  {
    string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
    string dir = Path.GetDirectoryName(selectedPath);

    var procInfo = new ProcessStartInfo();
    procInfo.UseShellExecute = true;
    procInfo.FileName="C:\\Program Files\\Git\\git-bash.exe";
    procInfo.WorkingDirectory = dir;

    Process.Start(procInfo);
    //C:\Windows\System32\cmd.exe / c "set HOME=D:\GitSettings&"C:\Program Files\Git\bin\sh.exe" --login -i
  }
#endif

#if UNITY_STANDALONE_OSX

  [MenuItem("Assets/Open Terminal", priority = 3, validate = false)]
  public static void BashOpenMenu()
  {
    string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);


    string dir = Path.GetFullPath(
      Directory.Exists(selectedPath) ? selectedPath : Path.GetDirectoryName(selectedPath)
    );

    var procInfo = new ProcessStartInfo();
    UnityEngine.Debug.Log($"open '{dir}' -a Terminal");
    //Process.Start($"open {dir} -a Terminal");


    procInfo.UseShellExecute = true;
    procInfo.FileName = $"open";
    procInfo.Arguments = $"{dir} -a Terminal";

    Process p = Process.Start(procInfo);
    UnityEngine.Debug.Log($"open {p}");
  }

#endif


#endif

  private struct CSSource
  {

    public string className;


    public override string ToString() =>
$@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof({this.className}))]
public class {this.className}Editor:Editor
{{

  public override void OnInspectorGUI()
  {{

    {this.className} target = this.target as {this.className};
    base.OnInspectorGUI();
    if (GUILayout.Button(""Button""))
    {{
      //code
    }}
    
  }}  
}}
";
  }


}
