using UnityEngine;

public class InputHandler
{
	private MouseInput mouseInput;
	public Ray touchRay;

	public static InputHandler Get;

	public void Init()
	{
		Get = this;
		
		mouseInput = new MouseInput();
		mouseInput.Enable();

		mouseInput.Mouse.MouseClick.performed += ctx => MouseClick();
	}

	public void MouseClick()
	{
		GameBoard board = GameState.Get.board;
		GameTile tile = board.GetTile(touchRay);
		if (tile != null)
		{
			board.SelectTile(board.hoveredTile);
		}
	}

	public void GameUpdate()
	{
		Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
		touchRay = Camera.main.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0.0f));
	}
}