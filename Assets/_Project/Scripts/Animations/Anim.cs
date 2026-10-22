using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Animations
{
    public static class Anim
    {
        public static class Level
        {
            public static Sequence CollapseMoveExpand(
                IList<Transform> targets, Vector3 point, bool keepFormation = true,
                GameObject spherePrefab = null, float sphereSize = 0.5f,
                float collapseTime = 0.6f, float moveTime = 1f, float expandTime = 0.6f,
                float stagger = 0f, float emission = 3f, Action<int> onHidden = null)
            {
                var master = DOTween.Sequence();
                if (targets == null || targets.Count == 0) return master;

                var groupCenter = targets.Aggregate(Vector3.zero, (s, t) => s + t.position) / targets.Count;

                for (int i = 0; i < targets.Count; i++)
                {
                    int index = i;
                    Transform target = targets[i];
                    if (target == null) continue;

                    Vector3 startPos = target.position;
                    Vector3 endPos = keepFormation ? point + (startPos - groupCenter) : point;
                    Vector3 originalScale = target.localScale;

                    var rs = target.GetComponentsInChildren<Renderer>();
                    var srcMat = rs.Length > 0 ? rs[0].sharedMaterial : null;
                    var bounds = rs.Length > 0
                        ? rs.Aggregate(rs[0].bounds, (b, r) => { b.Encapsulate(r.bounds); return b; })
                        : new Bounds(startPos, Vector3.zero);
                    Vector3 startCenter = bounds.center;
                    Vector3 offset = startCenter - startPos;

                    Color color = srcMat == null ? Color.white
                        : srcMat.HasProperty("_BaseColor") ? srcMat.GetColor("_BaseColor")
                        : srcMat.HasProperty("_Color") ? srcMat.GetColor("_Color") : Color.white;

                    GameObject sphereGo;
                    if (spherePrefab != null) sphereGo = Object.Instantiate(spherePrefab);
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
                    seq.Append(target.DOScale(0f, collapseTime).SetEase(Ease.InBack));
                    seq.Join(sphere.DOScale(sphereSize, collapseTime).SetEase(Ease.OutQuad));
                    seq.AppendCallback(() =>
                    {
                        target.gameObject.SetActive(false);
                        if (onHidden == null) return;
                        onHidden(index);              // объект сам себя вернул (Rewind)
                        endPos = target.position;     // новая позиция = цель
                    });

                    // 2. полёт (endPos читается лениво, в момент старта)
                    seq.Append(DOVirtual.Float(0f, 1f, moveTime,
                        v => sphere.position = Vector3.LerpUnclamped(startCenter, endPos + offset, v))
                        .SetEase(Ease.InOutCubic));

                    // 3. расхлопывание
                    seq.AppendCallback(() =>
                    {
                        target.position = endPos;
                        target.gameObject.SetActive(true);
                    });
                    seq.Append(target.DOScale(originalScale, expandTime).SetEase(Ease.OutBack));
                    seq.Join(sphere.DOScale(0f, expandTime).SetEase(Ease.InQuad));

                    seq.OnKill(() =>
                    {
                        if (sphere != null) Object.Destroy(sphere.gameObject);
                        if (target == null) return;
                        target.gameObject.SetActive(true);
                        target.localScale = originalScale;
                    });

                    master.Insert(i * stagger, seq);
                }
                return master;
            }
        }
    }
}