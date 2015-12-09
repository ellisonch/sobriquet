using Sobriquet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Runner {
	internal class DefaultGenerators {
		private static int _order = 5;
		private static Stopwatch _sw = new Stopwatch();

		private readonly Generator _maleFirstGenerator;
		private readonly Generator _femaleFirstGenerator;
		private readonly Generator _lastGenerator;

		public DefaultGenerators() {
			var maleFirstNamesFileName = Properties.Resources.dist_male;
			var femaleFirstNamesFileName = Properties.Resources.dist_female;
			var lastNamesFileName = Properties.Resources.dist_all;

			var maleFirstGenerator = Generate(_order, maleFirstNamesFileName);
			var femaleFirstGenerator = Generate(_order, femaleFirstNamesFileName);
			var lastGenerator = Generate(_order, lastNamesFileName);
			
			Console.WriteLine("Creation took {0}ms", _sw.ElapsedMilliseconds);
			
			_maleFirstGenerator = maleFirstGenerator;
			_femaleFirstGenerator = femaleFirstGenerator;
			_lastGenerator = lastGenerator;
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

		private static Generator Generate(int order, byte[] file) {
			var wns = FromNameTabWeightFile(file);
			_sw.Start();
			var generator = new Generator(order, wns);
			_sw.Stop();
			return generator;
		}
		
		public static IEnumerable<WeightedName> FromNameTabWeightFile(byte[] bytes) {
			var file = new System.IO.StreamReader(new MemoryStream(bytes), Encoding.UTF8);

			var names = new List<WeightedName>();
			string line;

			while ((line = file.ReadLine()) != null) {
				var parts = line.Split(new char[] { '\t' });
				var name = parts[0];
				var weightString = parts[1];
				var weight = int.Parse(weightString);
				names.Add(new WeightedName(name, weight));
			}

			return names;
		}
	}
}
