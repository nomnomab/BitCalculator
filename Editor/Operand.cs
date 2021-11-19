namespace Nomnom.BitCalculator.Editor {
    public class Operand {
        public readonly OperandType Type;
        public readonly string Value;
        
        public Operand(OperandType type) {
            Type = type;
            Value = null;
        }
        
        public Operand(OperandType type, string value) {
            Type = type;
            Value = value;
        }
    }
}