using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 文本控件,支持超链接
/// </summary>
/// 
public class HyperlinkText : Text, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    /// <summary>
    /// 超链接信息类
    /// </summary>
    public class HyperlinkInfo
    {
        public int startIndex;

        public int endIndex;

        public string name;

        public readonly List<Rect> boxes = new List<Rect>();
    }


    private Color linkColor;


    public Color LinkColor { get => linkColor; set { linkColor = value; SetAllDirty(); } }


    private Color hoverColor;
    public Color HoverColor { get=>hoverColor; set { hoverColor = value;SetAllDirty(); } }
    //GraTest graTest;
    /// <summary>
    /// 解析完最终的文本
    /// </summary>
    private string m_OutputText;

    /// <summary>
    /// 超链接信息列表
    /// </summary>
    private readonly List<HyperlinkInfo> m_HrefInfos = new List<HyperlinkInfo>();

    /// <summary>
    /// 文本构造器
    /// </summary>
    protected static readonly StringBuilder s_TextBuilder = new StringBuilder();

    public EventWraperV1<string> OnLinkClick = new EventWraperV1<string>();


    /// <summary>
    /// 超链接正则
    /// </summary>
    private static readonly Regex s_HrefRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);

    private HyperlinkText mHyperlinkText;


    public string GetHyperlinkInfo
    {
        get { return text; }
    }


    protected override void Awake()
    {
        base.Awake();
        mHyperlinkText = GetComponent<HyperlinkText>();
        //graTest = FindObjectOfType<GraTest>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        //mHyperlinkText.onHrefClick.AddListener(OnHyperlinkTextInfo);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        //mHyperlinkText.onHrefClick.RemoveListener(OnHyperlinkTextInfo);
    }


    
    public override void SetVerticesDirty()
    {
        base.SetVerticesDirty();
#if UNITY_EDITOR
        if (UnityEditor.PrefabUtility.GetPrefabType(this) == UnityEditor.PrefabType.Prefab)
        {
            return;
        }
#endif
        //  m_OutputText = GetOutputText(text);
        text = GetHyperlinkInfo;
        m_OutputText = GetOutputText(text);

    }





    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        var orignText = m_Text;
        m_Text = m_OutputText;
        base.OnPopulateMesh(toFill);
        m_Text = orignText;
        UIVertex vert = new UIVertex();
        // 处理超链接包围框
        foreach (var hrefInfo in m_HrefInfos)
        {
            hrefInfo.boxes.Clear();
            if (hrefInfo.startIndex >= toFill.currentVertCount)
            {
                continue;
            }


            // 将超链接里面的文本顶点索引坐标加入到包围框
            toFill.PopulateUIVertex(ref vert, hrefInfo.startIndex);
            var pos = vert.position;
            var bounds = new Bounds(pos, Vector3.zero);

            for (int i = hrefInfo.startIndex, m = hrefInfo.endIndex; i < m; i++)
            {
                if (i >= toFill.currentVertCount)
                {
                    break;
                }
                
                toFill.PopulateUIVertex(ref vert, i);
                vert.color = linkColor;
                pos = vert.position;
                if (pos.x < bounds.min.x) // 换行重新添加包围框
                {
                    hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                    bounds = new Bounds(pos, Vector3.zero);
                }
                else
                {
                    bounds.Encapsulate(pos); // 扩展包围框
                }
            }
            hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
        }
        //graTest.Draw(m_HrefInfos);
    }

    /// <summary>
    /// 获取超链接解析后的最后输出文本
    /// </summary>
    /// <returns></returns>
    protected virtual string GetOutputText(string outputText)
    {
        s_TextBuilder.Length = 0;
        m_HrefInfos.Clear();
        var indexText = 0;
        var realIndex = 0;
        foreach (Match match in s_HrefRegex.Matches(outputText))
        {
            s_TextBuilder.Append(outputText.Substring(indexText, match.Index - indexText));
            realIndex += match.Index - indexText;

            var group = match.Groups[1];
            var hrefInfo = new HyperlinkInfo
            {
                startIndex = (realIndex) * 4, 
                endIndex = (realIndex + match.Groups[2].Length - 1) * 4 + 3,
                name = group.Value
            };
            m_HrefInfos.Add(hrefInfo);
            
            //s_TextBuilder.Append($"<color=#{ColorUtility.ToHtmlStringRGB(LinkColor)}>");  // 超链接颜色
            s_TextBuilder.Append(match.Groups[2].Value);
            realIndex += match.Groups[2].Length;
            //s_TextBuilder.Append("</color>");
            indexText = match.Index + match.Length;
        }
        s_TextBuilder.Append(outputText.Substring(indexText, outputText.Length - indexText));
        return s_TextBuilder.ToString();
    }

    /// <summary>
    /// 点击事件检测是否点击到超链接文本
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 lp;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out lp);
        foreach (var hrefInfo in m_HrefInfos)
        {
            var boxes = hrefInfo.boxes;
            for (var i = 0; i < boxes.Count; ++i)
            {
                if (boxes[i].Contains(lp))
                {
                    OnLinkClick.Invoke(hrefInfo.name);
                    return;
                }
            }
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        //throw new NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new NotImplementedException();
    }
}

