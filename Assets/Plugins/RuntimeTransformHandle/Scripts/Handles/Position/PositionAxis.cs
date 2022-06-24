using System;
using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 20.10.2020
     */
    public class PositionAxis : HandleBase
    {
        protected Vector3 _startPosition;
        protected Vector3 _axis;

        private Vector3 _interactionOffset;
        private Ray     _raxisRay;

        public PositionAxis Initialize(RuntimeTransformHandle p_runtimeHandle, Vector3 p_axis, Color p_color)
        {
            _parentTransformHandle = p_runtimeHandle;
            _axis = p_axis;
            _defaultColor = p_color;
            
            InitializeMaterial();

            transform.SetParent(p_runtimeHandle.transform, false);

            GameObject o = new GameObject();
            o.transform.SetParent(transform, false);
            MeshRenderer mr = o.AddComponent<MeshRenderer>();
            mr.material = _material;
            MeshFilter mf = o.AddComponent<MeshFilter>();
            mf.mesh = MeshUtils.CreateCone(2f, .02f, .02f, 8, 1);
            MeshCollider mc = o.AddComponent<MeshCollider>();
            mc.sharedMesh = MeshUtils.CreateCone(2f, .1f, .02f, 8, 1);
            o.transform.localRotation = Quaternion.FromToRotation(Vector3.up, p_axis);

            o = new GameObject();
            o.transform.SetParent(transform, false);
            mr = o.AddComponent<MeshRenderer>();
            mr.material = _material;
            mf = o.AddComponent<MeshFilter>();
            mf.mesh = MeshUtils.CreateCone(.4f, .2f, .0f, 8, 1);
            mc = o.AddComponent<MeshCollider>();
            o.transform.localRotation = Quaternion.FromToRotation(Vector3.up, _axis);
            o.transform.localPosition = p_axis * 2;

            return this;
        }

        public override void Interact(Vector3 p_previousPosition)
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            float   closestT = HandleMathUtils.ClosestPointOnRay(_raxisRay, cameraRay);
            Vector3 hitPoint = _raxisRay.GetPoint(closestT);
            
            Vector3 offset = hitPoint + _interactionOffset - _startPosition;
            
            Vector3 snapping = _parentTransformHandle.positionSnap;
            float   snap     = Vector3.Scale(snapping, _axis).magnitude;
            if (snap != 0 && _parentTransformHandle.snappingType == HandleSnappingType.RELATIVE)
            {
                offset = (Mathf.Round(offset.magnitude / snap) * snap) * offset.normalized; 
            }

            Vector3 position = _startPosition + offset;
            
            if (snap != 0 && _parentTransformHandle.snappingType == HandleSnappingType.ABSOLUTE)
            {
                if (snapping.x != 0) position.x = Mathf.Round(position.x / snapping.x) * snapping.x;
                if (snapping.y != 0) position.y = Mathf.Round(position.y / snapping.y) * snapping.y;
                if (snapping.x != 0) position.z = Mathf.Round(position.z / snapping.z) * snapping.z;
            }
            
            _parentTransformHandle.target.position = position;

            base.Interact(p_previousPosition);
        }
        
        public override void StartInteraction(Vector3 p_hitPoint)
        {
            base.StartInteraction(p_hitPoint);
            
            _startPosition = _parentTransformHandle.target.position;

            Vector3 raxis = _parentTransformHandle.space == HandleSpace.LOCAL
                ? _parentTransformHandle.target.rotation * _axis
                : _axis;
            
            _raxisRay = new Ray(_startPosition, raxis);

            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            float closestT = HandleMathUtils.ClosestPointOnRay(_raxisRay, cameraRay);
            Vector3 hitPoint = _raxisRay.GetPoint(closestT);
            
            _interactionOffset = _startPosition - hitPoint;
        }
    }
}