using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace SXLMod.Debugging
{
    public class SXLMovementTracer : MonoBehaviour
    {
        private Coroutine _routine;

        private List<LineRenderer> lines = new List<LineRenderer>();
        private LineRenderer currentLine;

        private int pointCount = 0;

        public void StartTracing()
        {
            PlayerController.Instance.respawn.OnRespawn += this.IncrementLine;
            IncrementLine();
            _routine = this.StartCoroutine(TraceMovement());
        }

        public void StopTracing()
        {
            PlayerController.Instance.respawn.OnRespawn -= this.IncrementLine;
            StopCoroutine(_routine);
            foreach(LineRenderer l in lines)
            {
                Destroy(l.gameObject);
            }
            Destroy(this);
        }

        public IEnumerator<WaitForEndOfFrame> TraceMovement()
        {
            while (currentLine != null)
            {
                currentLine.positionCount = pointCount + 1;
                Vector3 boardPosition = PlayerController.Instance.boardController.boardTransform.position;
                currentLine.SetPosition(pointCount, boardPosition);
                pointCount = currentLine.positionCount;
                yield return new WaitForEndOfFrame();
            }
        }

        private void IncrementLine()
        {
            GameObject temp = new GameObject($"MovementTracer_{lines.Count}");
            temp.transform.SetParent(SXLModManager.Instance.transform);
            LineRenderer line = temp.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.positionCount = 0;
            line.loop = false;
            line.startWidth = 0.025f;
            line.startColor = Color.red;
            line.endColor = Color.green;

            lines.Add(line);
            currentLine = line;
            pointCount = 0;
        }

    }
}
