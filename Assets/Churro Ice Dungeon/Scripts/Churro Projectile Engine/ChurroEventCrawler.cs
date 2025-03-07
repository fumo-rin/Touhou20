using Bremsengine;
using Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChurroIceDungeon
{
    [System.Serializable]
    public class ChurroEventCrawler : ChurroProjectileEvent
    {
        [System.Serializable]
        public struct crawlerSettings
        {
            public ChurroProjectile crawlerPrefab;
            public ChurroProjectile.ArcSettings arc;
            public float addedAimAngle;
            public float interval;
            public int repeats;
            public float addedRepeatAngle;
            public Action<ChurroProjectile> OnSpawn;
            public crawlerSettings(ChurroProjectile crawler, ChurroProjectile.ArcSettings arc, float addedAimAngle)
            {
                this.crawlerPrefab = crawler;
                this.arc = arc;
                this.addedAimAngle = addedAimAngle;
                this.interval = 0f;
                this.repeats = 0;
                this.addedRepeatAngle = 0f;
                OnSpawn = null;
            }
            public crawlerSettings SetRepeats(int repeats, float repeatTime, float addedRepeatAngle)
            {
                this.repeats = repeats;
                this.interval = repeatTime;
                this.addedRepeatAngle = addedRepeatAngle;
                return this;
            }
            public crawlerSettings AttachOnSpawnEvent(Action<ChurroProjectile> action)
            {
                OnSpawn += action;
                return this;
            }
        }
        public ChurroEventCrawler SetRepeats(int repeats, float interval, float addedRepeatAngle)
        {
            crawler.SetRepeats(repeats, interval, addedRepeatAngle);
            return this;
        }
        crawlerSettings crawler;
        public 
        bool hasSpawned = false;
        public ChurroEventCrawler(eventSettings settings, crawlerSettings crawler)
        {
            ApplySettings(settings);
            this.crawler = crawler;
        }
        public crawlerSettings AttachOnSpawnEvent(Action<ChurroProjectile> action)
        {
            crawler.OnSpawn += action;
            return crawler;
        }
        protected override void OnFirstRunPayload(ChurroProjectile eventProjectile)
        {
            IEnumerator CO_Repeat(ChurroProjectile eventProjectile)
            {
                WaitForSeconds repeatDelay = new(crawler.interval);
                for (int i = 0; i < crawler.repeats; i++)
                {
                    yield return repeatDelay;
                    SpawnCrawler(eventProjectile, crawler.addedAimAngle + crawler.addedRepeatAngle * i);
                }
            }
            SpawnCrawler(eventProjectile, crawler.addedAimAngle);
            if (eventProjectile != null && eventProjectile.isActiveAndEnabled && crawler.repeats > 0)
            {
                eventProjectile.StartCoroutine(CO_Repeat(eventProjectile));
            }
        }
        private void SpawnCrawler(ChurroProjectile owner, float rotation)
        {
            if (owner == null)
            {
                return;
            }
            ChurroProjectile.InputSettings input = new(owner.CurrentPosition, owner.CurrentVelocity.Rotate2D(rotation));
            ChurroProjectile.SpawnArc(crawler.crawlerPrefab, input, crawler.arc, out List<ChurroProjectile> p);
            foreach (var item in p)
            {
                crawler.OnSpawn?.Invoke(item);
            } 
        }
        protected override void RunPayload(ChurroProjectile eventProjectile, float deltaTime)
        {

        }
    }
}
