using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sobriquet {
	[ProtoContract]
	public class Generator {
		[ProtoMember(1)]
		private int _seed = 0;
		[ProtoMember(2)]
		private Markov.MarkovChain _chain;

		[ProtoMember(3)]
		private Dictionary<string, bool> _originalNames = new Dictionary<string, bool>();
		[ProtoMember(4)]
		private Dictionary<string, bool> _seenNames = new Dictionary<string, bool>();

		[Obsolete]
		private Generator() { }

		public Generator(int order, IEnumerable<WeightedName> names) {
			_chain = new Markov.MarkovChain(order);
			foreach (var wn in names) {
				_chain.Add(wn.Name, wn.Weight);
				_originalNames[wn.Name] = true;
			}
		}

		public string Next() {
			var name = _chain.Chain(_seed);
			_seed++;
			return name;
		}

		public string NextNew() {
			string name;

			while (true) {
				name = Next();
				if (!_originalNames.ContainsKey(name)) {
					break;
				}
			}
			return name;
		}

		public string NextUnique() {
			string name;

			while (true) {
				name = NextNew();
				if (!_seenNames.ContainsKey(name)) {
					_seenNames[name] = true;
					break;
				}
			}
			return name;
		}
	}
}
