using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu]
public class GameWorldData : ScriptableObject
{
	public int LevelId;
	public List<float2> MobSpawnPoints;
	public List<float2> PotSpawnPoints;
	public PropSettings Pot;
	public MobSettings Eye;
	public MobSettings Golem;
	public ChestSettings Chest;
	public float ChestSpawnProb;
	public float2 ItemSpawnPoint => float2.zero; 
}