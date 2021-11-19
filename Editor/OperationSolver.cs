using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nomnom.BitCalculator.Editor {
    public static class OperationSolver {
        private static OperandType[] _operandOrder = {
            OperandType.Invert,
            OperandType.LeftShift, OperandType.RightShift,
            OperandType.AND,
            OperandType.XOR,
            OperandType.OR
        };
        
        public static int Solve(List<Operand> operands) {
            if (operands.Count == 1) {
                return int.Parse(operands[0].Value);
            }
            
            // need to collect group items first
            List<Operand> finalOperands = new List<Operand>();
            
            for (int i = 0; i < operands.Count; i++) {
                Operand operand = operands[i];

                if (operand.Type != OperandType.StartingGroup) {
                    finalOperands.Add(operand);
                    continue;
                }

                int start = i + 1;
                finalOperands.Add(spawnGroup(ref start));

                i += start - i - 1;

                OperandGroup spawnGroup(ref int start) {
                    OperandGroup group = new OperandGroup();

                    for (int i = start; i < operands.Count; i++) {
                        Operand operand = operands[i];
                        
                        // if we find a closer, then we are gucchi
                        if (operand.Type == OperandType.EndingGroup) {
                            // collect group
                            start++;
                            return group;
                        }
                        
                        if (operand.Type == OperandType.StartingGroup) {
                            // uh, recursive group collection time baby
                            start++;
                            group.Operands.Add(spawnGroup(ref start));

                            i += start - i - 1;
                        }
                        else {
                            group.Operands.Add(operand);
                        }

                        start++;
                    }

                    return group;
                }
            }
            
            // solve groups first
            OperandGroup rootGroup = new OperandGroup();
            foreach (Operand finalOperand in finalOperands) {
                rootGroup.Operands.Add(finalOperand);
            }
            OperandResult result = (OperandResult)SolveGroup(rootGroup);
            
            return result.Result;
            
            // for (int i = 0; i < finalOperands.Count; i++) {
            //     if (finalOperands[i] is OperandGroup group) {
            //         finalOperands[i] = SolveGroup(group);
            //     }
            //     
            //     Debug.Log($"[{i}] {finalOperands[i]?.Type}");
            // }
        }

        private static Operand SolveGroup(OperandGroup group) {
            // check children for groups
            for (int i = 0; i < group.Operands.Count; i++) {
                Operand operand = group.Operands[i];

                if (operand is OperandGroup newGroup) {
                    // replace with result
                    group.Operands[i] = SolveGroup(newGroup);
                }
            }
            
            // evaluate
            // Debug.Log($"{group.Operands[0]?.Type} {group.Operands[1]?.Type} {group.Operands[2]?.Type}");

            if (group.Operands.Count > 3) {
                // solve in order
                // sort
                foreach (OperandType ty in _operandOrder) {
                    for (int i = 0; i < group.Operands.Count; i++) {
                        if (i < 0) {
                            break;
                        }
                        
                        Operand operand = group.Operands[i];

                        if (operand.Type == ty) {
                            Operand lhs = group.Operands[i - 1];
                            Operand rhs = group.Operands[i + 1];

                            if (lhs is OperandGroup lhsGroup) {
                                lhs = SolveGroup(lhsGroup);
                                OperandResult rslt = lhs as OperandResult;
                                Debug.Log(rslt?.Result);
                            }
                            
                            if (rhs is OperandGroup rhsGroup) {
                                rhs = SolveGroup(rhsGroup);
                                OperandResult rslt = rhs as OperandResult;
                                Debug.Log(rslt?.Result);
                            }
                            
                            // found solve this batch
                            OperandResult result = SolveTriplet(lhs, operand, rhs);

                            // nuke the triple
                            int index1 = i - 1;
                            group.Operands.Insert(index1, result);

                            group.Operands.RemoveAt(i);
                            group.Operands.RemoveAt(i);
                            group.Operands.RemoveAt(i);

                            // go to the next item
                            i -= 3;
                        }
                    }
                }

                return group.Operands[0];
            }

            return SolveTriplet(group.Operands[0], group.Operands[1], group.Operands[2]);
        }

        private static OperandResult SolveTriplet(Operand operandLhs, Operand middle, Operand operandRhs) {
            int lhs = operandLhs is OperandResult lhsResult ? lhsResult.Result : int.Parse(operandLhs.Value);
            int rhs = operandRhs is OperandResult rhsResult ? rhsResult.Result : int.Parse(operandRhs.Value);

            switch (middle.Type) {
                case OperandType.AND: return new OperandResult(lhs & rhs);
                case OperandType.OR: return new OperandResult(lhs | rhs);
                case OperandType.XOR: return new OperandResult(lhs ^ rhs);
                case OperandType.LeftShift: return new OperandResult(lhs << rhs);
                case OperandType.RightShift: return new OperandResult(lhs >> rhs);
                case OperandType.Invert: return new OperandResult(~rhs);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private class OperandResult : Operand {
            public int Result;

            public OperandResult(int result) : base(OperandType.Result) {
                Result = result;
            }
        }

        private class OperandGroup: Operand {
            public List<Operand> Operands;

            public OperandGroup() : base(OperandType.Group) {
                Operands = new List<Operand>();
            }
        }
    }
}