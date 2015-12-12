//-----------------------------------------------------------------------
//  Copyright © 2011 John Gietzen.
//  Modifications Copyright © 2015 Charles (Chucky) Ellison.
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//-----------------------------------------------------------------------

namespace Sobriquet {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// Builds and walks interconnected states based on a weighted probability.
	/// </summary>
	internal class MarkovChain {
		private readonly int _order;

		private readonly Dictionary<string, Dictionary<char, int>> _items = new Dictionary<string, Dictionary<char, int>>();
		private readonly Dictionary<string, int> _terminals = new Dictionary<string, int>();

		/// <summary>
		/// Initializes a new instance of the MarkovChain class.
		/// </summary>
		/// <param name="order">Indicates the desired order of the <see cref="Markov.MarkovChain"/>.</param>
		/// <remarks>
		/// <para>The <paramref name="order"/> of a generator indicates the depth of its internal state.  A generator
		/// with an order of 1 will choose items based on the previous item, a generator with an order of 2
		/// will choose items based on the previous 2 items, and so on.</para>
		/// <para>Zero is not classically a valid order value, but it is allowed here.  Choosing a zero value has the
		/// effect that every state is equivalent to the starting state, and so items will be chosen based on their
		/// total frequency.</para>
		/// </remarks>
		public MarkovChain(int order) {
			if (order < 0) {
				throw new ArgumentOutOfRangeException("order");
			}

			_order = order;
		}

		/// <summary>
		/// Adds the items to the generator with the weight specified.
		/// </summary>
		/// <param name="items">The items to add to the generator.</param>
		/// <param name="weight">The weight at which to add the items.</param>
		public void Add(string items, int weight) {
			// Queue<char> previous = new Queue<char>();
			int starti = 0;
			int length = 0;
			string previous = "";

			foreach (var item in items) {
				var key = previous;

				this.Add(key, item, weight);
				
				length++;
				if (length > _order) {
					length--;
					starti++;
				}

				previous = SafeSubstring(items, starti, length);
			}

			var terminalKey = previous;
			_terminals[terminalKey] = _terminals.ContainsKey(terminalKey)
				? weight + _terminals[terminalKey]
				: weight;
		}

		private void Add(string state, char next, int weight) {
			Dictionary<char, int> weights;
			if (!_items.TryGetValue(state, out weights)) {
				weights = new Dictionary<char, int>();
				_items.Add(state, weights);
			}

			int oldWeight;
			weights.TryGetValue(next, out oldWeight);
			weights[next] = oldWeight + weight;
		}

		/// <summary>
		/// Randomly walks the chain.
		/// </summary>
		/// <returns>An <see cref="string"/> of the items chosen.</returns>
		/// <remarks>Assumes an empty starting state.</remarks>
		public string Chain() {
			return this.Chain("", new Random());
		}

		/// <summary>
		/// Randomly walks the chain.
		/// </summary>
		/// <param name="previous">The items preceding the first item in the chain.</param>
		/// <returns>An <see cref="string"/> of the items chosen.</returns>
		public string Chain(string previous) {
			return this.Chain(previous, new Random());
		}

		/// <summary>
		/// Randomly walks the chain.
		/// </summary>
		/// <param name="seed">The seed for the random number generator, used as the random number source for the chain.</param>
		/// <returns>An <see cref="string"/> of the items chosen.</returns>
		/// <remarks>Assumes an empty starting state.</remarks>
		public string Chain(int seed) {
			return this.Chain("", new Random(seed));
		}

		/// <summary>
		/// Randomly walks the chain.
		/// </summary>
		/// <param name="previous">The items preceding the first item in the chain.</param>
		/// <param name="seed">The seed for the random number generator, used as the random number source for the chain.</param>
		/// <returns>An <see cref="string"/> of the items chosen.</returns>
		public string Chain(string previous, int seed) {
			return this.Chain(previous, new Random(seed));
		}

		/// <summary>
		/// Randomly walks the chain.
		/// </summary>
		/// <param name="rand">The random number source for the chain.</param>
		/// <returns>An <see cref="string"/> of the items chosen.</returns>
		/// <remarks>Assumes an empty starting state.</remarks>
		public string Chain(Random rand) {
			return this.Chain("", rand);
		}
		
		/// <summary>
		/// Randomly walks the chain.
		/// </summary>
		/// <param name="previous">The items preceding the first item in the chain.</param>
		/// <param name="rand">The random number source for the chain.</param>
		/// <returns>An <see cref="string"/> of the items chosen.</returns>
		public string Chain(string previous, Random rand) {
			StringBuilder result = new StringBuilder();

			Queue<char> state = new Queue<char>(previous);
			while (true) {
				while (state.Count > _order) {
					state.Dequeue();
				}

				var key = new string(state.ToArray());

				Dictionary<char, int> weights;
				if (!_items.TryGetValue(key, out weights)) {
					return result.ToString();
				}

				int terminalWeight;
				_terminals.TryGetValue(key, out terminalWeight);

				var total = weights.Sum(w => w.Value);
				var value = rand.Next(total + terminalWeight) + 1;

				if (value > total) {
					return result.ToString();
				}

				var currentWeight = 0;
				foreach (var nextItem in weights) {
					currentWeight += nextItem.Value;
					if (currentWeight >= value) {
						result.Append(nextItem.Key);
						state.Enqueue(nextItem.Key);
						break;
					}
				}
			}
		}

		internal IEnumerable<string> AllRaw(int maxlen) {
			return AllRaw("", maxlen);
		}

		internal IEnumerable<string> AllRaw(string prefix, int maxlen) {
			if (prefix.Length > _order) {
				throw new ArgumentException(string.Format("prefix should be fewer than {0} chars long", _order));
			}

			var queue = new Queue<string>();
			queue.Enqueue(prefix);

			while (queue.Count > 0) {
				var current = queue.Dequeue();
				if (current.Length > maxlen) {
					continue;
				}
				var suffix = GetLast(current, _order);

				// first, see if it's a possible terminal state and if so, return it
				int terminalWeight;
				_terminals.TryGetValue(suffix, out terminalWeight);
				if (terminalWeight > 0) {
					yield return current;
				}

				// next, enqueue all the possible extensions
				Dictionary<char, int> weights;
				if (!_items.TryGetValue(suffix, out weights)) {
					// yield return current;
					continue;
				}
				foreach (var kvp in weights) {
					var nextChar = kvp.Key;
					var weight = kvp.Value;
					if (weight == 0) {
						continue;
					}
					queue.Enqueue(current + nextChar);
				}
			}
		}


		private static string SafeSubstring(string text, int start, int length) {
			return text.Length <= start ? ""
				: text.Length - start <= length ? text.Substring(start)
				: text.Substring(start, length);
		}
		private string GetLast(string prefix, int length) {
			if (prefix.Length <= length) {
				return prefix;
			}
			return prefix.Substring(prefix.Length - length);
		}
	}
}
