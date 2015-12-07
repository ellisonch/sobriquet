using ProtoBuf;
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
	[ProtoContract]
	internal class DefaultGenerators {
		private static string _serializedFileName = @"D:\prog\Sobriquet\defaultGen.dat";
		private static int _order = 6;

		[ProtoMember(1)]
		private Generator _maleFirstGenerator;
		[ProtoMember(2)]
		private Generator _femaleFirstGenerator;
		[ProtoMember(3)]
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
			var maleFirstNames = @"D:\prog\Sobriquet\dist.male.first";
			var femaleFirstNames = @"D:\prog\Sobriquet\dist.female.first";
			var lastNames = @"D:\prog\Sobriquet\dist.all.last";

			var wns = FromNameTabWeightFile(maleFirstNames);
			var maleFirstGenerator = new Generator(_order, wns);

			wns = FromNameTabWeightFile(femaleFirstNames);
			var femaleFirstGenerator = new Generator(_order, wns);

			wns = FromNameTabWeightFile(lastNames);
			var lastGenerator = new Generator(_order, wns);

			var dg = new DefaultGenerators(maleFirstGenerator, femaleFirstGenerator, lastGenerator);
			// formatter.Serialize(, fn_generator);
			return dg;
		}

		private static DefaultGenerators LoadFromDisk() {
			//BinaryFormatter formatter = new BinaryFormatter();
			//var binaryFile = new FileStream(_serializedFileName, FileMode.Open);

			//return (DefaultGenerators)formatter.Deserialize(binaryFile);
			DefaultGenerators dg;
			using (var file = File.OpenRead(_serializedFileName)) {
				dg = Serializer.Deserialize<DefaultGenerators>(file);
			}
			return dg;
		}
		private static void SerializeToDisk(DefaultGenerators dg) {
			//BinaryFormatter formatter = new BinaryFormatter();
			//var binaryFile = new FileStream(_serializedFileName, FileMode.OpenOrCreate);

			//formatter.Serialize(binaryFile, dg);
			using (var file = File.Create(_serializedFileName)) {
				Serializer.Serialize(file, dg);
			}
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
