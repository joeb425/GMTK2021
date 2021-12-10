﻿using System;
using DefaultNamespace.Data;
using Mantis.AttributeSystem;
using HexLibrary;
using Mantis.GameplayTags;
using Mantis.Hex;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

// [RequireComponent(typeof(GameplayAttributeContainer))]
public class Enemy : MonoBehaviour
{
	[SerializeField]
	public float maxHealth;

	[SerializeField]
	public AudioClip deathSfx;

	[SerializeField]
	public ParticleSystem deathParticleSystem;

	[SerializeField]
	public GameObject enemyModel;

	EnemyFactory _originFactory;

	private float _progress;

	private Slider slider;

	public event Action OnReachEnd;
	public event Action OnKilled;

	[SerializeField]
	public GameObject healthBarPrefab;

	private GameObject _healthBarInstance;

	public Quaternion desiredRotation;

	[HideInInspector]
	public GameplayAttributeContainer Attributes;

	[HideInInspector]
	public GameplayTagContainer gameplayTagContainer;

	public EnemyFactory OriginFactory
	{
		get => _originFactory;
		set
		{
			Debug.Assert(_originFactory == null, "Redefined origin factory!");
			_originFactory = value;
		}
	}

	private void Awake()
	{
		gameplayTagContainer = GetComponent<GameplayTagContainer>();
		Attributes = GetComponent<GameplayAttributeContainer>();
	}

	public void SpawnOn(Vector3 worldPos)
	{
		transform.position = worldPos;

		if (!_healthBarInstance)
		{
			_healthBarInstance = Instantiate(healthBarPrefab);
		}

		_healthBarInstance.SetActive(true);
		SetHealthbarPosition();

		slider = _healthBarInstance.GetComponentInChildren<Slider>();

		Attributes.InitAttribute(MyAttributes.Get().Health, maxHealth);
		Attributes.InitAttribute(MyAttributes.Get().MaxHealth, maxHealth);
		Attributes.InitAttribute(MyAttributes.Get().Speed, 1.0f);

		enemyModel.transform.localPosition = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, 0.0f);
	}

	public void ApplyDamage(float damage)
	{
		Debug.Assert(damage >= 0f, "Negative damage applied.");
		// Attributes.ApplyModifierDirectly(new GameplayAttributeModifier(MyAttributes.Get().Health, -1 * damage, AttributeOperator.Add));
	}

	public bool GameUpdate()
	{
		// if (GameplayTagManager.instance.RequestTag("Status.Invulnerable", out GameplayTag statusInvulnerable))
		// {
			// if (gameplayTagContainer.ContainsTag(statusInvulnerable))
			// {
				// Debug.Log("Invuln");
			// }
		// }

		Attributes.GameUpdate();

		slider.value = CalculateHealth();
		if (GetHealth() <= 0f)
		{
			_healthBarInstance.SetActive(false);
			OriginFactory.Reclaim(this);
			OnKilled?.Invoke();
			PlayDeathSfx();
			return false;
		}

		if (GetHealth() < GetMaxHealth())
		{
			_healthBarInstance.SetActive(true);
		}

		var path = GameState.Get().Board.enemyPath;

		_progress += Mathf.Clamp(Time.deltaTime * Attributes.GetCurrentValue(MyAttributes.Get().Speed), 0.0f, path.Count);
		int index = (int)Mathf.Floor(_progress);

		if (index + 1 < path.Count)
		{
			Hex hex = path[index];
			Hex nextHex = path[index + 1];

			float tileProgress = _progress - index;
			Vector3 hexFrom = GameState.Get().Board.grid.flat.HexToWorld(hex);
			Vector3 hexTo = GameState.Get().Board.grid.flat.HexToWorld(nextHex);
			transform.position = Vector3.LerpUnclamped(hexFrom, hexTo, tileProgress);

			Vector3 delta = hexTo - hexFrom;
			desiredRotation = Quaternion.LookRotation(delta, Vector3.up);
			enemyModel.transform.rotation = Quaternion.Slerp(enemyModel.transform.rotation, desiredRotation, Time.deltaTime * 5.0f);
		}
		else
		{
			// reached end
			OnReachEnd?.Invoke();
			OriginFactory.Reclaim(this);
			return false;
		}

		SetHealthbarPosition();

		return true;
	}

	void SetHealthbarPosition()
	{
		_healthBarInstance.transform.position = enemyModel.transform.position + Vector3.up * 2.0f + Vector3.forward * 0.5f;
	}

	float GetHealth()
	{
		return Attributes.GetCurrentValue(MyAttributes.Get().Health);
	}

	float GetMaxHealth()
	{
		return Attributes.GetCurrentValue(MyAttributes.Get().MaxHealth);
	}

	float CalculateHealth()
	{
		return GetHealth() / GetMaxHealth();
	}

	private void OnDestroy()
	{
		Destroy(_healthBarInstance);
	}

	private void PlayDeathSfx()
	{
		Game.Get.GetAudioHandler().PlaySfx(deathSfx);
		ParticleSystem ps = Instantiate(deathParticleSystem);
		ps.transform.position = transform.position;
		ps.Play();
		// particleSystem.Play();
	}
}