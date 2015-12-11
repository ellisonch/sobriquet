﻿using Sobriquet;
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
			var names = new List<string> {
				"Carlos"
				, "Charlotte"
				, "Marley"
			};
			var namegen = new Sobriquet.Generator(3, names);

			for (int i = 0; i < 5; i++) {
				Console.WriteLine(namegen.Next());
			}


			var dg = new DefaultGenerators(5);
			//Console.WriteLine(dg.MaleFirstName.Next() + " " + dg.LastName.Next());
			for (int i = 0; i < 4; i++) {
				Console.WriteLine(dg.MaleFirstName.Next() + " " + dg.LastName.Next());
			}
			//for (int i = 0; i < 40; i++) {
			//	// var name = new string(chain.Chain(_rand).ToArray());
			//	var name = InitCaps(dg.FemaleFirstName.NextUnique()) + " " + InitCaps(dg.LastName.NextUnique());
			//	Console.WriteLine("{0}", name);
			//}

			var generatedNames = new HashSet<string>(dg.MaleFirstName.AllRaw(5));
			var actualNames = new HashSet<string>(dg.MaleFirstName.OriginalNames.Where((x) => x.Length <= 5));
			var added = generatedNames.Except(actualNames);
			var removed = actualNames.Except(generatedNames);

			Console.WriteLine("Added:");
			foreach (var name in added) {
				Console.WriteLine(name);
			}

			Console.WriteLine("Removed:");
			foreach (var name in removed) {
				Console.WriteLine(name);
			}

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
