using System;
using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 20.10.2020
     */
    public class RotationAxis : HandleBase
    {
        private Mesh _arcMesh;
        private Material _arcMaterial;
        private Vector3 _axis;
        private Vector3 _perp1;
        private Vector3 _perp2;
        private Quaternion _startRotation;
        private float _xmod = 1;
        private float _ymod = 1;

        public RotationAxis Initialize(RuntimeTransformHandle p_runtimeHandle, Vector3 p_axis, Vector3 p_perp, Color p_color)
        {
            _parentTransformHandle = p_runtimeHandle;
            _axis = p_axis;
            _perp1 = p_perp;
            _perp2 = Vector3.Cross(_axis, _perp1);
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
            return this;
        }

        public override void Interact(Vector3 p_previousPosition)
        {
            Vector3 mouseVector = (Input.mousePosition - p_previousPosition);
            mouseVector.x = _xmod * mouseVector.x;
            mouseVector.y = _ymod * mouseVector.y;
            float mag = mouseVector.magnitude;
            mouseVector = Camera.main.transform.rotation * mouseVector.normalized;

            Vector3 planeNormal = _parentTransformHandle.space == HandleSpace.LOCAL ? _parentTransformHandle.target.rotation * _axis : _axis;
            Vector3 projected = Vector3.ProjectOnPlane(mouseVector, planeNormal);
            
            projected *= Time.deltaTime * mag * 2; // Bulhar
            float d = projected.x + projected.y + projected.z;
            delta += d;

            Vector3 rotatedAxis = _startRotation * _axis;
            Vector3 invertedRotatedAxis = Quaternion.Inverse(_startRotation) * _axis;
            
            if (_parentTransformHandle.space == HandleSpace.LOCAL)
            {
                float deg = delta * 180f / Mathf.PI;
                
                if (_parentTransformHandle.rotationSnap != 0)
                    deg = Mathf.Round(deg / _parentTransformHandle.rotationSnap) * _parentTransformHandle.rotationSnap;
                
                float snappedDelta = deg * Mathf.PI / 180f;
                _arcMesh = MeshUtils.CreateArc(transform.position, _hitPoint, rotatedAxis, 2, -snappedDelta,
                    Mathf.Abs(Mathf.CeilToInt(snappedDelta * 180 / Mathf.PI)) + 1);
                DrawArc();
                
                _parentTransformHandle.target.localRotation =
                    _startRotation * Quaternion.AngleAxis(-snappedDelta * 180f / Mathf.PI, _axis);
            }
            else
            {
                float deg = delta * 180f / Mathf.PI;
                if (_parentTransformHandle.rotationSnap != 0)
                    deg = Mathf.Round(deg / _parentTransformHandle.rotationSnap) * _parentTransformHandle.rotationSnap;
                float snappedDelta = deg * Mathf.PI / 180f;
                
                //_arcMesh.mesh = MeshUtils.CreateArc(2, snappedDelta, Mathf.Abs(Mathf.CeilToInt(snappedDelta*90/Mathf.PI))+1);
                _arcMesh = MeshUtils.CreateArc(transform.position, _hitPoint, _axis, 2, -snappedDelta,
                    Mathf.Abs(Mathf.CeilToInt(snappedDelta * 180 / Mathf.PI)) + 1);
                DrawArc();

                _parentTransformHandle.target.rotation = _startRotation *
                                                         Quaternion.AngleAxis(-snappedDelta * 180f / Mathf.PI,
                                                             invertedRotatedAxis);
            }

            base.Interact(p_previousPosition);
        }

        public override void StartInteraction(Vector3 p_hitPoint)
        {
            base.StartInteraction(p_hitPoint);
            _startRotation = _parentTransformHandle.space == HandleSpace.LOCAL ? _parentTransformHandle.target.localRotation : _startRotation = _parentTransformHandle.target.rotation;

            _arcMaterial = new Material(Shader.Find("sHTiF/HandleShader"));
            _arcMaterial.color = new Color(1,1,0,.4f);
            _arcMaterial.renderQueue = 5000;
            //_arcMesh.gameObject.SetActive(true);

            Plane axisPlane;
            if (_parentTransformHandle.space == HandleSpace.LOCAL)
            {
                Vector3 rotatedAxis = _startRotation * _axis;
                axisPlane = new Plane(rotatedAxis, _parentTransformHandle.target.position);
                _hitPoint = axisPlane.ClosestPointOnPlane(_hitPoint);
            }
            else
            {
                axisPlane = new Plane(_axis, _parentTransformHandle.target.position);
                _hitPoint = axisPlane.ClosestPointOnPlane(_hitPoint);
            }

            Plane xPlane = new Plane(_parentTransformHandle.handleCamera.transform.up, _parentTransformHandle.target.position);
            Plane yPlane = new Plane(_parentTransformHandle.handleCamera.transform.right, _parentTransformHandle.target.position);
            _xmod = xPlane.SameSide(_hitPoint, _parentTransformHandle.handleCamera.transform.position) ? -1 : 1;
            _ymod = yPlane.SameSide(_hitPoint, _parentTransformHandle.handleCamera.transform.position) ? 1 : -1;
            if (!axisPlane.GetSide(_parentTransformHandle.handleCamera.transform.position)) 
            {
                _xmod = -_xmod;
                _ymod = -_ymod;
            }
        }
        
        public override void EndInteraction()
        {
            base.EndInteraction();
            //Destroy(_arcMesh.gameObject);
            delta = 0;
        }

        void DrawArc()
        {
            // _arcMaterial.SetPass(0);
            // Graphics.DrawMeshNow(_arcMesh, Matrix4x4.identity);
            Graphics.DrawMesh(_arcMesh, Matrix4x4.identity, _arcMaterial, 0);

            // GameObject arc = new GameObject();
            // MeshRenderer mr = arc.AddComponent<MeshRenderer>();
            // mr.material = new Material(Shader.Find("sHTiF/HandleShader"));
            // mr.material.color = new Color(1,1,0,.5f);
            // _arcMesh = arc.AddComponent<MeshFilter>();
        }
    }
}