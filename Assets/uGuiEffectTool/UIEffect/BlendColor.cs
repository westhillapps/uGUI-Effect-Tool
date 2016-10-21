/*
uGui-Effect-Tool
Copyright (c) 2016 WestHillApps (Hironari Nishioka)
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UiEffect
{
    [AddComponentMenu("UI/Effects/Blend Color"), RequireComponent(typeof(Graphic))]
    public class BlendColor : BaseMeshEffect
    {
        public enum BlendMode
        {
            Multiply,
            Additive,
            Subtractive,
            Override,
        }

        [SerializeField]
        private BlendMode m_blendMode = BlendMode.Multiply;
        [SerializeField]
        private Color m_color = Color.white;

        public BlendMode blendMode { get { return m_blendMode; } set { if (m_blendMode != value) { m_blendMode = value; Refresh(); } } }
        public Color color { get { return m_color; } set { if (m_color != value) { m_color = value; Refresh(); } } }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (IsActive() == false)
            {
                return;
            }

            List<UIVertex> vList = UiEffectListPool<UIVertex>.Get();

            vh.GetUIVertexStream(vList);

            ModifyVertices(vList);

            vh.Clear();
            vh.AddUIVertexTriangleStream(vList);

            UiEffectListPool<UIVertex>.Release(vList);
        }

        private void ModifyVertices(List<UIVertex> vList)
        {
            if (IsActive() == false || vList == null || vList.Count == 0)
            {
                return;
            }

            UIVertex newVertex;
            for (int i = 0; i < vList.Count; i++)
            {
                newVertex = vList[i];
                byte orgAlpha = newVertex.color.a;
                switch (m_blendMode)
                {
                    case BlendMode.Multiply:
                        newVertex.color *= m_color;
                        break;
                    case BlendMode.Additive:
                        newVertex.color += m_color;
                        break;
                    case BlendMode.Subtractive:
                        newVertex.color -= m_color;
                        break;
                    case BlendMode.Override:
                        newVertex.color = m_color;
                        break;
                    default:
                        break;
                }
                newVertex.color.a = orgAlpha;
                vList[i] = newVertex;
            }
        }

        private void Refresh()
        {
            if (graphic != null)
            {
                graphic.SetVerticesDirty();
            }
        }
    }
}
