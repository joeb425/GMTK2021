using UnityEngine;
using UnityEngine.UIElements;

namespace UI.HUD
{
	// TODO finish this
	public class EnemyWavePanel : GameVisualElement
	{
		private Label _waveNumber;
		private Label _enemyName;
		
		public new class UxmlFactory : UxmlFactory<EnemyWavePanel, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		public override void OnGamePreInit()
		{
			Debug.Log("Enemy wave panel Pre init?");
			Game.Get.spawnerHandler.OnNextWave += DisplayWave;
			_waveNumber = this.Q<Label>("WaveNumber");
			_enemyName = this.Q<Label>("EnemyName");
			Debug.Assert(_waveNumber != null, "_waveNumber != null");
			Debug.Assert(_enemyName != null, "_enemyName != null");
		}

		private void DisplayWave(SpawnerHandler.Wave wave, int waveIndex)
		{
			_waveNumber.text = ""  + waveIndex;
			_enemyName.text = wave.enemyPrefab.name;
		}
	}
}