// hw brickbench
// benchmark test for NXT/EV3 and similar Micro Controllers
// PL: C# build by Xamarin Studio 5.5.3 + MonoBrick Frirmware Add-in 
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
			for (var y = 0; y < 5000; ++y) {
				var s = RandMersenneTwister.Get() % 10001;
			}
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
			for(y=0;y<30;++y)
			{	//memcpy(t, a, sizeof(a));
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

		static void TestTextOut()
		{
			Lcd.Instance.SaveScreen ();

			var point = new Point();

			for (var y = 0; y < 20; ++y)
			{	Lcd.Instance.Clear();

				point.Y = 10;
				Lcd.Instance.WriteText(Font.SmallFont, point, string.Format("{0} {1} int_Add", 0, 1000), true);
				point.Y += 10;
				Lcd.Instance.WriteText(Font.SmallFont, point, string.Format("{0} {1} int_Mult", 1, 1010), true);
				point.Y += 10;
				Lcd.Instance.WriteText(Font.SmallFont, point, string.Format("{0} {1} float_op", 2, 1020), true);
				point.Y += 10;
				Lcd.Instance.WriteText(Font.SmallFont, point, string.Format("{0} {1} randomize", 3, 1030), true);
				point.Y += 10;
				Lcd.Instance.WriteText(Font.SmallFont, point, string.Format("{0} {1} matrx_algb", 4, 1040), true);
				point.Y += 10;
				Lcd.Instance.WriteText(Font.SmallFont, point, string.Format("{0} {1} arr_sort", 5, 1050), true);
				point.Y += 10;
				Lcd.Instance.WriteText(Font.SmallFont, point, string.Format("{0} {1} displ_txt", 6, 1060), true);
				point.Y += 10;
				Lcd.Instance.WriteText(Font.SmallFont, point, string.Format("{0} {1} testing...", 7, 1070), true);

				Lcd.Instance.Update ();
			}

			Lcd.Instance.LoadScreen ();
		}

		static void TestGraphics()
		{
			var p5040 = new Point (50, 40);
			var p3024 = new Point (30, 24);
			var p1010 = new Point (10, 10);
			var p6060 = new Point (60, 60);
			var p5020 = new Point (50, 20);
			var p9070 = new Point (90, 70);
			var r20204040 = new Rectangle (new Point (20, 20), new Point(40, 40));
			var r65252030 = new Rectangle (new Point (65, 25), new Point (20, 30));
			var p7030 = new Point (70, 30);

			Lcd.Instance.SaveScreen ();

			for (var y = 0; y < 100; ++y)
			{	LcdConsole.Clear();

				// CircleOut(50, 40, 10);
				Lcd.Instance.DrawCircle (p5040, 10, true, false);
				// CircleOutEx(30, 24, 10, DRAW_OPT_FILL_SHAPE);
				Lcd.Instance.DrawCircle (p3024, 10, true, true);
				// LineOut(10, 10, 60, 60);
				Lcd.Instance.DrawLine (p1010, p6060, true);
				// LineOut(50, 20, 90, 70);
				Lcd.Instance.DrawLine (p5020, p9070, true);
				// RectOut(20, 20, 40, 40);
				Lcd.Instance.DrawRectangle (r20204040, true, false);
				// RectOutEx(65, 25, 20, 30, DRAW_OPT_FILL_SHAPE);
				Lcd.Instance.DrawRectangle (r65252030, true, true);
				// EllipseOut(70, 30, 15, 20);
				Lcd.Instance.DrawEllipse (p7030, 15, 20, true, false);

				Lcd.Instance.Update ();
			}

			Lcd.Instance.LoadScreen ();
		}


		public static void Main (string[] args)
		{
			LcdConsole.Clear();
			LcdConsole.WriteLine ("hw brickbench. (C)H.Wunder 2013");
			LcdConsole.WriteLine ("initializing...");

			int i;
			for (i = 0; i < 500; ++i)
			{
				a[i] = RandMersenneTwister.GetIntValue() % 30000;
				b[i] = RandMersenneTwister.GetIntValue() % 30000;
				c[i] = RandMersenneTwister.GetIntValue() % 30000;
			};

			while (!escapeIsPressed)
			{
				DoTest (0, "Int_Add", TestIntAdd);
				DoTest (1, "Int_Mult", TestIntMult);
				DoTest (2, "Float_Op", TestFloatMath);
				DoTest (3, "Rand_array", TestRandMT);
				DoTest (4, "Matrx_algb", TestMatrixMath);
				DoTest (5, "Array_sort", TestSort);
				DoTest (6, "Display_text", TestTextOut);
				DoTest (7, "Graphics", TestGraphics);

				LcdConsole.WriteLine ("Press Enter for repeat tests");
				LcdConsole.WriteLine ("or Esc for exit...");
				WaitForEnterOrEscPress();
			}

		}

		static EventWaitHandle stopped = new ManualResetEvent(false);
		static bool escapeIsPressed = false;

		static void WaitForEnterOrEscPress()
		{
			escapeIsPressed = false;  
			ButtonEvents buts = new ButtonEvents();
			buts.EnterPressed += () => stopped.Set();
			buts.EscapePressed += () => {escapeIsPressed = true; stopped.Set();};
			stopped.Reset ();
			stopped.WaitOne();
		}

		static Stopwatch sw = new Stopwatch ();

		static void DoTest(int testNo, string testName, Action test)
		{
			sw.Reset ();
			sw.Start ();
			test();
			sw.Stop ();
			runtime[testNo] = sw.ElapsedMilliseconds;
			displayValue (testNo, sw.ElapsedMilliseconds, testName);
		}

		static void displayValue(int testNo, long testTime, String testName)
		{
			LcdConsole.WriteLine ("{0:D1}: {1} - {2} ms", testNo, testName.PadRight(10), testTime);
		}

	}
}