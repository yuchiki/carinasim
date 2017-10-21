using System;
using System.Collections.Generic;


namespace CarinaSim
{
    public static class DictionaryExtensions
    {
        public static TV GetOrDefault<TK, TV>(this Dictionary<TK, TV> dic, TK key, TV defaultValue = default(TV))
        {
            TV result;
            return dic.TryGetValue(key, out result) ? result : defaultValue;
        }
    }

    public static class UInt32Extensions
    {
        public static UInt32 opecode(this UInt32 inst) => inst >> 26;
        public static UInt32 funct(this UInt32 inst) => inst & 0b111111;
        public static UInt32 rs(this UInt32 inst) => (inst >> 21) & 0b11111;
        public static UInt32 rt(this UInt32 inst) => (inst >> 16) & 0b11111;
        public static UInt32 rd(this UInt32 inst) => (inst >> 11) & 0b11111;
        public static UInt32 shamt(this UInt32 inst) => (inst >> 6) & 0b11111;
        public static UInt32 immediate(this UInt32 inst) => inst & 0xFFFF;
        public static UInt32 address(this UInt32 inst) => inst & 0b1111_1111_1111_1111_1111_1111_11;
        public static UInt32 fmt(this UInt32 inst) => (inst >> 21) & 0b11111;
        public static UInt32 ft(this UInt32 inst) => (inst >> 16) & 0b11111;
        public static UInt32 fs(this UInt32 inst) => (inst >> 11) & 0b11111;
        public static UInt32 fd(this UInt32 inst) => (inst >> 6) & 0b11111;
        public static UInt32 signExtIm(this UInt32 inst) => (inst.immediate() >= 0x8000) ? 0xFFFF0000 + inst.immediate() : inst.immediate();
        public static UInt32 zeroExtIm(this UInt32 inst) => inst.immediate();
        public static UInt32 branchAddr(this UInt32 inst) {
            var imm = inst.immediate();
            if (imm >= 0x8000)
            {
                return 0xFFFC0000 + (imm - 0x8000) << 2;
            }
            else
            {
                return imm << 2;
            }
        }
        public static UInt32 jumpAddr(this UInt32 inst) => inst.immediate() << 2;

        public static float toSingle(this UInt32 inst, bool isLittleEndian){
            byte[] bytes = BitConverter.GetBytes(inst);
            if (BitConverter.IsLittleEndian != isLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToSingle(bytes,0);
        }
    }

    public static class FloatExtensions{
        public static UInt32 ToUInt32(this float f, bool isLittleEndian){
            byte[] bytes = BitConverter.GetBytes(f);
            if (BitConverter.IsLittleEndian != isLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToUInt32(bytes,0);
        }
    }
}

