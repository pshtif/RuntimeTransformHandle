using System;
using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 20.10.2020
     */
    public class RotationAxis : HandleBase
    {
        private MeshFilter _arcMesh;
        private Vector3 _axis;
        private Vector3 _perp;
        private Quaternion _startRotation;

        public RotationAxis Initialize(RuntimeTransformHandle p_runtimeHandle, Vector3 p_axis, Vector3 p_perp, Color p_color)
        {
            _parentTransformHandle = p_runtimeHandle;
            _axis = p_axis;
            _perp = p_perp;
            _defaultColor = p_color;
            
            InitializeMaterial();

            transform.SetParent(p_runtimeHandle.transform, false);

            GameObject o = new GameObject();
            o.transform.SetParent(transform, false);
            MeshRenderer mr = o.AddComponent<MeshRenderer>();
            mr.material = _material;
            MeshFilter mf = o.AddComponent<MeshFilter>();
            mf.mesh = MeshUtils.CreateTorus(2f, .02f, 32, 6);
            MeshCollider mc = o.AddComponent<MeshCollider>();
            mc.sharedMesh = MeshUtils.CreateTorus(2f, .1f, 32, 6);
            o.transform.localRotation = Quaternion.FromToRotation(Vector3.up, _axis);

            o = new GameObject();
            o.transform.SetParent(transform, false);
            mr = o.AddComponent<MeshRenderer>();
            mr.material = new Material(Shader.Find("sHTiF/HandleShader"));
            mr.material.color = new Color(1,1,0,.5f);
            _arcMesh = o.AddComponent<MeshFilter>();
            o.transform.localRotation = Quaternion.FromToRotation(Vector3.up, _axis);
            o.SetActive(false);

            return this;
        }

        public override void Interact(Vector3 p_previousPosition)
        {
            Vector3 mouseVector = (Input.mousePosition - p_previousPosition);
            float mag = mouseVector.magnitude;
            mouseVector = Camera.main.transform.rotation * mouseVector.normalized;

            Vector3 rperp = _parentTransformHandle.space == HandleSpace.LOCAL ? _parentTransformHandle.target.rotation * _axis : _axis;
            Vector3 projected = Vector3.ProjectOnPlane(mouseVector, rperp);
            
            projected *= Time.deltaTime * mag * 2; // Bulhar
            float d = _perp.x * projected.x + _perp.y * projected.y + _perp.z * projected.z;
            delta += d;

            if (_parentTransformHandle.space == HandleSpace.LOCAL)
            {
                float deg = delta * 180f / Mathf.PI;
                if (_parentTransformHandle.rotationSnap != 0)
                    deg = Mathf.Round(deg / _parentTransformHandle.rotationSnap) * _parentTransformHandle.rotationSnap;
                float snappedDelta = deg * Mathf.PI / 180f;

                _arcMesh.mesh = MeshUtils.CreateArc(2, -snappedDelta, Mathf.Abs(Mathf.CeilToInt(snappedDelta*90/Mathf.PI))+1);
                _arcMesh.transform.localRotation =
                    Quaternion.FromToRotation(Vector3.up, _axis) * Quaternion.AngleAxis(-snappedDelta, Vector3.up);
                _parentTransformHandle.target.localRotation =
                    _startRotation * Quaternion.AngleAxis(-snappedDelta * 180f / Mathf.PI, _axis);
            }
            else
            {
                Vector3 raxis = Quaternion.Inverse(_startRotation) * _axis;
                
                float deg = delta * 180f / Mathf.PI;
                if (_parentTransformHandle.rotationSnap != 0)
                    deg = Mathf.Round(deg / _parentTransformHandle.rotationSnap) * _parentTransformHandle.rotationSnap;
                float snappedDelta = deg * Mathf.PI / 180f;
                
                _arcMesh.mesh = MeshUtils.CreateArc(2, snappedDelta, Mathf.Abs(Mathf.CeilToInt(snappedDelta*90/Mathf.PI))+1);
                _arcMesh.transform.localRotation =
                    Quaternion.FromToRotation(Vector3.up, _axis) * Quaternion.AngleAxis(-snappedDelta, Vector3.up);
                _parentTransformHandle.target.rotation = _startRotation * Quaternion.AngleAxis(-snappedDelta * 180f / Mathf.PI, raxis);
            }

            base.Interact(p_previousPosition);
        }

        public override void StartInteraction()
        {
            base.StartInteraction();
            
            _startRotation = _parentTransformHandle.space == HandleSpace.LOCAL ? _parentTransformHandle.target.localRotation : _startRotation = _parentTransformHandle.target.rotation;
            
            _arcMesh.gameObject.SetActive(true);
        }
        
        public override void EndInteraction()
        {
            base.EndInteraction();
            _arcMesh.gameObject.SetActive(false);
            _arcMesh.mesh = null;
            delta = 0;
        }
    }
}