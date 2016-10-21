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
    [AddComponentMenu("UI/Effects/Gradient Alpha"), RequireComponent(typeof(Graphic))]
    public class GradientAlpha : BaseMeshEffect
    {
        private const int ONE_TEXT_VERTEX = 6;

        [SerializeField, Range(0f, 1f)]
        private float m_alphaTop = 1f;
        [SerializeField, Range(0f, 1f)]
        private float m_alphaBottom = 1f;
        [SerializeField, Range(0f, 1f)]
        private float m_alphaLeft = 1f;
        [SerializeField, Range(0f, 1f)]
        private float m_alphaRight = 1f;
        [SerializeField, Range(-1f, 1f)]
        private float m_gradientOffsetVertical = 0f;
        [SerializeField, Range(-1f, 1f)]
        private float m_gradientOffsetHorizontal = 0f;
        [SerializeField]
        private bool m_splitTextGradient = false;

        public float alphaTop { get { return m_alphaTop; } set { if (m_alphaTop != value) { m_alphaTop = Mathf.Clamp01(value); Refresh(); } } }
        public float alphaBottom { get { return m_alphaBottom; } set { if (m_alphaBottom != value) { m_alphaBottom = Mathf.Clamp01(value); Refresh(); } } }
        public float alphaLeft { get { return m_alphaLeft; } set { if (m_alphaLeft != value) { m_alphaLeft = Mathf.Clamp01(value); Refresh(); } } }
        public float alphaRight { get { return m_alphaRight; } set { if (m_alphaRight != value) { m_alphaRight = Mathf.Clamp01(value); Refresh(); } } }
        public float gradientOffsetVertical { get { return m_gradientOffsetVertical; } set { if (m_gradientOffsetVertical != value) { m_gradientOffsetVertical = Mathf.Clamp(value, -1f, 1f); Refresh(); } } }
        public float gradientOffsetHorizontal { get { return m_gradientOffsetHorizontal; } set { if (m_gradientOffsetHorizontal != value) { m_gradientOffsetHorizontal = Mathf.Clamp(value, -1f, 1f); Refresh(); } } }
        public bool splitTextGradient { get { return m_splitTextGradient; } set { if (m_splitTextGradient != value) { m_splitTextGradient = value; Refresh(); } } }

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

            float minX = 0f, minY = 0f, maxX = 0f, maxY = 0f, width = 0f, height = 0;

            UIVertex newVertex;
            for (int i = 0; i < vList.Count; i++)
            {
                if (i == 0 || (m_splitTextGradient && i % ONE_TEXT_VERTEX == 0))
                {
                    minX = vList[i].position.x;
                    minY = vList[i].position.y;
                    maxX = vList[i].position.x;
                    maxY = vList[i].position.y;

                    int vertNum = m_splitTextGradient ? i + ONE_TEXT_VERTEX : vList.Count;

                    for (int k = i; k < vertNum; k++)
                    {
                        if (k >= vList.Count)
                        {
                            break;
                        }
                        UIVertex vertex = vList[k];
                        minX = Mathf.Min(minX, vertex.position.x);
                        minY = Mathf.Min(minY, vertex.position.y);
                        maxX = Mathf.Max(maxX, vertex.position.x);
                        maxY = Mathf.Max(maxY, vertex.position.y);
                    }

                    width = maxX - minX;
                    height = maxY - minY;
                }

                newVertex = vList[i];

                float alphaOriginal = newVertex.color.a / 255f;
                float alphaVertical = Mathf.Lerp(m_alphaBottom, m_alphaTop, (height > 0 ? (newVertex.position.y - minY) / height : 0) + m_gradientOffsetVertical);
                float alphaHorizontal = Mathf.Lerp(m_alphaLeft, m_alphaRight, (width > 0 ? (newVertex.position.x - minX) / width : 0) + m_gradientOffsetHorizontal);

                newVertex.color.a = (byte)(Mathf.Clamp01(alphaOriginal * alphaVertical * alphaHorizontal) * 255);

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
