using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sobriquet {
	public class Generator {
		private int _seed = 0;
		private readonly MarkovChain _chain;

		private Dictionary<string, bool> _originalNames = new Dictionary<string, bool>();
		private Dictionary<string, bool> _seenNames = new Dictionary<string, bool>();

		/// <summary>
		/// Returns all the names used to build this generator
		/// </summary>
		public IEnumerable<string> OriginalNames {
			get {
				return _originalNames.Keys;
			}
		}

		/// <summary>
		/// Create a generator given a list of names
		/// </summary>
		/// <param name="order">Order of the internal markov chain</param>
		/// <param name="names">List of names to base generator on</param>
		public Generator(int order, IEnumerable<string> names) {
			_chain = new MarkovChain(order);
			foreach (var name in names) {
				_chain.Add(name, 1);
				_originalNames[name] = true;
			}
		}
		/// <summary>
		/// Create a generator given a weighted list of names
		/// </summary>
		/// <param name="order">Order of the internal markov chain</param>
		/// <param name="names">List of weighted names to base generator on</param>
		public Generator(int order, IEnumerable<WeightedName> wnames) {
			_chain = new MarkovChain(order);
			foreach (var wn in wnames) {
				_chain.Add(wn.Name, wn.Weight);
				_originalNames[wn.Name] = true;
			}
		}

		/// <summary>
		/// Returns a random name.
		/// </summary>
		public string Next() {
			var name = _chain.Chain(_seed);
			_seed++;
			return name;
		}

		/// <summary>
		/// Returns a random name, making sure not to generate a name that was given in the training data.
		/// </summary>
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

		/// <summary>
		/// Returns a random name, making sure not to generate a name that was given in the training data or that was previously returned by this function.
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// Returns all generatable names of a certain length or less.
		/// </summary>
		/// <param name="maxlen">Max returned name length</param>
		public IEnumerable<string> AllRaw(int maxlen) {
			return _chain.AllRaw(maxlen);
		}
	}
}
