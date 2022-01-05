using UnityEngine.UIElements;

namespace UI.HUD
{
	public class GameTopPanel : GameVisualElement
	{
		public new class UxmlFactory : UxmlFactory<GameTopPanel, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		private Label _cashLabel;
		private Label _livesLabel;
		private Button _settingsButton;

		public override void OnGameInit()
		{
			_cashLabel = this.Q<Label>("Cash");
			_livesLabel = this.Q<Label>("Lives");

			_settingsButton = this.Q<Button>("SettingsButton");
			_settingsButton.RegisterCallback<ClickEvent>(ev => GameState.Get().SlowDownGame());

			GameState.Get().OnCashChanged += (oldValue, newValue) => UpdateCashLabel(newValue);
			GameState.Get().OnLivesChanged += (oldValue, newValue) => UpdateLivesLabel(newValue);
			UpdateCashLabel(GameState.Get().CurrentCash);
			UpdateLivesLabel(GameState.Get().CurrentLives);
		}

		private void UpdateCashLabel(int lives)
		{
			_cashLabel.text = "" + lives;
		}

		private void UpdateLivesLabel(int lives)
		{
			_livesLabel.text = "" + lives;
		}
	}
}