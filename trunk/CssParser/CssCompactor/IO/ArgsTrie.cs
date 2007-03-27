/*-----------------------------------------------------------------------*\
	Copyright (c) 2007 Stephen M. McKamey

	CssCompactor is distributed under the terms of an MIT-style license
	http://www.opensource.org/licenses/mit-license.php
\*-----------------------------------------------------------------------*/

using System;
using System.Collections.Generic;

using BuildTools.Collections;

namespace BuildTools.IO
{
	/// <summary>
	/// Defines the prefix for mapping args to an indexed array
	/// </summary>
	public struct ArgsMap<TValue>
	{
		#region Fields

		public readonly string Prefix;
		public readonly TValue Index;

		#endregion Fields

		#region Init

		public ArgsMap(string prefix, TValue index)
		{
			if (String.IsNullOrEmpty(prefix))
			{
				throw new ArgumentNullException("prefix");
			}

			this.Prefix = prefix;
			this.Index = index;
		}

		#endregion Init
	}

	/// <summary>
	/// Creates a Trie out of ReadFilters
	/// </summary>
	public class ArgsTrie<TValue> : TrieNode<char, TValue>
	{
		#region Constants

		private const int DefaultTrieWidth = 3;
		private bool caseSensitive;

		#endregion Constants

		#region Init

		public ArgsTrie(IEnumerable<ArgsMap<TValue>> mappings) : this(mappings, false) { }

		public ArgsTrie(IEnumerable<ArgsMap<TValue>> mappings, bool caseSensitive) : base(DefaultTrieWidth)
		{
			this.caseSensitive = caseSensitive;

			// load trie
			foreach (ArgsMap<TValue> map in mappings)
			{
				TrieNode<char, TValue> node = this;

				string prefix = caseSensitive ? map.Prefix : map.Prefix.ToLowerInvariant();

				// build out the path for StartToken
				foreach (char ch in prefix)
				{
					if (!node.Contains(ch))
					{
						node = node[ch] = new TrieNode<char, TValue>(DefaultTrieWidth);
					}
					else
					{
						node = node[ch];
					}
				}

				// at the end of the Prefix is the Index
				node.Value = map.Index;
			}
		}

		#endregion Init

		#region Methods

		public Dictionary<TValue, string> MapAndTrimPrefixes(string[] args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}

			Dictionary<TValue, string> map = new Dictionary<TValue, string>(args.Length);
			foreach (string arg in args)
			{
				TrieNode<char, TValue> node = this;

				// walk each char of arg until match prefix
				for (int i=0; i<arg.Length; i++)
				{
					if (this.caseSensitive)
					{
						node = node[arg[i]];
					}
					else
					{
						node = node[Char.ToLowerInvariant(arg[i])];
					}

					if (node == null)
					{
						// no prefix found
						throw new ArgumentException("Unrecognized argument", arg);
					}

					if (node.HasValue)
					{
						// prefix found
						string value = arg.Substring(i+1);
						if (String.IsNullOrEmpty(value))
						{
							value = String.Empty;
						}
						map[node.Value] = value;
						break;
					}
				}
			}
			return map;
		}

		#endregion Methods
	}
}
