using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Assets.Editor
{
    [CustomEditor(typeof(HyperlinkText), true)]
    class HyperlinkTextEditor:UnityEditor.UI.TextEditor
    {

        public override void OnInspectorGUI()
        {

            base.OnInspectorGUI();
            var t = (HyperlinkText)target;
            t.LinkColor=EditorGUILayout.ColorField("超链接颜色",t.LinkColor);
            t.HoverColor = EditorGUILayout.ColorField("移动颜色", t.HoverColor);

        }

       



    }
}
