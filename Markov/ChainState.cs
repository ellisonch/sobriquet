using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Markov {
	internal class ChainState : IEquatable<ChainState> {
		private string _state;
		public ChainState(Queue<char> q) {
			_state = new string(q.ToArray());
		}
		public ChainState(string s) {
			_state = s;
		}

		public bool Equals(ChainState other) {
			if (other == null) {
				return false;
			}

			return _state.Equals(other._state);
		}
		
		public override int GetHashCode() {
			var code = _state.GetHashCode();

			//for (int i = 0; i < this.items.Length; i++) {
			//	code ^= this.items[i].GetHashCode();
			//}

			return code;
		}

		public override string ToString() {
			return _state;
		}
	}
}
