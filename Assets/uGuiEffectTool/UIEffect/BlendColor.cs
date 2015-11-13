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
    [AddComponentMenu("UI/Effects/Blend Color"), RequireComponent(typeof(Graphic))]
#if USE_BASE_VERTEX_EFFECT
    public class BlendColor : BaseVertexEffect
#else
    public class BlendColor : BaseMeshEffect
#endif
    {
        public enum BLEND_MODE
        {
            Multiply,
            Additive,
            Subtractive,
            Override,
        }

        public BLEND_MODE blendMode = BLEND_MODE.Multiply;
        public Color color = Color.grey;

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

            UIVertex tempVertex = vList[0];
            for (int i = 0; i < vList.Count; i++) {
                tempVertex = vList[i];
                byte orgAlpha = tempVertex.color.a;
                switch (blendMode) {
                    case BLEND_MODE.Multiply:
                        tempVertex.color *= color;
                        break;
                    case BLEND_MODE.Additive:
                        tempVertex.color += color;
                        break;
                    case BLEND_MODE.Subtractive:
                        tempVertex.color -= color;
                        break;
                    case BLEND_MODE.Override:
                        tempVertex.color = color;
                        break;
                }
                tempVertex.color.a = orgAlpha;
                vList[i] = tempVertex;
            }
        }

        /// <summary>
        /// Refresh Blend Color on playing.
        /// </summary>
        public void Refresh ()
        {
            if (graphic != null) {
                graphic.SetVerticesDirty();
            }
        }
    }
}
