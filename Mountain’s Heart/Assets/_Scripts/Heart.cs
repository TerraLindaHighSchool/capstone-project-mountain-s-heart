using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
        private Sprite sprite;
        private int value;

        public Heart(Sprite s, int i)
        {
            sprite = s;
            value = i;
        }
}