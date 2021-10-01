using System.Collections.Generic;
using System.Linq;
using Misc;
using UnityEditor;
using UnityEngine;

namespace GameplayTags
{
	public class GameplayTagManager
	{
		private bool _isLoaded = false;
		private Dictionary<string, GameplayTag> _gameplayTagsByName = new Dictionary<string, GameplayTag>();

		public GameplayTagManager()
		{
			LoadTags();
		}

		public bool RequestTag(string tagName, out GameplayTag tag)
		{
			LoadTags();
			return _gameplayTagsByName.TryGetValue(tagName, out tag);
		}

		public bool IsLoaded()
		{
			return _isLoaded;
		}

		public void LoadTags()
		{
			if (IsLoaded())
			{
				return;
			}

			_gameplayTagsByName = new Dictionary<string, GameplayTag>();

			_isLoaded = true;
			var tags = CustomAssetUtils.FindAssetsByType<GameplayTag>();
			foreach (GameplayTag tag in tags)
			{
				Debug.Log($"Initial load tag {tag.name}");
				if (!_gameplayTagsByName.ContainsKey(tag.name))
				{
					_gameplayTagsByName.Add(tag.name, tag);
				}
			}
		}

		public void ReadTagsGoogleDocs()
		{
			LoadTags();

			// rebuild every tag 
			foreach (var keyValuePair in _gameplayTagsByName)
			{
				keyValuePair.Value.Reset();
			}
			
			LoadGoogleDocs.Load(
				"https://docs.google.com/spreadsheets/d/e/2PACX-1vTLjwrRjkQUgdmoWzjUyMSnbwqe1pX1ZXw_tQLKoRSOnTsu9Mh61Vp9kKJgtR2sKmKbN7cCy0f9VKLs/pub?gid=991045290&single=true&output=csv",
				OnLoadedTags);
		}

		public void OnLoadedTags(string fileData)
		{
			foreach (var line in CustomAssetUtils.ReadLines(fileData))
			{
				foreach (string tagName in line.Split(',').Where(s => s.Length > 0))
				{
					string[] splitTags = tagName.Split('.');
					GameplayTag parent = null;
					string lastTag = "";
					foreach (string splitTag in splitTags)
					{
						lastTag += splitTag;
						// Debug.Log($"$Tag: '{lastTag}' '{splitTag}'");
						parent = FindOrCreateTag(lastTag, parent);
						lastTag += ".";
					}
				}
			}
		}

		public GameplayTag FindOrCreateTag(string tagName, GameplayTag parent)
		{
			if (!RequestTag(tagName, out GameplayTag tag))
			{
				string filePath = $"Assets/Data/Tags/{tagName}.asset";
				// Debug.Log($"Create Asset at: {filePath}");
				tag = ScriptableObject.CreateInstance(typeof(GameplayTag)) as GameplayTag;
				AssetDatabase.CreateAsset(tag, filePath);
			}

			if (!tag)
			{
				return null;
			}

			if (parent)
			{
				_gameplayTagsByName[tag.name] = tag;
				tag.SetParent(parent);
				parent.AddChild(tag);
			}

			return tag;
		}
		
	}
}