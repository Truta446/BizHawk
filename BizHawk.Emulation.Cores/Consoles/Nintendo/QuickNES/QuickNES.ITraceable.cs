﻿using System;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Cores.Components.M6502;

namespace BizHawk.Emulation.Cores.Consoles.Nintendo.QuickNES
{
	partial class QuickNES
	{
		public TraceBuffer Tracer { get; private set; }

		private LibQuickNES.TraceCallback _tracecb;

		private void MakeTrace(IntPtr data)
		{
			int[] s = new int[7];
			System.Runtime.InteropServices.Marshal.Copy(data, s, 0, 7);

			byte a = (byte)s[0];
			byte x = (byte)s[1];
			byte y = (byte)s[2];
			ushort sp = (ushort)s[3];
			ushort pc = (ushort)s[4];
			byte p = (byte)s[5];

			byte opcode = (byte)s[6];

			int notused = 0;
			string opcodeStr = MOS6502X.Disassemble(pc, out notused, (address) => _memoryDomains.SystemBus.PeekByte(address));

			Tracer.Put(new TraceInfo
			{
				Disassembly = string.Format("{0:X4} {1}", pc, opcodeStr).PadRight(26),
				RegisterInfo = string.Format(
					"SP:{0:X2} A:{1:X2} P:{2:X2} X:{3:X2} Y:{4:X2}",
					sp,
					a,
					p,
					x,
					y)
			});
		}

		private const string TraceHeader = "PC: OP SP_A_P_X_Y ";

		private void ConnectTracer()
		{
			Tracer = new TraceBuffer { Header = TraceHeader };
			(ServiceProvider as BasicServiceProvider).Register<ITraceable>(Tracer);
			_tracecb = new LibQuickNES.TraceCallback(MakeTrace);
		}
	}
}