// RayUtils
// https://github.com/Bugfire

using UnityEngine;
using NUnit.Framework;

namespace RayStorm
{
	
	public class RayStringTest
	{

		[Test]
		public void RayStringTestSimplePasses ()
		{
			{
				var ray = new RayString (1);
				var std = "";

				ray.AddStr ("Foo");
				std += "Foo";
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());

				ray.AddStr ("Bar");
				std += "Bar";
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());

				ray.AddStr ("Baz", 1);
				std += FormatString ("Baz", 1);
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());

				ray.AddStr ("Hoo", 16);
				std += FormatString ("Hoo", 16);
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());

				ray.Clear ();
				std = "";
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());

				ray.AddStr ("Nice").AddStr ("Guy");
				std += "Nice" + "Guy";
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());

				ray.AddInt (0);
				std += string.Format ("{0}", 0);
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());

				ray.AddInt (120);
				std += string.Format ("{0}", 120);
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());

				ray.AddInt (-123456);
				std += string.Format ("{0}", -123456);
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());
			}

			{
				var ray = new RayString (8);
				var std = "";

				ray.AddInt (0, 5);
				std += string.Format ("{0,5}", 0);
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());

				ray.AddInt (120, 2);
				std += string.Format ("{0,2}", 120);
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());

				ray.AddInt (-123456, 10);
				std += string.Format ("{0,10}", -123456);
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());
			}

			{
				var ray = new RayString (16);
				var std = "";

				ray.AddFloat (0.0f, 0, 0);
				std += string.Format ("{0:f0}", 0.0f);
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());

				ray.AddFloat (123.456f, 0, 0);
				std += string.Format ("{0:f0}", 123.456f);
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());

				ray.AddFloat (123.456f, 0, 1);
				std += string.Format ("{0:f1}", 123.456f);
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());

				ray.AddFloat (123.456f, 0, 4);
				std += string.Format ("{0:f4}", 123.456f);
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());

				ray.AddFloat (-123.456f, 0, 2);
				std += string.Format ("{0:f2}", -123.456f);
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());

				ray.AddFloat (-123.456f, 0, 5);
				std += string.Format ("{0:f5}", -123.456f);
				Debug.LogFormat ("[{0}] == [{1}]", std, ray.ToString ());
				Assert.AreEqual (std, ray.ToString ());
			}
		}

		string FormatString (string text, int minLength)
		{
			var fillLength = minLength - text.Length;
			while (fillLength > 0) {
				text += " ";
				fillLength--;
			}
			return text;
		}
	}

}