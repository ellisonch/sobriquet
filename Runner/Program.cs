using Sobriquet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Runner {
	class Program {
		static void Main(string[] args) {
			var dg = DefaultGenerators.Startup();

			//for (int i = 0; i < 40; i++) {
			//	// var name = new string(chain.Chain(_rand).ToArray());
			//	var name = InitCaps(dg.FemaleFirstName.NextUnique()) + " " + InitCaps(dg.LastName.NextUnique());
			//	Console.WriteLine("{0}", name);
			//}
			Console.Read();
		}
		
		private static string InitCaps(string s) {
			if (s.Length == 0) {
				return "";
			}
			var t = s.ToLower();
			return char.ToUpper(t[0]) + t.Substring(1);
		}
	}
}
