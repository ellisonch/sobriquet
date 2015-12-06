using Sobriquet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Runner {
	[Serializable]
	internal class DefaultGenerators {
		private static string _serializedFileName = @"D:\prog\Sobriquet\defaultGen.dat";

		private Generator _fnGenerator;
		private Generator _lnGenerator;

		public DefaultGenerators(Generator fnGenerator, Generator lnGenerator) {
			_fnGenerator = fnGenerator;
			_lnGenerator = lnGenerator;
		}

		public Generator FirstName {
			get { return _fnGenerator; }
		}
		public Generator LastName {
			get { return _lnGenerator; }
		}

		public static DefaultGenerators Startup() {
			DefaultGenerators dg;

			try {
				dg = LoadFromDisk();

				if (dg != null) {
					return dg;
				}
			} catch { // if we fail in any way, just load the data from disk

			}


			dg = LoadFromData();
			SerializeToDisk(dg);

			return dg;
		}


		private static DefaultGenerators LoadFromData() {
			var firstNames = @"D:\prog\Sobriquet\dist.male.first";
			var lastNames = @"D:\prog\Sobriquet\dist.all.last";

			var wns = FromNameTabWeightFile(firstNames);
			var fnGenerator = new Generator(5, wns);

			wns = FromNameTabWeightFile(lastNames);
			var lnGenerator = new Generator(5, wns);

			var dg = new DefaultGenerators(fnGenerator, lnGenerator);
			// formatter.Serialize(, fn_generator);
			return dg;
		}

		private static DefaultGenerators LoadFromDisk() {
			BinaryFormatter formatter = new BinaryFormatter();
			var binaryFile = new FileStream(_serializedFileName, FileMode.Open);

			return (DefaultGenerators)formatter.Deserialize(binaryFile);
		}
		private static void SerializeToDisk(DefaultGenerators dg) {
			BinaryFormatter formatter = new BinaryFormatter();
			var binaryFile = new FileStream(_serializedFileName, FileMode.OpenOrCreate);

			formatter.Serialize(binaryFile, dg);
		}

		public static IEnumerable<WeightedName> FromNameTabWeightFile(string filename) {
			var file = new System.IO.StreamReader(filename);

			var names = new List<WeightedName>();
			string line;

			while ((line = file.ReadLine()) != null) {
				var parts = line.Split(new char[] { '\t' });
				var name = parts[0];
				var percentageString = parts[1];
				var percentage = float.Parse(percentageString);
				var weight = (int)(percentage * 1000);
				names.Add(new WeightedName(name, weight));
			}

			return names;
		}

	}

	class Program {
		static void Main(string[] args) {
			var dg = DefaultGenerators.Startup();

			for (int i = 0; i < 40; i++) {
				// var name = new string(chain.Chain(_rand).ToArray());
				var name = InitCaps(dg.FirstName.NextUnique()) + " " + InitCaps(dg.LastName.NextUnique());
				Console.WriteLine("{0}", name);
			}
			Console.Read();
		}
		
		private static string InitCaps(string s) {
			var t = s.ToLower();
			return char.ToUpper(t[0]) + t.Substring(1);
		}
	}
}
