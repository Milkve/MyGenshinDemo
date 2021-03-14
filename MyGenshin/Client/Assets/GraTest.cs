using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HyperlinkText;

public class GraTest : Graphic
{
    // Start is called before the first frame update


    List<HyperlinkInfo> minfos;



    public void Draw(List<HyperlinkInfo> hyperlinkInfos  )
    {
        minfos = hyperlinkInfos;
        UpdateGeometry();
    }



    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        vh.Clear();
        if (minfos == null) return;
        var verts = new UIVertex[4];
        //var p0 = new UIVertex();
        for (var i = 0; i < 4; i++)
        {

            verts[i].color = color;
        }

        foreach (var hrefinfo in minfos)
        {
            foreach (var box in hrefinfo.boxes)
            {
                verts[0].position = new Vector3(box.xMin, box.yMin);
                verts[1].position = new Vector3(box.xMax, box.yMin);
                verts[2].position = new Vector3(box.xMax, box.yMax);
                verts[3].position = new Vector3(box.xMin, box.yMax);
                vh.AddUIVertexQuad(verts);
            }
        }
    }
}
