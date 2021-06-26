public class GameState
{
	public static GameState Get;

	public GameBoard Board;
	
	public event System.Action<int, int> OnCashChanged;
	public event System.Action<int, int> OnLivesChanged;

	public int StartingCash;

	public int MaxLives;

	public int CurrentCash { get; private set; }

	public int CurrentLives { get; private set; }

	public void SetCash(int newCash)
	{
		int previousCash = CurrentCash;
		CurrentCash = newCash;
		OnCashChanged?.Invoke(previousCash, newCash);
	}

	public void SetLives(int newLives)
	{
		int previousLives = CurrentLives;
		CurrentLives = newLives;
		OnLivesChanged?.Invoke(previousLives, newLives);
	}

	public void Init()
	{
		Get = this;

		CurrentCash = StartingCash;
		CurrentLives = MaxLives;
	}
}
