using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Nomnom.BitCalculator.Editor {
    public partial class BitCalculator {
        private Texture2D _refreshIcon;
        private Texture2D _clearIcon;
        private Texture2D _clearNumberIcon;
        
        private void DrawNumberPanel(float y, float width) {
            if (!_refreshIcon) {
                _refreshIcon = Resources.Load<Texture2D>("BitCalculator/RefreshIcon");
            }
            
            if (!_clearIcon) {
                _clearIcon = Resources.Load<Texture2D>("BitCalculator/ClearIcon");
            }
            
            if (!_clearNumberIcon) {
                _clearNumberIcon = Resources.Load<Texture2D>("BitCalculator/ClearNumberIcon");
            }
            
            Rect rect = new Rect(16, y, width - 73 - 40, 60);
            
            EditorGUI.DrawRect(rect, (_hasError ? Color.red : Color.black) * (0.3f + (rect.Contains(Event.current.mousePosition) ? 0.2f : 0)));
            
            string newFormula = EditorGUI.TextField(rect, (string) _internalFormula, (GUIStyle) _skin.customStyles[2]);

            if (newFormula != _internalFormula) {
                _internalFormula = newFormula;
                UpdateConversions();
            }

            Rect runBtnRect = rect;
            runBtnRect.x += rect.width + 5;
            runBtnRect.width = 35;
            
            Rect clearBtnRect = runBtnRect;
            clearBtnRect.x += 40;
            clearBtnRect.height /= 2;
            
            Rect clearNumberBtnRect = clearBtnRect;
            clearNumberBtnRect.y += clearBtnRect.height;

            Color bgColor = GUI.backgroundColor;
            GUI.backgroundColor = EditorGUIUtility.isProSkin ? bgColor : Color.black;
            if (GUI.Button(clearBtnRect, new GUIContent(_clearIcon, "Clear formula"))) {
                _internalFormula = string.Empty;
            }
            
            GUI.backgroundColor = EditorGUIUtility.isProSkin ? bgColor : Color.black;
            if (GUI.Button(clearNumberBtnRect, new GUIContent(_clearNumberIcon, "Clear number"))) {
                _internalValue = 0;
                UpdateConversions();
            }

            GUI.backgroundColor = EditorGUIUtility.isProSkin ? bgColor : Color.black;
            if (GUI.Button(runBtnRect, new GUIContent(_refreshIcon, "Run formula"))) {
                // run formula conversion
                try {
                    string formula = _internalFormula.Replace(" ", string.Empty);
                    StringBuilder sb = new StringBuilder();

                    List<Operand> operands = new List<Operand>();

                    for (int i = 0; i < formula.Length; i++) {
                        char iChar = formula[i];

                        if (char.IsNumber(iChar)) {
                            // collect number
                            sb.Clear();

                            bool gottenNumber = false;

                            for (int j = i; j < formula.Length; j++) {
                                char jChar = formula[j];

                                if (!char.IsNumber(jChar)) {
                                    // done
                                    i = j - 1;

                                    operands.Add(new Operand(OperandType.Number, sb.ToString()));

                                    sb.Clear();
                                    gottenNumber = true;
                                    break;
                                }

                                sb.Append(jChar);
                            }

                            if (!gottenNumber) {
                                string final = sb.ToString();
                                operands.Add(new Operand(OperandType.Number, final));

                                sb.Clear();
                                i = formula.Length - 1;
                            }

                            continue;
                        }

                        switch (iChar) {
                            case '&':
                                operands.Add(new Operand(OperandType.AND));
                                break;
                            case '<':
                                // make sure there are two
                                if (formula[i + 1] == '<') {
                                    operands.Add(new Operand(OperandType.LeftShift));
                                    i++;
                                }
                                else {
                                    throw new Exception("< is not accompanied by another <");
                                }

                                break;
                            case '>': 
                                // make sure there are two
                                if (formula[i + 1] == '>') {
                                    operands.Add(new Operand(OperandType.RightShift));
                                    i++;
                                }
                                else {
                                    throw new Exception("> is not accompanied by another >");
                                }
                                
                                break;
                            case '~':
                                // get group
                                operands.Add(new Operand(OperandType.Number, "0"));
                                operands.Add(new Operand(OperandType.Invert));
                                break;
                            case '(':
                                // get group
                                operands.Add(new Operand(OperandType.StartingGroup));
                                break;
                            case ')':
                                operands.Add(new Operand(OperandType.EndingGroup));
                                break;
                            case '^':
                                operands.Add(new Operand(OperandType.XOR));
                                break;
                        }
                    }

                    // create operations
                    int output = OperationSolver.Solve(operands);
                    
                    _internalValue = output;
                    UpdateConversions();

                    _hasError = false;
                } catch (Exception _) {
                    // Debug.LogError(_);
                    _hasError = true;
                }
            }

            GUI.backgroundColor = bgColor;
        }
    }
}