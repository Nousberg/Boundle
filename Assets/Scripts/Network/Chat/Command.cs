using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using Assets.Scripts.Entities;

namespace Assets.Scripts.Network.Chat
{
    public abstract class Command
    {
        public event Action<string> OnFailureExecution;
        public event Action<string> OnSuccessExecution;

        public abstract bool Execute(EntityNetworkData initiator, params string[] args);
        public abstract bool CanExecute(EntityNetworkData.Rights rights);

        protected List<GameObject> GetTarget(Transform initiator, Selector selector)
        {
            GameObject[] targets = UnityEngine.Object.FindObjectsOfType<GameObject>();
            List<GameObject> result = new List<GameObject>();

            if (targets != null)
            {
                switch (selector.filter)
                {
                    case Filter.All:
                        result = targets.ToList();
                        break;

                    case Filter.Nearest:
                        var nearest = targets
                            .Where(n => n != initiator.gameObject)
                            .OrderBy(n => Vector3.Distance(initiator.position, n.transform.position))
                            .FirstOrDefault();

                        if (nearest != null)
                            result = new List<GameObject> { nearest };
                        break;

                    case Filter.Self:
                        return new List<GameObject> { initiator.gameObject };
                    case Filter.Others:
                        result = targets
                            .Where(n => n != initiator.gameObject)
                            .ToList();
                        break;
                }
            }
            else
            {
                return result;
            }

            if (selector.nametag.Count > 0)
            {
                List<GameObject> targetsWithNametag = new List<GameObject>();

                foreach (var nametag in selector.nametag)
                {
                    foreach (var element in result)
                    {
                        if (element.TryGetComponent<EntityNetworkData>(out var entity) && entity.Nametag == nametag)
                        {
                            targetsWithNametag.Add(entity.gameObject);
                        }
                    }
                }

                result = targetsWithNametag;
            }

            if (selector.targets.Count >  0)
            {
                List<GameObject> entities = new List<GameObject>();

                foreach (var element in result)
                {
                    if (element.TryGetComponent<Entity>(out var e) && selector.targets.Contains(e.MyType))
                    {
                        entities.Add(element);
                    }
                }

                result = entities;
            }


            foreach (var radius in selector.radius)
            {
                Debug.Log(selector.radius.IndexOf(radius) + "R: " + radius);
                List<GameObject> filteredObjects = result.FindAll(n => Vector3.Distance(n.transform.position, initiator.position) > radius);

                if (filteredObjects != null)
                {
                    filteredObjects.ForEach(n => result.Remove(n));
                }
            }

            if (result.Count < 1)
                ThrowMissingTarget(selector);

            return result;
        }

        protected Selector ParseSelector(int argument, string data)
        {
            Selector result = new Selector(Filter.Self, new List<EntityType>(), new List<string>(), new List<float>());

            MatchCollection entitySelectors = Regex.Matches(data, "type=([^,)(]+)");
            MatchCollection radiusSelectors = Regex.Matches(data, "radius=([^,)(]+)");
            MatchCollection nametagSelectors = Regex.Matches(data, "nametag=([^,)(]+)");

            try
            {
                foreach (Match entity in entitySelectors)
                {
                    var parsedType = Enum.Parse<EntityType>(entity.Groups[1].Value.Replace(")", ""), ignoreCase: true);
                    result.targets.Add(parsedType);
                }

                foreach (Match radius in radiusSelectors)
                {
                    float parsedRadius = float.Parse(radius.Groups[1].Value.Replace(")", ""));
                    result.radius.Add(parsedRadius);
                }

                foreach (Match nametag in nametagSelectors)
                {
                    string parsedNametag = nametag.Groups[1].Value.Replace(")", "");
                    Debug.Log(parsedNametag);
                    result.nametag.Add(parsedNametag);
                }

                result.filter = GetFilter(data);
            }
            catch (Exception ex)
            {
                ThrowInvalidArgument(argument, ex.Message);
            }

            return result;
        }

        private Filter GetFilter(string data)
        {
            string filterPattern = @"^[^(]*";
            Match filter = Regex.Match(data, filterPattern);

            if (filter.Success && Enum.TryParse<Filter>(filter.Value, ignoreCase: true, out Filter result))
            {
                return result;
            }

            return Filter.Self;
        }

        protected void ThrowMissingArgument(int argument, string context)
        {
            OnFailureExecution?.Invoke($"Missing {context} at {argument}");
        }

        protected void ThrowInvalidArgument(int argument, string context)
        {
            OnFailureExecution?.Invoke($"Unexpected {context} at {argument}");
        }

        protected void ThrowMissingTarget(Selector selector)
        {
            OnFailureExecution?.Invoke($"No target match selector: F={selector.filter}, N={string.Join(", ", JsonUtility.ToJson(selector.nametag))}, R={string.Join(", ", JsonUtility.ToJson(selector.radius))}, targets={string.Join(", ", JsonUtility.ToJson(selector.targets))}");
        }

        protected void SuccessExecution(string context)
        {
            OnSuccessExecution?.Invoke(context);
        }

        public enum Filter : byte
        {
            All,
            Self,
            Nearest,
            Others
        }

        public class Selector
        {
            public Filter filter;
            public List<EntityType> targets;
            public List<string> nametag;
            public List<float> radius;

            public Selector(Filter filter, List<EntityType> targets, List<string> nametag, List<float> radius)
            {
                this.filter = filter;
                this.targets = targets;
                this.nametag = nametag;
                this.radius = radius;
            }
        }
    }
}
