/*
uGui-Effect-Tool
Copyright (c) 2015 WestHillApps (Hironari Nishioka)
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

// If you are using Unity5.2.1p1 ~ p4, please remove the "UNITY_5_2_1" definition.
#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2_1
#define USE_BASE_VERTEX_EFFECT
#endif

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UiEffect
{
    [AddComponentMenu("UI/Effects/Gradient Color"), RequireComponent(typeof(Graphic))]
#if USE_BASE_VERTEX_EFFECT
    public class GradientColor : BaseVertexEffect
#else
    public class GradientColor : BaseMeshEffect
#endif
    {
        public enum DIRECTION
        {
            Vertical,
            Horizontal,
            Both,
        }

        public DIRECTION direction = DIRECTION.Both;
        public Color colorTop = Color.white;
        public Color colorBottom = Color.black;
        public Color colorLeft = Color.red;
        public Color colorRight = Color.blue;

#if USE_BASE_VERTEX_EFFECT
        public override void ModifyVertices (List<UIVertex> vList)
#else
        public override void ModifyMesh (VertexHelper vh)
        {
            if (IsActive() == false) {
                return;
            }

            var vList = new List<UIVertex>();
            vh.GetUIVertexStream(vList);

            ModifyVertices(vList);

            vh.Clear();
            vh.AddUIVertexTriangleStream(vList);
        }

        public void ModifyVertices (List<UIVertex> vList)
#endif
        {
            if (IsActive() == false || vList == null || vList.Count == 0) {
                return;
            }

            float topX = 0f, topY = 0f, bottomX = 0f, bottomY = 0f;

            for (int i = 0; i < vList.Count; i++) {
                var vertex = vList[i];
                topX = Mathf.Max(topX, vertex.position.x);
                topY = Mathf.Max(topY, vertex.position.y);
                bottomX = Mathf.Min(bottomX, vertex.position.x);
                bottomY = Mathf.Min(bottomY, vertex.position.y);
            }

            float width = topX - bottomX;
            float height = topY - bottomY;

            UIVertex tempVertex = vList[0];
            for (int i = 0; i < vList.Count; i++) {
                tempVertex = vList[i];
                byte orgAlpha = tempVertex.color.a;
                Color colorOrg = tempVertex.color;
                Color colorV = Color.Lerp(colorBottom, colorTop, (tempVertex.position.y - bottomY) / height);
                Color colorH = Color.Lerp(colorLeft, colorRight, (tempVertex.position.x - bottomX) / width);
                switch (direction) {
                    case DIRECTION.Both:
                        tempVertex.color = colorOrg * colorV * colorH;
                        break;
                    case DIRECTION.Vertical:
                        tempVertex.color = colorOrg * colorV;
                        break;
                    case DIRECTION.Horizontal:
                        tempVertex.color = colorOrg * colorH;
                        break;
                }
                tempVertex.color.a = orgAlpha;
                vList[i] = tempVertex;
            }
        }

        /// <summary>
        /// Refresh Gradient Color on playing.
        /// </summary>
        public void Refresh ()
        {
            if (graphic != null) {
                graphic.SetVerticesDirty();
            }
        }
    }
}
