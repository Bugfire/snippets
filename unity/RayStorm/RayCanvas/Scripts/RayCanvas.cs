// RayCanvas
// https://github.com/Bugfire

using UnityEngine;
using UnityEngine.Assertions;

namespace RayStorm
{
    /// <summary>
    /// RayCanvas.
    /// For Debug-Use, No dynamic heap allocation, Poor APIs
    /// </summary>
    [DefaultExecutionOrder (int.MaxValue - 1)]
    sealed public class RayCanvas : MonoBehaviour
    {
        #region Inspector Settings Values

        [SerializeField]
        Font _Font;

        [SerializeField]
        MeshFilter _MeshFilter;

        [SerializeField]
        int _Width = 640;

        [SerializeField]
        int _Height = 136;

        [SerializeField]
        int _MaxRect = 256;

        [SerializeField]
        bool _ShowMockInEditor = true;

        #endregion

        public int Width { get { return _Width; } }

        public int Height { get { return _Height; } }

        const int FIRST_CHAR = 32;
        const int LAST_CHAR = 127;

        Mesh _Mesh;
        Vector3[] _Vertices;
        Color[] _Colors;
        Vector2[] _UVs;
        int _lastRectIndex;
        bool _IsDirty;

        CharacterInfo[] _CharInfo = new CharacterInfo[LAST_CHAR - FIRST_CHAR];
        int _LineHeight;

        #region Unity Messages

        void Awake ()
        {
            Assert.IsNotNull (_Font, "_Font is not defined on " + RayDebugUtils.GetObjectName (this));
            Assert.IsNotNull (_MeshFilter, "_MeshFilter is not defined on " + RayDebugUtils.GetObjectName (this));

            Setup ();
        }

        void OnValidate ()
        {
            Setup ();
            if (_ShowMockInEditor) {
                FillRect (0, 0, _Width, _Height, new Color (1, 0, 1, 0.5f));
                SetTextPosition (0, 0);
                AddText (
                    "RayCanvas\n" +
                    "Sample Text!\n" +
                    " !\"#$%&'()*+,-./0123456789:;<=>?\n" +
                    "@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_\n" +
                    "`abcdefghijklmnopqrstuvwxyz{|}~\n");
            }
            Update ();
        }

        void Update ()
        {
            if (_IsDirty) {
                _Mesh.vertices = _Vertices;
                _Mesh.colors = _Colors;
                _Mesh.uv = _UVs;
                _Mesh.bounds = _bounds;

                _MeshFilter.mesh = _Mesh;
                _IsDirty = false;
            }
        }

        #endregion

        Bounds _bounds = new Bounds ();
        Color _textColor;
        int _anchorX;
        int _x;
        int _y;
        RayString _formatWork = new RayString (1024);

        public void SetTextColor (Color color)
        {
            _textColor = color;
        }

        public void SetTextPosition (int x, int y)
        {
            _anchorX = _x = x;
            _y = y;
        }

        public void AddText (string message)
        {
            var len = message.Length;
            for (var i = 0; i < len; ++i) {
                WriteChar (message [i]);
            }
        }

        public void AddText (float value, int integerPartLength, int fractionalPartLength)
        {
            _formatWork.Clear ().AddFloat (value, integerPartLength, fractionalPartLength);
            var len = _formatWork.Length;
            var buf = _formatWork.Buffer;
            for (var i = 0; i < len; ++i) {
                WriteChar (buf [i]);
            }
        }

        public void AddText (int value, int minLength = 0)
        {
            _formatWork.Clear ().AddInt (value, minLength);
            var len = _formatWork.Length;
            var buf = _formatWork.Buffer;
            for (var i = 0; i < len; ++i) {
                WriteChar (buf [i]);
            }
        }

        public void FillRect (int sx, int sy, int ex, int ey, Color color)
        {
            var vi = _lastRectIndex * 4;
            sy = _Height - sy;
            ey = _Height - ey;
            _Vertices [vi + 0] = new Vector3 (sx, sy, 0);
            _Vertices [vi + 1] = new Vector3 (ex, sy, 0);
            _Vertices [vi + 2] = new Vector3 (sx, ey, 0);
            _Vertices [vi + 3] = new Vector3 (ex, ey, 0);
            _UVs [vi + 0] = _UVs [vi + 1] = _UVs [vi + 2] = _UVs [vi + 3] = new Vector2 (0, 0);
            _Colors [vi + 0] = _Colors [vi + 1] = _Colors [vi + 2] = _Colors [vi + 3] = color;
            _bounds.Encapsulate (_Vertices [vi + 0]);
            _bounds.Encapsulate (_Vertices [vi + 3]);
            _lastRectIndex++;
            _IsDirty = true;
        }

        public void Clear ()
        {
            ClearBuffer ();
        }

        void WriteChar (char ch)
        {
            if (ch == '\n') {
                _x = _anchorX;
                _y += _LineHeight;
            } else if (ch >= FIRST_CHAR && ch < LAST_CHAR) {
                if (_lastRectIndex >= _MaxRect) {
                    Debug.LogError ("Character buffer overflow on " + RayDebugUtils.GetObjectName (this));
                    return;
                }
                var ci = _CharInfo [(int)ch - FIRST_CHAR];
                if (ch == ' ') {
                    _x += ci.advance;
                    return;
                }
                var vi = _lastRectIndex * 4;
                var x0 = _x + ci.minX;
                var x1 = _x + ci.maxX;
                var y0 = _Height - (_y - ci.minY);
                var y1 = _Height - (_y - ci.maxY);
                _Vertices [vi + 0] = new Vector3 (x0, y0, 0);
                _Vertices [vi + 1] = new Vector3 (x0, y1, 0);
                _Vertices [vi + 2] = new Vector3 (x1, y0, 0);
                _Vertices [vi + 3] = new Vector3 (x1, y1, 0);
                _UVs [vi + 0] = ci.uvBottomLeft;
                _UVs [vi + 1] = ci.uvTopLeft;
                _UVs [vi + 2] = ci.uvBottomRight;
                _UVs [vi + 3] = ci.uvTopRight;
                _Colors [vi + 0] = _Colors [vi + 1] = _Colors [vi + 2] = _Colors [vi + 3] = _textColor;
                _x += ci.advance;
                _bounds.Encapsulate (_Vertices [vi + 0]);
                _bounds.Encapsulate (_Vertices [vi + 3]);
                _lastRectIndex++;
                _IsDirty = true;
            }
        }

        void Setup ()
        {
            for (var i = FIRST_CHAR; i < LAST_CHAR; ++i) {
                CharacterInfo ci;
                if (_Font.GetCharacterInfo ((char)i, out ci)) {
                    _CharInfo [ci.index - FIRST_CHAR] = ci;
                }
            }
            _LineHeight = _Font.lineHeight;

            _Mesh = new Mesh ();
            _Mesh.MarkDynamic ();
            _Mesh.name = "RayCanvas";

            _Vertices = new Vector3[_MaxRect * 4];
            _Colors = new Color[_MaxRect * 4];
            _UVs = new Vector2[_MaxRect * 4];

            var tris = new int[_MaxRect * 6];
            var ti = 0;
            var vi = 0;
            for (var i = 0; i < _MaxRect; ++i) {
                tris [ti + 0] = vi + 0;
                tris [ti + 1] = vi + 1;
                tris [ti + 2] = vi + 2;
                tris [ti + 3] = vi + 2;
                tris [ti + 4] = vi + 1;
                tris [ti + 5] = vi + 3;
                ti += 6;
                vi += 4;
            }

            ClearBuffer ();
            _Mesh.vertices = _Vertices;
            _Mesh.colors = _Colors;
            _Mesh.uv = _UVs;
            _Mesh.triangles = tris;
            _Mesh.bounds = _bounds;

            _MeshFilter.mesh = _Mesh;

            SetTextColor (Color.white);
            SetTextPosition (0, 0);
        }

        void ClearBuffer ()
        {
            System.Array.Clear (_Vertices, 0, _Vertices.Length);
            System.Array.Clear (_Colors, 0, _Colors.Length);
            System.Array.Clear (_UVs, 0, _Colors.Length);
            _bounds.size = Vector3.zero;
            _lastRectIndex = 0;
            _IsDirty = true;
        }
    }

}