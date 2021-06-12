using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
	EnemyFactory originFactory;
	GameTile tileFrom, tileTo;
	Vector3 positionFrom, positionTo;
	float progress;
	Direction direction;
	DirectionChange directionChange;
	float directionAngleFrom, directionAngleTo;

	public float Health { get; set; } = 500.0f;
	public float maxHealth;
	public GameObject healthBarUI;
	public Slider slider;
	public EnemyFactory OriginFactory
	{
		get => originFactory;
		set
		{
			Debug.Assert(originFactory == null, "Redefined origin factory!");
			originFactory = value;
		}
	}

	public void SpawnOn(GameTile tile)
	{
		//	transform.localPosition = tile.transform.localPosition;
		Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go!", this);
		tileFrom = tile;
		tileTo = tile.NextTileOnPath;
		maxHealth = Health;
		//positionFrom = tileFrom.transform.localPosition;
		//positionTo = tileFrom.ExitPoint;
		//transform.localRotation = tileFrom.PathDirection.GetRotation();
		progress = 0f;
		PrepareIntro();
	}

	void PrepareIntro()
	{
		positionFrom = tileFrom.transform.localPosition;
		positionTo = tileFrom.ExitPoint;
		direction = tileFrom.PathDirection;
		directionChange = DirectionChange.None;
		directionAngleFrom = directionAngleTo = direction.GetAngle();
		transform.localRotation = direction.GetRotation();
	}

	public void ApplyDamage(float damage)
	{
		Debug.Assert(damage >= 0f, "Negative damage applied.");
		Health -= damage;
	}

	public bool GameUpdate()
	{
		slider.value = CalculateHealth();
		if (Health <= 0f)
		{
			OriginFactory.Reclaim(this);
			return false;
		}

		if (Health < maxHealth)
		{
			healthBarUI.SetActive(true);
		}

		progress += Time.deltaTime;
		while (progress >= 1f)
		{
			tileFrom = tileTo;
			tileTo = tileTo.NextTileOnPath;
			// check for end tile
			if (tileTo == null)
			{
				OriginFactory.Reclaim(this);
				return false;
			}

			//positionFrom = positionTo;
			//positionTo = tileFrom.ExitPoint;
			//transform.localRotation = tileFrom.PathDirection.GetRotation();
			progress -= 1f;
			PrepareNextState();
		}

		transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);
		return true;
	}

	float CalculateHealth()
	{
		return Health / maxHealth;
	}
	void PrepareNextState()
	{
		positionFrom = positionTo;
		positionTo = tileFrom.ExitPoint;
		directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
		direction = tileFrom.PathDirection;
		directionAngleFrom = directionAngleTo;
		switch (directionChange)
		{
			case DirectionChange.None:
				PrepareForward();
				break;
			case DirectionChange.TurnRight:
				PrepareTurnRight();
				break;
			case DirectionChange.TurnLeft:
				PrepareTurnLeft();
				break;
			default:
				PrepareTurnAround();
				break;
		}
	}

	void PrepareForward()
	{
		transform.localRotation = direction.GetRotation();
		directionAngleTo = direction.GetAngle();
	}

	void PrepareTurnRight()
	{
		directionAngleTo = directionAngleFrom + 90f;
	}

	void PrepareTurnLeft()
	{
		directionAngleTo = directionAngleFrom - 90f;
	}

	void PrepareTurnAround()
	{
		directionAngleTo = directionAngleFrom + 180f;
	}

	private void OnCollisionEnter(Collision other)
	{
		Debug.Log("Collision?");
	}
}