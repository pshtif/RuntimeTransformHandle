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
        protected Vector3 _perp;

        public PositionAxis Initialize(RuntimeTransformHandle p_runtimeHandle, Vector3 p_axis, Vector3 p_perp,
            Color p_color)
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
            Vector3 mouseVector = (Input.mousePosition - p_previousPosition);
            float mag = mouseVector.magnitude;
            mouseVector = Camera.main.transform.rotation * mouseVector.normalized;
        
            Vector3 rperp = _parentTransformHandle.space == HandleSpace.LOCAL
                ? _parentTransformHandle.target.rotation * _perp
                : _perp;
            Vector3 projected = Vector3.ProjectOnPlane(mouseVector, rperp);
        
            projected *= Time.deltaTime * mag * RuntimeTransformHandle.MOUSE_SENSITIVITY;
            Vector3 raxis = _parentTransformHandle.space == HandleSpace.LOCAL
                ? _parentTransformHandle.target.rotation * _axis
                : _axis;
            float d = raxis.x * projected.x + raxis.y * projected.y + raxis.z * projected.z;
        
            delta += d;
            Vector3 snappingVector = _parentTransformHandle.positionSnap;
            float snap = Vector3.Scale(snappingVector, _axis).magnitude;
            float snappedDelta = (snap == 0 || _parentTransformHandle.snappingType == HandleSnappingType.ABSOLUTE) ? delta : Mathf.Round(delta / snap) * snap;
            Vector3 position = _startPosition + raxis * snappedDelta;
            if (snap != 0 && _parentTransformHandle.snappingType == HandleSnappingType.ABSOLUTE)
            {
                if (snappingVector.x != 0) position.x = Mathf.Round(position.x / snappingVector.x) * snappingVector.x;
                if (snappingVector.y != 0) position.y = Mathf.Round(position.y / snappingVector.y) * snappingVector.y;
                if (snappingVector.z != 0) position.z = Mathf.Round(position.z / snappingVector.z) * snappingVector.z;
            }
            
            _parentTransformHandle.target.position = position;
        
            base.Interact(p_previousPosition);
        }
        
        public override void StartInteraction(Vector3 p_hitPoint)
        {
            base.StartInteraction(p_hitPoint);
            _startPosition = _parentTransformHandle.target.position;
        }
    }
}