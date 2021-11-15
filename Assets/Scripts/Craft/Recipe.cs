using System;
using UnityEngine;

namespace Bug.Craft
{
    [Serializable]
    public class Recipe
    {
        public string Name;
        public float CraftingTime;
        public Requirement[] Requirements;
        public GameObject Output;
    }
}
