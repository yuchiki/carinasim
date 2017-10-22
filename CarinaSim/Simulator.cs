using System;
using System.Linq;
using System.Collections.Generic;

namespace CarinaSim
{
    public class Simulator
    {
        public readonly MipsProgram Program;
        const int STACKWORDS = 0x0010_0000;
        List<UInt32> Mem;
        List<Instruction.Inst> Insts;


        UInt32[] GPR = new UInt32[32];
        float[] FPR = new float[32];
        bool FPCond = false;
        public UInt32 PC = 0;
        public int DynamicCounts = 0;

        public Simulator(MipsProgram program)
        {
            Program = program;
        }

        public string FormattedStatus()
        {
            var inst = Mem[(int)(PC >> 2)];
            return $"{PC:X4}:{inst.opecode():X2}={Instruction.Convert(inst),4}, at:{GPR[1]:X8} t0:{GPR[8]:X8} t8:{GPR[24]:X8} t9:{GPR[25]:X8} f0:{FPR[0]} f1:{FPR[1]} f2:{FPR[2]} f31:{FPR[31]}";
        }
        public void Initialize()
        {
            PC = Program.Info.entrypoint;
            GPR[29] = Program.Info.stackoffset;
            Mem = new List<UInt32>((int)((STACKWORDS) ));
            Mem.AddRange(Program.program);
            Mem.AddRange(Enumerable.Repeat((UInt32)0x0000DEAD, STACKWORDS));

            Insts = new List<Instruction.Inst>((int)Program.Info.textsize);
            for (int i = 0; i < (int)(Program.Info.textsize >> 2); i++)
            {
                Insts.Add(Instruction.Convert(Mem[(int)((Program.Info.textoffset >> 2) + i)]));
            }
        }

        public bool DoStep()
        {
            DynamicCounts++;
            GPR[0] = 0;
            UInt32 nextPC = PC + 4;
            var inst = Mem[(int)(PC >> 2)];
          //  Console.Error.Write(Instruction.Convert(inst));
          //  Console.Error.Write(Insts[(int)(PC >> 2)]);
          //  Console.Read();
            switch (Insts[(int)(PC >> 2)])
            {
                case Instruction.Inst.add:
                    GPR[inst.rd()] = GPR[inst.rs()] + GPR[inst.rt()];
                    break;
                case Instruction.Inst.addi:
                    GPR[inst.rt()] = GPR[inst.rs()] + inst.signExtIm();
                    break;
                case Instruction.Inst.and:
                    GPR[inst.rd()] = GPR[inst.rs()] & GPR[inst.rt()];
                    break;
                case Instruction.Inst.andi:
                    GPR[inst.rt()] = GPR[inst.rs()] & inst.zeroExtIm();
                    break;
                case Instruction.Inst.beq:
                    if (GPR[inst.rs()] == GPR[inst.rt()])
                        nextPC += inst.branchAddr();
                
                    break;
                case Instruction.Inst.bne:
                    if (GPR[inst.rs()] != GPR[inst.rt()])
                        nextPC += inst.branchAddr();                   
                    break;
                case Instruction.Inst.j:
                    nextPC = inst.jumpAddr();
                    break;
                case Instruction.Inst.jal:
                    GPR[31] = nextPC;
                    //Console.Read();
                    nextPC = inst.jumpAddr();
                    break;
                case Instruction.Inst.jr:
                   // Console.Read();
                    nextPC = GPR[inst.rs()]; 
                    break;
                case Instruction.Inst.jral:
                    GPR[31] = nextPC;
                    nextPC = GPR[inst.rs()];
                    break;
                case Instruction.Inst.lw:
                    GPR[inst.rt()] = Mem[(int)((GPR[inst.rs()] + inst.signExtIm()))];
                    break;
                case Instruction.Inst.nor:
                    GPR[inst.rd()] = ~(GPR[inst.rs()] | GPR[inst.rt()]);
                    break;
                case Instruction.Inst.or:
                    GPR[inst.rd()] = GPR[inst.rs()] | GPR[inst.rt()];
                    break;
                case Instruction.Inst.ori:
                    GPR[inst.rt()] = (GPR[inst.rs()] | inst.zeroExtIm());
                    break;
                case Instruction.Inst.slt:
                    GPR[inst.rd()] = ((GPR[inst.rs()] + 0x80000000) < (GPR[inst.rt()] + 0x80000000)) ? 1u : 0u;
                    break;
                case Instruction.Inst.slti:
                    GPR[inst.rt()] = ((GPR[inst.rs()] + 0x80000000) < (inst.zeroExtIm() + 0x80000000)) ? 1u : 0u;
                    break;
                case Instruction.Inst.sll:
                    GPR[inst.rd()] = GPR[inst.rs()] << (int)inst.shamt();
                    break;
                case Instruction.Inst.srl:
                    GPR[inst.rd()] = GPR[inst.rs()] >> (int)inst.shamt();
                    break;
                case Instruction.Inst.sw:          
                    Mem[(int)((GPR[inst.rs()] + inst.signExtIm()))] = GPR[inst.rt()];
                    break;
                case Instruction.Inst.sub:
                    GPR[inst.rd()] = GPR[inst.rs()] - GPR[inst.rt()];
                    break;
                case Instruction.Inst.hlt:
                    Console.WriteLine();
                    return false;                              
                case Instruction.Inst.input:
                    GPR[inst.rd()] = (uint)Console.Read();
                    break;
                case Instruction.Inst.output:
                    Console.Write((char)GPR[inst.rt()]);
                    break;
                case Instruction.Inst.bclt:
                    if (FPCond)
                        nextPC += inst.branchAddr();
                    break;
                case Instruction.Inst.bclf:
                    if (!FPCond)
                        nextPC += inst.branchAddr();
                    break;
                case Instruction.Inst.adds:
                    FPR[inst.fd()] = FPR[inst.fs()] + FPR[inst.ft()];
                    break;
                case Instruction.Inst.ceqs:
                    FPCond = FPR[inst.fs()] == FPR[inst.ft()];
                    break;
                case Instruction.Inst.clts:
                    FPCond = FPR[inst.fs()] < FPR[inst.ft()];
                    break;
                case Instruction.Inst.cles:
                    FPCond = FPR[inst.fs()] <= FPR[inst.ft()];
                    break;
                case Instruction.Inst.muls:
                    FPR[inst.fd()] = FPR[inst.fs()] * FPR[inst.ft()];
                    break;
                case Instruction.Inst.invs:
                    FPR[inst.fd()] = 1.0f / FPR[inst.ft()];
                    break;
                case Instruction.Inst.subs:
                    FPR[inst.fd()] = FPR[inst.fs()] - FPR[inst.ft()];
                    break;
                case Instruction.Inst.lws:
                    FPR[inst.rt()] = Mem[(int)((GPR[inst.rs()] + inst.signExtIm()))].toSingle(Instruction.isLittleEndian);
                    break;
                case Instruction.Inst.mult: //obsolete instruction
                    throw new Exception("mult?");
                case Instruction.Inst.sws:
                    Mem[(int)((GPR[inst.rs()] + inst.signExtIm()))] = FPR[inst.rt()].ToUInt32(Instruction.isLittleEndian);
                    break;
                default:
                    throw new Exception($"{Instruction.Convert(inst)} at {PC:X8}?????");
            }
            PC = nextPC;
            return true;
        }
    }
}

