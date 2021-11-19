using UnityEditor;
using UnityEngine;

namespace Nomnom.BitCalculator.Editor {
    public partial class BitCalculator: EditorWindow {
        [MenuItem("Tools/Nomnom/Bit Calculator")]
        private static void Init() {
            BitCalculator calculator = GetWindow<BitCalculator>("Calculator");
            calculator.minSize = calculator.maxSize = new Vector2(620, 450);
            calculator.Show();
        }

        private string _internalFormula;
        private long _internalValue = 12;
        private bool _hasError;
        private GUISkin _skin;
        private Event _event;

        private void OnEnable() {
            if (!_skin) {
                _skin = Resources.Load<GUISkin>("BitCalculator/BitSkin");
            }
            
            UpdateConversions();
        }

        private void OnGUI() {
            float intensity = EditorGUIUtility.isProSkin ? 0.2f : 0.75f;
            EditorGUI.DrawRect(new Rect(Vector2.zero, position.size), Color.black * intensity);
            Repaint();
            
            DrawNumberPanel(0, position.size.x);
            DrawLeftPanelMd(60, position.size.x);
            DrawBitPanel(155, position.size.x);
        }
    }
}