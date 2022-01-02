using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DefaultNamespace
{
	public class LoadScene : MonoBehaviour
	{
		[SerializeField]
		private AssetReference scene;

		private void Awake()
		{
			var op = Addressables.LoadSceneAsync(scene);
			op.Completed += handle =>
			{
			};
		}
	}
}