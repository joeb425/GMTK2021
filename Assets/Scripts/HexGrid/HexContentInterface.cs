namespace DefaultNamespace.HexGrid
{
	public interface IHexContent
	{
		public void SetTileType(int index);
		// public void OnCreate(HexTileSpawnInfo Info);
		public void OnDestroy();
		public void OnAdd();
	}
}