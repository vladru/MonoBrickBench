using System;

namespace BrickBench
{
	public static class RandM
	{
		static uint[] y = new uint[25];
		static int index = 25 + 1;
		const int M = 7;
		static readonly uint[] A = { 0, 0x8ebfd028 };

		public static uint Get()
		{
			if (index >= 25)
			{
				int k;
				if (index > 25)
				{
					uint r = 9, s = 3402;
					for (k = 0; k < 25; ++k)
					{
						r = 509845221 * r + 3;
						s *= s + 1;
						y[k] = s + (r >> 10);
					}
				}
				for (k = 0; k < 25 - M; ++k)
					y[k] = y[k + M] ^ (y[k] >> 1) ^ A[y[k] & 1];
				for (; k < 25; ++k)
					y[k] = y[k + (M - 25)] ^ (y[k] >> 1) ^ A[y[k] & 1];
				index = 0;
			}

			uint e = y[index++];
			e ^= (e << 7) & 0x2b5b2500;
			e ^= (e << 15) & 0xdb8b0000;
			e ^= (e >> 16);
			return e;
		}

		public static int GetIntValue()
		{
			return unchecked((int)Get());
		}
	}
}

