using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mantis.GameplayTags
{
	[CreateAssetMenu(menuName = "Data/GameplayTag")]
	public class GameplayTag : ScriptableObject
	{
		[SerializeField]
		private GameplayTag parent;

		[SerializeField]
		private List<GameplayTag> children = new List<GameplayTag>();

		public GameplayTag GetParent()
		{
			return parent;
		}

		public void SetParent(GameplayTag newParent)
		{
			parent = newParent;
			EditorUtility.SetDirty(this);
		}

		public bool MatchesTag(GameplayTag tagToCheck)
		{
			GameplayTag next = this;
			while (next != null)
			{
				if (next == tagToCheck)
				{
					return true;
				}
				next = next.parent;
			}

			return false;
		}

		public bool MatchesTagExact(GameplayTag tagToCheck)
		{
			return this == tagToCheck;
		}

		/// <summary>
		/// <para>Check is this gameplay tag is a descendant of another gameplay tag.</para>
		/// By default, only 4 levels of ancestors are searched.
		/// </summary>
		/// <param name="other">Ancestor gameplay tag</param>
		/// <returns>True if this gameplay tag is a descendant of the other gameplay tag</returns>
		public bool IsDescendantOf(GameplayTag other, int nSearchLimit = 4)
		{
			int i = 0;
			GameplayTag tag = parent;
			while (nSearchLimit > i++)
			{
				// tag will be invalid once we are at the root ancestor
				if (!tag) return false;

				// Match found, so we can return true
				if (tag == other) return true;

				// No match found, so try again with the next ancestor
				tag = tag.parent;
			}

			// If we've exhausted the search limit, no ancestor was found
			return false;
		}

		public void AddChild(GameplayTag child)
		{
			children.Add(child);
		}

		public void Reset()
		{
			parent = null;
			children.Clear();
		}
	}
}