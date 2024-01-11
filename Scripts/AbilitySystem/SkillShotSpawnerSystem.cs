﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AbilitySystem
{
    public class SkillShotSpawnerSystem : MonoBehaviour
    {
        public Canvas skillShotsCanvas;
        public GameObject fireBreathPrefab; // Specific prefab with FireBreathInitialBorders and FireBreathProgress
        public float effectDuration = 3.0f;

        // ABILITY ASSETS
        public FireBreath assetBasicFireBreath;
        public Fireball assetBasicFireball;
        public ExplosionZone assetExplosionZone;
        public DamagePool assetDamagePool;

        public AbilityController abilityController; // The ability controller

        private void Update()
        {
            // DEBUG PURPOSES
            // Если нажата ЛКМ
            if (Input.GetMouseButtonDown(0))
            {
                // Находим всех персонажей с тегом "Player"
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                // Находим Босса
                GameObject boss = GameObject.FindGameObjectWithTag("Boss");
                if (players.Length > 0 && boss != null)
                {
                    // Выбираем случайного персонажа
                    GameObject target = players[Random.Range(0, players.Length)];

                    // Вычисляем угол поворота от Босса к игроку
                    Quaternion rotation = Quaternion.LookRotation(target.transform.position - boss.transform.position);

                    // Создаем новый огненный шар и активируем его в направлении выбранного персонажа

                    // Устанавливаем способность для контроллера
                    // abilityController.ability = assetBasicFireball;

                    // Активируем способность
                    // abilityController.ActivateAbility(boss.transform.position, rotation);

                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                // Находим всех персонажей с тегом "Player"
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                // Находим Босса
                GameObject boss = GameObject.FindGameObjectWithTag("Boss");
                if (players.Length > 0 && boss != null)
                {
                    // Выбираем случайного персонажа
                    GameObject target = players[Random.Range(0, players.Length)];

                    // Вычисляем угол поворота от Босса к игроку
                    Quaternion rotation = Quaternion.LookRotation(target.transform.position - boss.transform.position);

                    // Создаем новое огненное дыхание и активируем его в направлении выбранного персонажа
                    FireBreath fireBreath = Instantiate(assetBasicFireBreath);
                    fireBreath.Activate(boss.transform.position, rotation);
                }
            }
        }

        public void SpawnFireBreathEffect(Vector3 position, Quaternion rotation)
        {
            GameObject instance = Instantiate(fireBreathPrefab, position, rotation, skillShotsCanvas.transform);

            // Устанавливаем ротацию только для оси Y
            instance.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,  0, 0);

            FireBreath newFireBreath = Instantiate(assetBasicFireBreath);
            newFireBreath.Activate();
            // StartCoroutine(newFireBreath.PerformEffect());
        }

        private IEnumerator PerformFireBreathEffect(GameObject fireBreathInstance)
        {
            Image fireBreathInitialBorders = fireBreathInstance.transform.Find("FireBreathInitialBorders").GetComponent<Image>();
            Image fireBreathProgress = fireBreathInstance.transform.Find("FireBreathProgress").GetComponent<Image>();

            float timePassed = 0.0f;
            while (timePassed < effectDuration)
            {
                timePassed += Time.deltaTime;
                fireBreathProgress.fillAmount = timePassed / effectDuration;
                yield return null;
            }

            StartCoroutine(FadeOutImages(fireBreathInitialBorders, fireBreathProgress));
        }

        private IEnumerator FadeOutImages(params Image[] imageList)
        {
            float fadeDuration = 0.5f; // duration of fade out effect you can adjust
            for (float t = 0.0f; t < fadeDuration; t += Time.deltaTime)
            {
                float normalizedTime = t / fadeDuration;
                foreach (var image in imageList)
                {
                    if (image != null)
                    {
                        Color c = image.color;
                        c.a = 1.0f - normalizedTime;
                        image.color = c;
                    }   
                }
                yield return null;
            }

            foreach (var image in imageList)
            {
                if (image != null)
                {
                    Destroy(image.gameObject.transform.parent.gameObject);
                }
            }
        }
    }
}