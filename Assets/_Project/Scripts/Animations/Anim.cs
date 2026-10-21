using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Animations
{
    public class Anim
    {
        public class Level
        {
            public static Sequence CollapseMoveExpand(
                IList<Transform> targets,
                Vector3 point,
                bool keepFormation = true,
                GameObject spherePrefab = null,
                float sphereSize = 0.5f,
                float collapseTime = 0.6f,
                float moveTime = 1f,
                float expandTime = 0.6f,
                float stagger = 0f,
                float emission = 3f,
                System.Action<int> onHidden = null)
            {
                var master = DOTween.Sequence();
                if (targets == null || targets.Count == 0) return master;

                Vector3 groupCenter = Vector3.zero;
                foreach (var t in targets) groupCenter += t.position;
                groupCenter /= targets.Count;

                for (int i = 0; i < targets.Count; i++)
                {
                    int index = i;
                    Transform target = targets[i];
                    if (target == null) continue;

                    Vector3 startPos = target.position;
                    Vector3 endPos = keepFormation ? point + (startPos - groupCenter) : point;
                    Vector3 originalScale = target.localScale;

                    var renderers = target.GetComponentsInChildren<Renderer>();
                    Vector3 startCenter = startPos;
                    Color color = Color.white;
                    Material srcMat = null;
                    if (renderers.Length > 0)
                    {
                        Bounds b = renderers[0].bounds;
                        for (int r = 1; r < renderers.Length; r++) b.Encapsulate(renderers[r].bounds);
                        startCenter = b.center;

                        srcMat = renderers[0].sharedMaterial;
                        if (srcMat != null)
                        {
                            if (srcMat.HasProperty("_BaseColor")) color = srcMat.GetColor("_BaseColor");
                            else if (srcMat.HasProperty("_Color")) color = srcMat.GetColor("_Color");
                        }
                    }
                    Vector3 endCenter = startCenter + (endPos - startPos);

                    GameObject sphereGo;
                    if (spherePrefab != null)
                    {
                        sphereGo = Object.Instantiate(spherePrefab);
                    }
                    else
                    {
                        sphereGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        Object.Destroy(sphereGo.GetComponent<Collider>());
                        if (srcMat != null) sphereGo.GetComponent<Renderer>().material = new Material(srcMat);
                    }
                    foreach (var r in sphereGo.GetComponentsInChildren<Renderer>())
                    {
                        var m = r.material;
                        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
                        if (m.HasProperty("_Color")) m.SetColor("_Color", color);
                        if (m.HasProperty("_EmissionColor"))
                        {
                            m.EnableKeyword("_EMISSION");
                            m.SetColor("_EmissionColor", color * emission);
                        }
                    }
                    foreach (var l in sphereGo.GetComponentsInChildren<Light>()) l.color = color;

                    Transform sphere = sphereGo.transform;
                    sphere.position = startCenter;
                    sphere.localScale = Vector3.zero;

                    var seq = DOTween.Sequence();

                    // 1. схлопывание
                    seq.Append(target.DOScale(Vector3.zero, collapseTime).SetEase(Ease.InBack));
                    seq.Join(sphere.DOScale(Vector3.one * sphereSize, collapseTime).SetEase(Ease.OutQuad));
                    seq.AppendCallback(() =>
                    {
                        target.gameObject.SetActive(false);
                        if (onHidden != null)
                        {
                            onHidden(index);                 // объект сам себя вернул (Rewind)
                            endPos = target.position;        // берём новую позицию как цель
                            endCenter = startCenter + (endPos - startPos);
                        }
                    });

                    // 2. полёт (конец считается в момент старта, поэтому DOVirtual)
                    seq.Append(DOVirtual.Float(0f, 1f, moveTime,
                        v => sphere.position = Vector3.LerpUnclamped(startCenter, endCenter, v))
                        .SetEase(Ease.InOutCubic));

                    // 3. расхлопывание
                    seq.AppendCallback(() =>
                    {
                        target.position = endPos;
                        target.gameObject.SetActive(true);
                    });
                    seq.Append(target.DOScale(originalScale, expandTime).SetEase(Ease.OutBack));
                    seq.Join(sphere.DOScale(Vector3.zero, expandTime).SetEase(Ease.InQuad));

                    seq.OnKill(() =>
                    {
                        if (sphere != null) Object.Destroy(sphere.gameObject);
                        if (target != null)
                        {
                            target.gameObject.SetActive(true);
                            target.localScale = originalScale;
                        }
                    });

                    master.Insert(i * stagger, seq);
                }

                return master;
            }
        }
    }
}