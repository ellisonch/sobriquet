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
		//private static string _maleFirstNamesFileName = ResourceManager. // @"D:\prog\Sobriquet\names.data\dist.male.first";
		//private static string _femaleFirstNamesFileName = @"D:\prog\Sobriquet\names.data\dist.female.first";
		//private static string _lastNamesFileName = @"D:\prog\Sobriquet\names.data\dist.all.last";

		private static int _order = 5;
		private static Stopwatch _sw = new Stopwatch();

		private Generator _maleFirstGenerator;
		private Generator _femaleFirstGenerator;
		private Generator _lastGenerator;
		

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

			dg = LoadFromData();
			Console.WriteLine("Creation took {0}ms", _sw.ElapsedMilliseconds);
			// SerializeToDisk(dg);

			return dg;
		}


		private static DefaultGenerators LoadFromData() {
			// Properties.Resources.dist_all
			var maleFirstNamesFileName = Properties.Resources.dist_male;
			var femaleFirstNamesFileName = Properties.Resources.dist_female;
			var lastNamesFileName = Properties.Resources.dist_all;
			//System.Reflection.Assembly myAssembly = System.Reflection.Assembly.Load("Runner");
			//ResourceManager myManager = new ResourceManager("dist.all", myAssembly);

			
			var maleFirstGenerator = Generate(_order, maleFirstNamesFileName);
			var femaleFirstGenerator = Generate(_order, femaleFirstNamesFileName);
			var lastGenerator = Generate(_order, lastNamesFileName);

			maleFirstGenerator = Generate(_order, maleFirstNamesFileName);
			femaleFirstGenerator = Generate(_order, femaleFirstNamesFileName);
			lastGenerator = Generate(_order, lastNamesFileName);
			maleFirstGenerator = Generate(_order, maleFirstNamesFileName);
			femaleFirstGenerator = Generate(_order, femaleFirstNamesFileName);
			lastGenerator = Generate(_order, lastNamesFileName);

			var dg = new DefaultGenerators(maleFirstGenerator, femaleFirstGenerator, lastGenerator);
			return dg;
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
