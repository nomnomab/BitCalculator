using UnityEditor;
using UnityEngine;

namespace Nomnom.BitCalculator.Editor {
    public partial class BitCalculator {
        private void DrawBitPanel(float y, float width) {
            Rect rect = new Rect(16, y, width - 32, position.size.y - y - 16);
            EditorGUI.DrawRect(rect, Color.black * 0.3f);
            GUIStyle bitBtnStyle = _skin.customStyles[3];
            GUIStyle bitGroupStyle = _skin.customStyles[4];
            
            // draw bits
            GUILayout.BeginArea(rect);
            {
                EditorGUILayout.BeginVertical();
                {
                    drawBitRow(3);
                    drawBitRow(2);
                    drawBitRow(1);
                    drawBitRow(0);
                }
                EditorGUILayout.EndVertical();
            }
            GUILayout.EndArea();

            // draw 4 bits + offset
            void drawBitRow(int startGroup) {
                startGroup *= 4;
                EditorGUILayout.BeginHorizontal();
                {
                    drawBitGroup(startGroup + 3);
                    drawBitGroup(startGroup + 2);
                    drawBitGroup(startGroup + 1);
                    drawBitGroup(startGroup + 0);
                }
                EditorGUILayout.EndHorizontal();
            }
            
            void drawBitGroup(int group) {
                int offset = group * 4;
                float elementWidth = (rect.width - 36f) / 16f;
                float elementHeight = rect.height / 6f - 1;
                GUILayoutOption elementWidthOption = GUILayout.Width(elementWidth);
                GUILayoutOption elementHeightOption = GUILayout.Height(elementHeight);

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        for (int i = 0; i < 4; i++) {
                            long shift = (long)1 << (offset + (4 - i - 1));
                            long bit = _internalValue & shift;
                            bool one = bit != 0;

                            GUI.color = one ? Color.green : Color.white;
                            
                            if (GUILayout.Button(one ? "1" : "0", bitBtnStyle, elementWidthOption, elementHeightOption)) {
                                // flip bit
                                _internalValue ^= shift;
                                UpdateConversions();
                            }
                        }
                        
                        GUI.color = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.LabelField(offset.ToString(), bitGroupStyle, GUILayout.Width(elementWidth * 4));
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
}