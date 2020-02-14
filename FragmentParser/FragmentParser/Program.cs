using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FragmentParser
{
	class Program
	{
		static void Main(string[] args)
		{
			List<string> fragments = new List<string>() { "on or eat: it was a hobbit-hole", "ends of worms and an ooz", "In a hole in the ground there lived a hobbit.", "hole in the ground", "obbit. Not a nasty dirty, wet hole, filled", "oozy smell, nor yet a dry, bare", "ole, filled with the en", "it-hole, and that means comfort.", "y, bare, sandy hole with nothing in it", "h nothing in it to sit down on " };

			//List<string> fragments = new List<string>() { "all is well", "ell that en", "hat end", "t ends well" };

			Console.WriteLine("Merging fragments:" + String.Join(", ", fragments) + "\n");

			while (fragments.Count > 1)
			{
				Console.WriteLine("--------------------------------------------------");
				Console.WriteLine("Starting a pass to check for matching fragments...");
				Console.WriteLine("--------------------------------------------------\n");
				MergeCompleteMatch(ref fragments);
				MergeLargestSubstring(ref fragments);
			}

			Console.WriteLine("Fragment merging is complete. The end result is :");
			Console.WriteLine(fragments[0] + "\n");
			Console.WriteLine("Press any key to exit.");
			Console.ReadKey();
		}

		/// <summary>
		/// merges fragments that are a 100% match.
		/// </summary>
		static private void MergeCompleteMatch(ref List<string> fragments)
		{
			for (var i = 0; i < fragments.Count; i++)
			{
				var fragment = fragments[i];
				for (var j = i + 1; j < fragments.Count; j++)
				{
					if (fragment.Contains(fragments[j]))
					{
						Console.WriteLine("Found exact match in \"" + fragments[j] + "\" and \"" + fragment + "\".");
						Console.WriteLine("Removing \"" + fragments[j] + "\".\n");
						fragments.RemoveAt(j);
					}

					if (fragments[j].Contains(fragment))
					{
						Console.WriteLine("Found exact match in \"" + fragment + "\" and \"" + fragments[j] + "\".");
						Console.WriteLine("Removing \"" + fragment + "\".\n");
						fragments.RemoveAt(i);
					}
				}
			}
		}

		/// <summary>
		/// merges the longest possible substring match
		/// </summary>
		static private void MergeLargestSubstring(ref List<string> fragments)
		{
			var stringMatch = new StringMatch();
			for (var i = 0; i < fragments.Count; i++)
			{
				CheckStartOfStrings(i, ref fragments, ref stringMatch);
				CheckEndOfStrings(i, ref fragments, ref stringMatch);
			}
			if (stringMatch.SubtringIndex != 0)
				MergeFragments(ref fragments, ref stringMatch);
		}


		/// <summary>
		/// Compares the end of a fragment against all other fragment beginnings. Alters the stringMatch object if a match has the greatest length
		/// </summary>
		static private void CheckStartOfStrings(int fragmentIndex, ref List<string> fragments, ref StringMatch stringMatch)
		{
			var fragment = fragments[fragmentIndex];
			for (var ss = 1; ss < fragment.Length - 1; ss++) // start at 1 index because a full match should have already been caught in MergeCompleteMatch()
			{
				var expression = fragment.Substring(ss);
				if (expression.Length < stringMatch.MatchLength) 
					return; //exit early if we've already found the largest possible match
				for (var i = 0; i < fragments.Count - 1; i++)
				{
					if (i != fragmentIndex && !(i == stringMatch.EndStringIndex && stringMatch.StartStringIndex == fragmentIndex) ) // don't compare to self
					{
						var isMatch = Regex.IsMatch(fragments[i], @"^" + expression); // does the substring match the start of another string?
						if (isMatch)
						{
							Console.WriteLine("Found a match in \"" + expression + "\"");
							if (expression.Length > stringMatch.MatchLength) // is a match the largest match we've seen on this run?
							{
								Console.WriteLine("Match is largest seen in this pass with a length of " + expression.Length + ".\n");
								stringMatch.StartStringIndex = fragmentIndex;
								stringMatch.EndStringIndex = i;
								stringMatch.SubtringIndex = ss;
								stringMatch.MatchLength = expression.Length;
							}
							else
							{
								Console.WriteLine("Other matches are larger. Ignoring match this pass.\n");
							}
						}
					}
				}
			}

		}

		/// <summary>
		/// Compares the start of a string against all other fragment endings. Alters the stringMatch object if a match has a greater length
		/// </summary>
		static private void CheckEndOfStrings(int fragmentIndex, ref List<string> fragments, ref StringMatch stringMatch)
		{
			var fragment = fragments[fragmentIndex];
			for (var ss = fragment.Length - 1; ss > 1; ss--)
			{
				var expression = fragment.Substring(0, ss);
				if (expression.Length < stringMatch.MatchLength) 
					return; //exit early if we've already found the largest possible match
				for (var i = 0; i < fragments.Count; i++)
				{
					if (i != fragmentIndex && !(i == stringMatch.StartStringIndex && fragmentIndex == stringMatch.EndStringIndex)) // don't compare to self or a previous match
					{
						var isMatch = Regex.IsMatch(fragments[i], expression + "$"); // does the substring match the end of another string?
						if (isMatch)
						{
							Console.WriteLine("Found a match in \"" + expression + "\"");
							if (expression.Length > stringMatch.MatchLength) // is a match the largest match we've seen on this run?
							{
								Console.WriteLine("Match is largest seen in this pass with a length of " + expression.Length + ".\n");
								stringMatch.StartStringIndex = i;
								stringMatch.EndStringIndex = fragmentIndex;
								stringMatch.SubtringIndex = fragments[i].IndexOf(expression);
								stringMatch.MatchLength = expression.Length;
							}
							else
							{
								Console.WriteLine("Other matches are larger. Ignoring match this pass.\n");
							}
						}
					}
				}
			}
		}


		/// <summary>
		/// Merges fragments and removes an item from the list of strings
		/// </summary>
		static private void MergeFragments(ref List<string> fragments, ref StringMatch stringMatch)
		{
			var fragmentA = fragments[stringMatch.StartStringIndex];
			var fragmentB = fragments[stringMatch.EndStringIndex];
			fragments[stringMatch.StartStringIndex] = fragmentA.Substring(0, stringMatch.SubtringIndex) + fragmentB;
			Console.WriteLine("Merge Result is: \"" + fragments[stringMatch.StartStringIndex] + "\".\n");
			fragments.RemoveAt(stringMatch.EndStringIndex);
		}


		/// <summary>
		/// A helper class to store matching string data
		/// StartStringIndex = the index within the list for the string that comes 1st
		/// EndStringIndex = the index within the list for the string that comes last
		/// SubtringIndex = the index to begin splicing the overlapping substrings
		/// </summary>
		private class StringMatch
		{
			public int StartStringIndex { get; set; }
			public int EndStringIndex { get; set; }
			public int SubtringIndex { get; set; }
			public int MatchLength { get; set; }
		}

	}
}
