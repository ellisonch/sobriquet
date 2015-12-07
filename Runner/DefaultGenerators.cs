using Sobriquet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Runner {
	internal class DefaultGenerators {
		private static string _maleFirstNamesFileName = @"D:\prog\Sobriquet\names.data\dist.male.first";
		private static string _femaleFirstNamesFileName = @"D:\prog\Sobriquet\names.data\dist.female.first";
		private static string _lastNamesFileName = @"D:\prog\Sobriquet\names.data\dist.all.last";

		private static int _order = 5;

		private Generator _maleFirstGenerator;
		private Generator _femaleFirstGenerator;
		private Generator _lastGenerator;

		[Obsolete]
		private DefaultGenerators() { }
		public DefaultGenerators(Generator maleFirstGenerator, Generator femaleFirstGenerator, Generator lnGenerator) {
			_maleFirstGenerator = maleFirstGenerator;
			_femaleFirstGenerator = femaleFirstGenerator;
			_lastGenerator = lnGenerator;
		}

		public Generator MaleFirstName {
			get { return _maleFirstGenerator; }
		}
		public Generator FemaleFirstName {
			get { return _femaleFirstGenerator; }
		}
		public Generator LastName {
			get { return _lastGenerator; }
		}

		public static DefaultGenerators Startup() {
			DefaultGenerators dg;
			Stopwatch sw;

			//try {
			//	sw = Stopwatch.StartNew();
			//	dg = LoadFromDisk();
			//	Console.WriteLine("Deserialization took {0}ms", sw.ElapsedMilliseconds);

			//	if (dg != null) {
			//		return dg;
			//	}
			//} catch (Exception e) { // if we fail in any way, just load the data from disk
			//	Console.WriteLine("Exception: {0}: {1}", e.Message, e);
			//}

			sw = Stopwatch.StartNew();
			dg = LoadFromData();
			Console.WriteLine("Creation took {0}ms", sw.ElapsedMilliseconds);
			// SerializeToDisk(dg);

			return dg;
		}


		private static DefaultGenerators LoadFromData() {
			var maleFirstGenerator = Generate(_order, _maleFirstNamesFileName);
			var femaleFirstGenerator = Generate(_order, _femaleFirstNamesFileName);
			var lastGenerator = Generate(_order, _lastNamesFileName);
			
			var dg = new DefaultGenerators(maleFirstGenerator, femaleFirstGenerator, lastGenerator);
			return dg;
		}

		private static Generator Generate(int order, string filename) {
			var wns = FromNameTabWeightFile(filename);
			var generator = new Generator(order, wns);
			return generator;
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
}
