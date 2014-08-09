// hw brickbench
// benchmark test for NXT/EV3 and similar Micro Controllers
// PL: C# build by Xamarin Studio 5.2. on runtime Mono 3.3.0
// Autor: (C) Helmut Wunder 2013, 2014
// Ported to MonoBrick C# by Vlad Ruzov
// freie Verwendung für private Zwecke
// für kommerzielle Zwecke nur nach Genehmigung durch den Autor.
// protected under the friendly Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License
// http://creativecommons.org/licenses/by-nc-sa/3.0/
// version 1.08

// Please be note that code below is not good style c# code.
// It has been ported from C so that the changes were minimal. 

using System;
using System.Diagnostics;
using System.Threading;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.UserInput;

namespace BrickBench
{
	public static class Program
	{

		static int[] a = new int[500], b = new int[500], c = new int[500];
		static long[] runtime = new long[8];

		// A[Ra x Ca] * B[Rb x Cb] = C[Ra x Cb]
		// N = Ra
		// M = Ca, Rb
		// K = Cb
		static void MatrixMatrixMult(int N, int M, int K, float[,] A, float[,] B, float[,] C) {
			int i, j, s;
			for (i = 0; i < N; ++i) {
				for (j = 0; j < K; ++j) {
					C[i,j] = 0;
					for (s = 0; s < M; ++s) {
						C[i,j] += A[i,s] * B[s,j];
					}
				}
			}
		}

		static float MatrixDet2x2(float[,] A) {               // Determinante 2x2
			return ( A[0,0]*A[1,1]- A[0,1]*A[1,0] );
		}

		static float MatrixDet3x3(float[,] A) {               // Determinante 3x3
			return (A[0,0]*A[1,1]*A[2,2]
				+A[0,1]*A[1,2]*A[2,0]
				+A[0,2]*A[1,0]*A[2,1]
				-A[0,2]*A[1,1]*A[2,0]
				-A[0,1]*A[1,0]*A[2,2]
				-A[0,0]*A[1,2]*A[2,1]);
		}

		static void shellsort(uint size, int[] sortData)
		{
			uint i, j;
			int temp;
			uint increment = size / 2;

			int[] A = (int[])sortData.Clone();

			while (increment > 0) {
				for (i = increment; i < size; i++) {
					j = i;
					temp = A[i];
					while ((j >= increment) && (A[j-increment] > temp)) {
						A[j] = A[j - increment];
						j = j - increment;
					}
					A[j] = temp;
				}

				if (increment == 2)
					increment = 1;
				else
					increment = (uint) ((float)increment / 2.2);
			}

			A.CopyTo(sortData, 0);
		}

		// --------------------------------------------
		// benchmark test procedures
		// --------------------------------------------

		static void TestIntAdd()
		{
			int i = 1, j = 11, k = 112, l = 1111, m = 11111, n = -1, o = -11, p = -111, q = -1112, r = -11111;
			int x;
			int s = 0;
			for (x = 0; x < 10000; ++x)
			{
				s += i; s += j; s += k;	s += l; s += m;	s += n;	s += o;	s += p; s += q;	s += r;
			}
			//return s;
		}

		static void TestIntMult() {
			int x, y;
			int s = 1;
			for(y=0;y<2000;++y) {
				s=1;
				for(x=1;x<=13;++x) { s*=x;}
				for(x=13;x>0;--x) { s/=x;}
			}
			//return s;
		}

		static void TestFloatMath() {
			double s = 3.141592;
			int y;

			for (y = 0; y < 5000; ++y) {
				s *= Math.Sqrt(s);
				s = Math.Sin(s);
				s *= Math.Cos(10.5 * s);
				s = Math.Sqrt(s);
				s = Math.Exp(s);
			}
			//return s;
		}

		static void TestRandMT() {
			uint s;
			int y;

			for (y = 0; y < 5000; ++y) {
				s = RandM.Get() % 10001;
			}
			//return s;
		}

		static void TestMatrixMath() {

			float[,] A = new float[2,2];
			float[,] B = new float[2,2];
			float[,] C = new float[2,2];
			float[,] O = new float[3,3];
			float s = 0;

			int x;
			for (x = 0; x < 250; ++x)
			{
				A[0,0] = 1;	A[0,1] = 3;
				A[1,0] = 2;	A[1,1] = 4;

				B[0,0] = 10; B[1,0] = 30;
				B[0,1] = 20; B[1,1] = 40;

				MatrixMatrixMult(2, 2, 2, A, B, C);

				A[0,0] = 1;	A[0,1] = 3;	A[1,0] = 2;	A[1,1] = 4;

				s = MatrixDet2x2(A);

				O[0,0] = 1;	O[0,1] = 5;	O[0,2] = 3;
				O[1,0] = 2;	O[1,1] = 4;	O[1,2] = 7;
				O[2,0] = 4;	O[2,1] = 6;	O[2,2] = 2;

				s = MatrixDet3x3(O);
				// s must be 74
				// http://www.wikihow.com/Find-the-Determinant-of-a-3X3-Matrix
			}

			s += (O[0,0] * O[1,1] * O[2,2]);
			//return s;
		}

		static void TestSort()
		{
			int[] t = new int[500];
			int y;
			//int i;
			for(y=0;y<30;++y) {
				//memcpy(t, a, sizeof(a));
				//for(i=0; i<500;++i) {t[i]=a[i];}
				a.CopyTo(t, 0);
				shellsort(500, t);
				//memcpy(t, b,  sizeof(a));
				//for(i=0; i<500;++i) {t[i]=b[i];}
				b.CopyTo(t, 0);
				shellsort(500, t);
				//memcpy(t, c,  sizeof(a));
				//for(i=0; i<500;++i) {t[i]=c[i];}
				c.CopyTo(t, 0);
				shellsort(500, t);
			}
		}

		static void displayValue(int testNo, long testTime, String testName)
		{
			LcdConsole.WriteLine ("{0:D1}: {1} - {2} ms", testNo, testName.PadRight(10), testTime);
		}

		public static void Main (string[] args)
		{
			LcdConsole.Clear();
			LcdConsole.WriteLine ("hw brickbench. (C)H.Wunder 2013");
			LcdConsole.WriteLine ("initializing...");

			int i;
			for (i = 0; i < 500; ++i)
			{
				a[i] = RandM.GetIntValue() % 30000;
				b[i] = RandM.GetIntValue() % 30000;
				c[i] = RandM.GetIntValue() % 30000;
			};

			while (!bEscapeIsPressed)
			{
				DoTest (0, "Int_Add", TestIntAdd);
				DoTest (1, "Int_Mult", TestIntMult);
				DoTest (2, "Float_Op", TestFloatMath);
				DoTest (3, "Rand_array", TestRandMT);
				DoTest (4, "Matrx_algb", TestMatrixMath);
				DoTest (5, "Array_sort", TestSort);

				LcdConsole.WriteLine ("Press Enter for repeat tests");
				LcdConsole.WriteLine ("or Esc for exit...");
				WaitForEnterOrEscPress();
			}

		}

		static EventWaitHandle stopped = new ManualResetEvent(false);
		static bool bEscapeIsPressed = false;

		private static void WaitForEnterOrEscPress()
		{
			bEscapeIsPressed = false;  
			ButtonEvents buts = new ButtonEvents();
			buts.EnterPressed += () => stopped.Set();
			buts.EscapePressed += () => {bEscapeIsPressed = true; stopped.Set();};
			stopped.Reset ();
			stopped.WaitOne();
		}

		static Stopwatch sw = new Stopwatch ();

		private static void DoTest(int testNo, string testName, Action test)
		{
			sw.Reset ();
			sw.Start ();
			test();
			sw.Stop ();
			runtime[testNo] = sw.ElapsedMilliseconds;
			displayValue (testNo, sw.ElapsedMilliseconds, testName);
		}
	}

}