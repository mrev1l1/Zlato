using UnityEngine;	
using System.Collections;

public class SimulationEngine : MonoBehaviour {

	//  TODO: Путь к файлу нужно получать извне.
	public ConfigLoader loader = new ConfigLoader("D:\\Zlato\\NPCLifestyle\\npc_config.txt"); 

	public short satiety { get; private set; }
	public short mood { get; private set; }
	public short hygiene { get; private set; }

	public GameObject player;
	public GameObject thisNPC; //Подразумевается, что будет задаваться в Юнити вручную level-дизайнерами

	private bool isCharging;
	private float chargingTime = 0; //JUST FOR DEBBUGING
	private float elapsedTime = 0; //JUST FOR DEBBUGING

	private static int fatalMinimum = 9;
	private static float bigDistance = 100.0f;
	// Use this for initialization
	void Start () {

		loader.Initialise ();

		player = GameObject.FindGameObjectWithTag ("Player");

		satiety = (short)Random.Range (1, 24);
		mood = (short)Random.Range (1, 24);
		hygiene = (short)Random.Range (1, 24);

		foreach (var happening in loader.randomPlaces)
		{
			if(Random.Range(0.0f, 1.0f) <= happening.Value)
			{
				print("go to "+ happening.Key);
			}
			else
			{
				print("don't go to "+ happening.Key);
			}
		}

		foreach (var place in loader.timeTable) 
		{
			if(place.Key.Contains( 18 /* тут, по идее, нужно из объекта игрока брать время суток */))
			{
				thisNPC.transform.position = place.Value;
				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (IsNear()) 
		{
			if(false /* проверка на то, что NPC не взаимодействует с игроком прямо сейчас,
			          например не в диалоге \ драке*/)
			{

			}
			else
			{
				if(!isCharging)
				{
					if(satiety <= mood && satiety <= hygiene && satiety < SimulationEngine.fatalMinimum)
					{
						thisNPC.transform.position = loader.satietyLocation; // пойти поесть. в будущем заменить на перемещение, 
																			 // как оно будет готово
						// запомнить время, когда ушел, чтобы час отсчитать
						isCharging = true;
						satiety = 24; print("need food!");
						return;
					}
					if(mood <= satiety && mood <= hygiene && mood < SimulationEngine.fatalMinimum)
					{
						thisNPC.transform.position = loader.moodLocation;
						isCharging = true;
						mood = 24; print("need mood!");
						return;
					}
					if(hygiene <= satiety && hygiene <= mood && hygiene < SimulationEngine.fatalMinimum)
					{
						thisNPC.transform.position = loader.hygieneLocation;
						isCharging = true;
						hygiene = 24; print("need washing :(!");
						return;
					}

					//если прошел час, - уменьшить характеристики
					elapsedTime+=Time.deltaTime;
					if(elapsedTime > 10)
					{
						if(satiety > 0)
						{
							satiety -= (short)(1 * loader.satietyFactor);
						}
						if(mood > 0)
						{
							mood -= (short)(1 * loader.moodFactor);
						}
						if(hygiene > 0)
						{
							hygiene -= (short)(1 * loader.hygieneFactor);
						}

						elapsedTime = 0;

						print("S: " + satiety +" M: " + mood + " H: " + hygiene);
					}
				}
				else
				{
					chargingTime += Time.deltaTime;

					if(chargingTime > 10)
					{
						isCharging = false;

						foreach (var place in loader.timeTable) 
						{
							if(place.Key.Contains( 18 /* тут, по идее, нужно из объекта игрока брать время суток */))
							{
								thisNPC.transform.position = place.Value;
								break;
							}
						}

						chargingTime = 0;
					}
				}
			}
		
		} else 
		{
			foreach(var place in loader.timeTable)
			{
				if(place.Key.Contains( 18 /* тут, по идее, нужно из объекта игрока брать время суток */))
				{
				   thisNPC.transform.position = place.Value;
					break;
				}
			}
		}
	}

	bool IsNear()
	{
		Vector3 distanceVector = new Vector3 (player.transform.position.x - thisNPC.transform.position.x, 
		                                      player.transform.position.y - thisNPC.transform.position.y, 
		                                      player.transform.position.z - thisNPC.transform.position.z);

		float distance = Mathf.Sqrt (distanceVector.x * distanceVector.x+
		                             distanceVector.y * distanceVector.y+
		                             distanceVector.z * distanceVector.z);

		if (distance <= SimulationEngine.bigDistance)
			return true;

		return false;
	}
}
