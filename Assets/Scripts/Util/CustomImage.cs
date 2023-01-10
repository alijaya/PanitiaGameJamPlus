using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("UI/Extensions/Primitives/Custom Image")]
    public class CustomImage : UIPrimitiveBase
    {
        public static int QuadLimit = 5000;
        // css mode
        public enum Mode
        {
            None,
            Repeat,
            Space,
            Round,
        }

        public Mode modeX = Mode.Repeat;
        public Mode modeY = Mode.Repeat;

        public bool useBorder = false;
        public float widthFill = 1;
        public float heightFill = 1;
        public Vector2 tilePivot = Vector2.zero;

        public bool useCrop
        {
            get
            {
                return widthFill != 1 || heightFill != 1;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var rect = rectTransform.rect;

            var rectSprite = activeSprite.rect;

            if (useBorder)
            {
                var border = activeSprite.border;
                rectSprite.xMin += border.x;
                rectSprite.yMin += border.y;
                rectSprite.xMax -= border.z;
                rectSprite.yMax -= border.w;
            }

            if (useCrop)
            {
                var cropRect = new Rect((1 - widthFill)/2, (1 - heightFill)/2, widthFill, heightFill);

                rectSprite = multiplyUV(cropRect, rectSprite);
            }

            var rectUV = getUV(rectSprite, activeSprite.rect);

            var countX = 1;
            var countY = 1;

            var clipX = false;
            var clipY = false;

            var cellSize = rectSprite.size / pixelsPerUnit;
            var pivotOffset = tilePivot * (rect.size - cellSize);
            var space = cellSize;

            if (modeX == Mode.Repeat)
            {
                var minCountFX = pivotOffset.x / cellSize.x;
                var maxCountFX = (rect.width - pivotOffset.x) / cellSize.x;
                countX = Mathf.CeilToInt(maxCountFX) + Mathf.CeilToInt(minCountFX);

                pivotOffset.x = (pivotOffset.x % cellSize.x);
                if (pivotOffset.x > 0) pivotOffset.x -= cellSize.x;

                clipX = true;
            } else if (modeX == Mode.Space)
            {
                countX = Mathf.FloorToInt(rect.width / cellSize.x);
                if (countX <= 0) countX = 1;
                if (countX > 1)
                {
                    pivotOffset.x = 0;
                    space.x = (rect.width - cellSize.x) / (countX - 1);
                }
            } else if (modeX == Mode.Round)
            {
                countX = Mathf.RoundToInt(rect.width / cellSize.x);
                if (countX <= 0) countX = 1;

                pivotOffset.x = 0;
                space.x = rect.width / countX;
                cellSize.x = rect.width / countX;
            }

            if (modeY == Mode.Repeat)
            {
                var minCountFY = pivotOffset.y / cellSize.y;
                var maxCountFY = (rect.height - pivotOffset.y) / cellSize.y;
                countY = Mathf.CeilToInt(maxCountFY) + Mathf.CeilToInt(minCountFY);

                pivotOffset.y = (pivotOffset.y % cellSize.y);
                if (pivotOffset.y > 0) pivotOffset.y -= cellSize.y;

                clipY = true;
            } else if (modeY == Mode.Space)
            {
                countY = Mathf.FloorToInt(rect.height / cellSize.y);
                if (countY <= 0) countY = 1;
                if (countY > 1)
                {
                    pivotOffset.y = 0;
                    space.y = (rect.height - cellSize.y) / (countY - 1);
                }
            }
            else if (modeY == Mode.Round)
            {
                countY = Mathf.FloorToInt(rect.height / cellSize.y);
                if (countY <= 0) countY = 1;

                pivotOffset.y = 0;
                space.y = rect.height / countY;
                cellSize.y = rect.height / countY;
            }

            if (countX * countY > QuadLimit) return;

            for (var iy = 0; iy < countY; iy++)
            {
                for (var ix = 0; ix < countX; ix++)
                {
                    var localOffset = new Vector2(ix * space.x, iy * space.y);
                    var min = rect.min + pivotOffset + localOffset;
                    var max = min + cellSize;
                    var cellRect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);

                    var clampRect = cellRect;

                    if (clipX)
                    {
                        clampRect.xMin = Mathf.Max(min.x, rect.xMin);
                        clampRect.xMax = Mathf.Min(max.x, rect.xMax);
                    }
                    if (clipY)
                    {
                        clampRect.yMin = Mathf.Max(min.y, rect.yMin);
                        clampRect.yMax = Mathf.Min(max.y, rect.yMax);
                    }

                    var clampUV = getUV(clampRect, cellRect);
                    clampUV = multiplyUV(clampUV, rectUV);


                    addRect(vh, color, clampRect.min, clampRect.max, clampUV.min, clampUV.max);
                    //addRect(vh, color, cellRect.min, cellRect.max, rectUV.min, rectUV.max);
                }
            }
        }

        public static void addRect(VertexHelper vh, Color color, Vector2 minPos, Vector2 maxPos, Vector2 minUV, Vector2 maxUV)
        {
            if (maxPos.x <= minPos.x || maxPos.y <= minPos.y) return;
            vh.AddUIVertexQuad(SetVbo(color,
                new[] { minPos, new Vector2(minPos.x, maxPos.y), maxPos, new Vector2(maxPos.x, minPos.y) },
                new[] { minUV, new Vector2(minUV.x, maxUV.y), maxUV, new Vector2(maxUV.x, minUV.y) }));
        }

        public static Rect multiplyUV(Rect uvA, Rect uvB)
        {
            var result = Rect.zero;
            result.min = uvA.min * uvB.size + uvB.min;
            result.max = uvA.max * uvB.size + uvB.min;
            return result;
        }

        public static Rect getUV(Rect modifiedRect, Rect originalRect)
        {
            var result = Rect.zero;
            result.min = (modifiedRect.min - originalRect.min) / originalRect.size;
            result.max = (modifiedRect.max - originalRect.min) / originalRect.size;
            return result;
        }

        public static UIVertex[] SetVbo(Color color, Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] vbo = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                var vert = UIVertex.simpleVert;
                vert.color = color;
                vert.position = vertices[i];
                vert.uv0 = uvs[i];
                vbo[i] = vert;
            }
            return vbo;
        }

    }
}