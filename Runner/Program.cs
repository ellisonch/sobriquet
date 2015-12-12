using Sobriquet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runner {
	class Program {
		static void Main(string[] args) {
			////////////////////
			// generate names based on supplied list
			var names = new List<string> {
				"Carlos"
				, "Charlotte"
				, "Marley"
			};
			var namegen = new Sobriquet.Generator(3, names);
			for (int i = 0; i < 5; i++) {
				Console.WriteLine(namegen.Next());
			}

			////////////////////
			// generate names based on supplied, default generators
			var dg = new DefaultGenerators(5);
			//Console.WriteLine(dg.MaleFirstName.Next() + " " + dg.LastName.Next());
			for (int i = 0; i < 4; i++) {
				Console.WriteLine(dg.MaleFirstName.Next() + " " + dg.LastName.Next());
			}

			////////////////////
			// generate all names of a certain size
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
