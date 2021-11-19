using System;
using UnityEditor;
using UnityEngine;

namespace Nomnom.BitCalculator.Editor {
    public partial class BitCalculator {
        private string _hex;
        private string _dec;
        private string _oct;
        private string _bin;
        private Texture2D _copyIcon;
        
        private void DrawLeftPanelMd(float y, float width) {
            const float HEIGHT = 20;
            
            Rect rect = new Rect(2, y - 13, width - 4, HEIGHT);

            if (!_copyIcon) {
                _copyIcon = Resources.Load<Texture2D>("BitCalculator/CopyIcon");
            }
            
            drawGroup("HEX", _hex);

            if (_hasError) {
                Rect errorRect = rect;
                errorRect.x -= 57;
                
                EditorGUI.LabelField(errorRect, (string) "Invalid Input!", (GUIStyle) _skin.customStyles[5]);
            }

            drawGroup("DEC", _dec);
            drawGroup("OCT", _oct);
            drawGroup("BIN", _bin);

            void drawGroup(string label, string value) {
                rect.y += HEIGHT;

                Rect prefixRect = rect;
                prefixRect.width = 50;

                Rect valueRect = rect;
                valueRect.width = width - prefixRect.width - 20 - 19;
                valueRect.x += prefixRect.width;
                
                Rect btnRect = valueRect;
                btnRect.width = 20;
                btnRect.x += valueRect.width;

                EditorGUI.LabelField(prefixRect, label, (GUIStyle) _skin.customStyles[1]);
                EditorGUI.LabelField(valueRect, value, (GUIStyle) _skin.customStyles[0]);

                GUI.color = Color.white;
                Color bgColor = GUI.backgroundColor;
                GUI.backgroundColor = EditorGUIUtility.isProSkin ? bgColor : Color.black;
                if (GUI.Button(btnRect, new GUIContent(_copyIcon, "Copy"))) {
                    EditorGUIUtility.systemCopyBuffer = value;
                }

                GUI.backgroundColor = bgColor;
            }
        }

        private void UpdateConversions() {
            _hex = _internalValue.ToString("X");
            _dec = _internalValue.ToString();
            _oct = Convert.ToString((long) _internalValue, 8);
            _bin = Convert.ToString((long) _internalValue, 2).PadLeft(64, '0');
        }
    }
}