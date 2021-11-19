using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nomnom.BitCalculator.Editor {
    public static class OperationSolver {
        private static OperandType[] _operandOrder = {
            OperandType.Invert,
            OperandType.Exponent,
            OperandType.MultiplyDivide,
            OperandType.AddSubtract,
            OperandType.LeftShift, OperandType.RightShift,
            OperandType.AND,
            OperandType.XOR,
            OperandType.OR
        };
        
        public static long Solve(List<Operand> operands) {
            if (operands.Count == 1) {
                return long.Parse(operands[0].Value);
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

            if (group.Operands.Count > 3) {
                // solve in order
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
                            }
                            
                            if (rhs is OperandGroup rhsGroup) {
                                rhs = SolveGroup(rhsGroup);
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
                            i -= 2;
                        }
                    }
                }

                return group.Operands[0];
            }

            return SolveTriplet(group.Operands[0], group.Operands[1], group.Operands[2]);
        }

        private static OperandResult SolveTriplet(Operand operandLhs, Operand middle, Operand operandRhs) {
            long lhs = operandLhs is OperandResult lhsResult ? lhsResult.Result : long.Parse(operandLhs.Value);
            long rhs = operandRhs is OperandResult rhsResult ? rhsResult.Result : long.Parse(operandRhs.Value);

            switch (middle.Type) {
                case OperandType.AND: return new OperandResult(lhs & rhs);
                case OperandType.OR: return new OperandResult(lhs | rhs);
                case OperandType.XOR: return new OperandResult(lhs ^ rhs);
                case OperandType.LeftShift: return new OperandResult(lhs << (int)rhs);
                case OperandType.RightShift: return new OperandResult(lhs >> (int)rhs);
                case OperandType.Invert: return new OperandResult(~rhs);
                case OperandType.AddSubtract: return new OperandResult(middle.Value == "+" ? lhs + rhs : lhs - rhs);
                case OperandType.MultiplyDivide: return new OperandResult(middle.Value == "*" ? lhs * rhs : lhs / rhs);
                case OperandType.Exponent: return new OperandResult((long)Mathf.Pow(lhs, rhs));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private class OperandResult : Operand {
            public long Result;

            public OperandResult(long result) : base(OperandType.Result) {
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