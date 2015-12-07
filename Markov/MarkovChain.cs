//-----------------------------------------------------------------------
// <copyright file="MarkovChain.cs" company="(none)">
//  Copyright © 2011 John Gietzen.
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
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace Markov
{
	using ProtoBuf;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Cryptography;

	/// <summary>
	/// Builds and walks interconnected states based on a weighted probability.
	/// </summary>
	[ProtoContract]
    public class MarkovChain
    {
		[ProtoMember(1)]
		private readonly int order;

		[ProtoMember(2)]
		private readonly Dictionary<string, Dictionary<char, int>> items = new Dictionary<string, Dictionary<char, int>>();
		[ProtoMember(3)]
		private readonly Dictionary<string, int> terminals = new Dictionary<string, int>();

		[Obsolete]
		private MarkovChain() { }

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
		public MarkovChain(int order)
        {
            if (order < 0)
            {
                throw new ArgumentOutOfRangeException("order");
            }

            this.order = order;
        }

        /// <summary>
        /// Adds the items to the generator with a weight of one.
        /// </summary>
        /// <param name="items">The items to add to the generator.</param>
        public void Add(string items)
        {
            this.Add(items, 1);
        }

        /// <summary>
        /// Adds the items to the generator with the weight specified.
        /// </summary>
        /// <param name="items">The items to add to the generator.</param>
        /// <param name="weight">The weight at which to add the items.</param>
        public void Add(string items, int weight)
        {
            Queue<char> previous = new Queue<char>();
            foreach (var item in items)
            {
                var key = new string(previous.ToArray());

                this.Add(key, item, weight);

                previous.Enqueue(item);
                if (previous.Count > this.order)
                {
                    previous.Dequeue();
                }
            }

            var terminalKey = new string(previous.ToArray());
            this.terminals[terminalKey] = this.terminals.ContainsKey(terminalKey)
                ? weight + this.terminals[terminalKey]
                : weight;
        }

        /// <summary>
        /// Adds the item to the generator, with the specified state preceding it.
        /// </summary>
        /// <param name="state">The state preceding the item.</param>
        /// <param name="next">The item to add.</param>
        /// <remarks>
        /// See <see cref="Markov.MarkovChain.Add(string, char, int)"/> for remarks.
        /// </remarks>
        public void Add(string state, char next)
        {
            this.Add(state, next, 1);
        }
		
        /// <summary>
        /// Adds the item to the generator, with the specified state preceding it and the specified weight.
        /// </summary>
        /// <param name="state">The state preceding the item.</param>
        /// <param name="next">The item to add.</param>
        /// <param name="weight">The weight of the item to add.</param>
        /// <remarks>
        /// This adds the state as-is.  The state may not be reachable if, for example, the
        /// number of items in the state is greater than the order of the generator, or if the
        /// combination of items is not available in the other states of the generator.
        /// </remarks>
        public void Add(string state, char next, int weight)
        {
            Dictionary<char, int> weights;
            if (!this.items.TryGetValue(state, out weights))
            {
                weights = new Dictionary<char, int>();
                this.items.Add(state, weights);
            }

            weights[next] = weights.ContainsKey(next)
                ? weight + weights[next]
                : weight;
        }

        /// <summary>
        /// Gets the items from the generator that follow from an empty state.
        /// </summary>
        /// <returns>A dictionary of the items and their weight.</returns>
        public Dictionary<char, int> GetInitialStates()
        {
            var startState = "";

            Dictionary<char, int> weights;
            if (this.items.TryGetValue(startState, out weights))
            {
                return new Dictionary<char, int>(weights);
            }

            return null;
        }

        /// <summary>
        /// Gets the items from the generator that follow from the specified state preceding it.
        /// </summary>
        /// <param name="state">The state preceding the items of interest.</param>
        /// <returns>A dictionary of the items and their weight.</returns>
        public Dictionary<char, int> GetNextStates(string state)
        {
            Dictionary<char, int> weights;
            if (this.items.TryGetValue(state, out weights))
            {
                return new Dictionary<char, int>(weights);
            }

            return null;
        }

        /// <summary>
        /// Randomly walks the chain.
        /// </summary>
        /// <returns>An <see cref="IEnumerable&lt;T&gt;"/> of the items chosen.</returns>
        /// <remarks>Assumes an empty starting state.</remarks>
        public string Chain()
        {
            return this.Chain("", new RandomWrapper(new Random()));
        }

        /// <summary>
        /// Randomly walks the chain.
        /// </summary>
        /// <param name="previous">The items preceding the first item in the chain.</param>
        /// <returns>An <see cref="IEnumerable&lt;T&gt;"/> of the items chosen.</returns>
        public string Chain(string previous)
        {
            return this.Chain(previous, new RandomWrapper(new Random()));
        }

        /// <summary>
        /// Randomly walks the chain.
        /// </summary>
        /// <param name="seed">The seed for the random number generator, used as the random number source for the chain.</param>
        /// <returns>An <see cref="IEnumerable&lt;T&gt;"/> of the items chosen.</returns>
        /// <remarks>Assumes an empty starting state.</remarks>
        public string Chain(int seed)
        {
            return this.Chain("", new RandomWrapper(new Random(seed)));
        }

        /// <summary>
        /// Randomly walks the chain.
        /// </summary>
        /// <param name="previous">The items preceding the first item in the chain.</param>
        /// <param name="seed">The seed for the random number generator, used as the random number source for the chain.</param>
        /// <returns>An <see cref="IEnumerable&lt;T&gt;"/> of the items chosen.</returns>
        public string Chain(string previous, int seed)
        {
            return this.Chain(previous, new RandomWrapper(new Random(seed)));
        }

        /// <summary>
        /// Randomly walks the chain.
        /// </summary>
        /// <param name="rand">The random number source for the chain.</param>
        /// <returns>An <see cref="IEnumerable&lt;T&gt;"/> of the items chosen.</returns>
        /// <remarks>Assumes an empty starting state.</remarks>
        public string Chain(Random rand)
        {
            return this.Chain("", new RandomWrapper(rand));
        }

        /// <summary>
        /// Randomly walks the chain.
        /// </summary>
        /// <param name="previous">The items preceding the first item in the chain.</param>
        /// <param name="rand">The random number source for the chain.</param>
        /// <returns>An <see cref="IEnumerable&lt;T&gt;"/> of the items chosen.</returns>
        public string Chain(string previous, Random rand)
        {
            return this.Chain(previous, new RandomWrapper(rand));
        }

        /// <summary>
        /// Randomly walks the chain.
        /// </summary>
        /// <param name="rand">The random number source for the chain.</param>
        /// <returns>An <see cref="IEnumerable&lt;T&gt;"/> of the items chosen.</returns>
        /// <remarks>Assumes an empty starting state.</remarks>
        public string Chain(RandomNumberGenerator rand)
        {
            return this.Chain("", new RandomNumberGeneratorWrapper(rand));
        }

        /// <summary>
        /// Randomly walks the chain.
        /// </summary>
        /// <param name="previous">The items preceding the first item in the chain.</param>
        /// <param name="rand">The random number source for the chain.</param>
        /// <returns>An <see cref="IEnumerable&lt;T&gt;"/> of the items chosen.</returns>
        public string Chain(string previous, RandomNumberGenerator rand)
        {
            return this.Chain(previous, new RandomNumberGeneratorWrapper(rand));
        }

        /// <summary>
        /// Randomly walks the chain.
        /// </summary>
        /// <param name="rand">The random number source for the chain.</param>
        /// <returns>An <see cref="IEnumerable&lt;T&gt;"/> of the items chosen.</returns>
        /// <remarks>Assumes an empty starting state.</remarks>
        public string Chain(IRandom rand)
        {
            return this.Chain("", rand);
        }

		/// <summary>
		/// Randomly walks the chain.
		/// </summary>
		/// <param name="previous">The items preceding the first item in the chain.</param>
		/// <param name="rand">The random number source for the chain.</param>
		/// <returns>An <see cref="IEnumerable&lt;T&gt;"/> of the items chosen.</returns>
		public string Chain(string previous, IRandom rand) {
			return new string(ChainAux(previous, rand).ToArray());
		}

        
        private IEnumerable<char> ChainAux(string previous, IRandom rand)
        {
            Queue<char> state = new Queue<char>(previous);
            while (true)
            {
                while (state.Count > this.order)
                {
                    state.Dequeue();
                }

                var key = new string(state.ToArray());

                Dictionary<char, int> weights;
                if (!this.items.TryGetValue(key, out weights))
                {
                    yield break;
                }

                int terminalWeight;
                this.terminals.TryGetValue(key, out terminalWeight);

                var total = weights.Sum(w => w.Value);
                var value = rand.Next(total + terminalWeight) + 1;

                if (value > total)
                {
                    yield break;
                }

                var currentWeight = 0;
                foreach (var nextItem in weights)
                {
                    currentWeight += nextItem.Value;
                    if (currentWeight >= value)
                    {
                        yield return nextItem.Key;
                        state.Enqueue(nextItem.Key);
                        break;
                    }
                }
            }
        }
    }
}
