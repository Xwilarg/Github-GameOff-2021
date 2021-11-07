﻿using UnityEngine;

namespace Bug.Player
{
    public class CarriedObject
    {
        public CarriedObject(Transform player, GameObject prefab)
        {
            _prefab = prefab;
            _hint = Object.Instantiate(prefab, player.transform.position + player.transform.forward, player.transform.rotation);
        }

        public void UpdatePosition(Transform player)
        {
            _hint.transform.position = player.transform.position + player.transform.forward;
            _hint.transform.rotation = player.transform.rotation;
        }

        private GameObject _hint;
        private GameObject _prefab;
    }
}