using UnityEngine;
using UnityEngine.UIElements;

namespace UI.HUD
{
	// TODO finish this
	public class EnemyWavePanel : GameVisualElement
	{
		private Label _waveNumber;
		private int _numWaves;

		public new class UxmlFactory : UxmlFactory<EnemyWavePanel, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		public override void OnGamePreInit()
		{
			Game.Get.spawnerHandler.OnNextWave += DisplayWave;
			_numWaves = GameData.Get().GetCurrentLevel().waves.Count;

			_waveNumber = this.Q<Label>("WaveNumber");

			Debug.Assert(_waveNumber != null, "_waveNumber != null");
		}

		public override void OnGameInit()
		{
			base.OnGameInit();
		}

		private void DisplayWave(SpawnerHandler.Wave wave, int waveIndex)
		{
			_waveNumber.text = $"{waveIndex + 1}/{_numWaves}";
		}
	}
}