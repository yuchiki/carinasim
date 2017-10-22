using System;
using System.Collections.Generic;

namespace CarinaSim
{
    public static class Instruction
    {
        public static bool isLittleEndian = true;

        public enum Inst
        {
            dontknow,
            add,
            addi,
            and,
            andi,
            beq,
            bne,
            j,
            jal,
            jr,
            jral,
            lw,
            nor,
            or,
            ori,
            slt,
            slti,
            sll,
            srl,
            sw,
            sub,
            nop,
            hlt,
            input,
            output,
            RformatVIRTUAL,
            FRformatVIRTUAL,
            singleVIRTUAL,
            bclVIRTUAL,
            bclt,
            bclf,
            adds,
            ceqs,
            cles,
            clts,
            muls,
            subs,
            invs,
            lws,
            mult,
            sws,

        }
        ;


        public static readonly Dictionary<UInt32, Inst> InstTable = new Dictionary<uint, Inst>()
        {
            { 0x0, Inst.RformatVIRTUAL },
            { 0x8, Inst.addi },
            { 0xc, Inst.andi },
            { 0x4, Inst.beq },
            { 0x5, Inst.bne },
            { 0x2, Inst.j },
            { 0x3, Inst.jal },
            { 0x23,Inst.lw },
            { 0xd,Inst.ori },
            { 0xa,Inst.slti },
            { 0x2b,Inst.sw },
            { 0x11, Inst.FRformatVIRTUAL },
            { 0x31,Inst.lws },
            { 0x39, Inst.sws },
            { 0x1a, Inst.input },
            { 0x1b, Inst.output },
            { 0x18, Inst.mult },
            { 0x3F, Inst.hlt }
        };

        public static readonly Dictionary<UInt32, Inst> RFormatTable = new Dictionary<uint, Inst>()
        {
            { 0x09, Inst.jral },
            { 0x20, Inst.add },
            { 0x24, Inst.and },
            { 0x08, Inst.jr },
            { 0x27, Inst.nor },
            { 0x25, Inst.or },
            { 0x2a, Inst.slt },
            { 0x00, Inst.sll },
            { 0x02, Inst.srl },
            { 0x22, Inst.sub }
        };

        public static readonly Dictionary<UInt32, Inst> RFFormatTable = new Dictionary<uint, Inst>()
        {
            { 0x10, Inst.singleVIRTUAL },
            { 0x08, Inst.bclVIRTUAL }  
        };

        public static readonly Dictionary<UInt32, Inst> SingleTable = new Dictionary<uint, Inst>()
        {
            { 0x00, Inst.adds },
            { 0x3, Inst.invs },
            { 0x32,Inst.ceqs },
            { 0x3c,Inst.clts },
            { 0x3e,Inst.cles },
            { 0x1,Inst.subs },
            { 0x2,Inst.muls }

        };

        public static Inst Convert(UInt32 inst)
        {
           // var res = InstTable.GetOrDefault(inst.opecode(), Inst.dontknow);
            var res = InstTable[inst.opecode()];
            switch (res)
            {
                case Inst.RformatVIRTUAL:
                    //res = RFormatTable.GetOrDefault(inst.funct(), Inst.dontknow);
                    res = RFormatTable[inst.funct()];
                    break;
                case Inst.FRformatVIRTUAL:
                    //res = RFFormatTable.GetOrDefault(inst.fmt(), Inst.dontknow);
                    res = RFFormatTable[inst.fmt()];
                    if (res == Inst.singleVIRTUAL)
                    {
                        //res = SingleTable.GetOrDefault(inst.funct(), Inst.dontknow);
                        res = SingleTable[inst.funct()];
                    }
                    else if (res == Inst.bclVIRTUAL)
                    {
                        res = (inst.ft() == 1) ? Inst.bclt : Inst.bclf;
                    }
                    break;
                default:
                    break;
            }


            return res;
        }
    }
}

