using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using Common.Data;
using Editor;

public class  MainEditorWindow:OdinMenuEditorWindow
{


    [MenuItem("Tools/GameEditor")]
    private static void OpenWindow()
    {
        var window = GetWindow<MainEditorWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
    }
    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree(true);
        OdinEquipOverview.Instance.UpdateOverview();
        OdinEquipTable.Instance.Load(OdinEquipOverview.Instance.Equips);
        tree.Add("Equip", OdinEquipTable.Instance, EditorIcons.File) ;
        return tree;
    }

}
