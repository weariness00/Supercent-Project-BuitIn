using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class MovePath : MonoBehaviour
    {
        public Transform[] pathTransform;

        public Vector3[] PathPointArray => pathTransform.Select(t => t.position).ToArray();
        public List<Vector3> PathPointList => pathTransform.Select(t => t.position).ToList();
    }
}

