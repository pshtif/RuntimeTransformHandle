using System.IO;
using System.Security.Permissions;
using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 20.10.2020
     */
    public class ScaleAxis : HandleBase
    {
        private const float SIZE = 2;
        
        private Vector3 _axis;
        private Vector3 _perp;
        private Vector3 _startScale;

        public ScaleAxis Initialize(RuntimeTransformHandle p_parentTransformHandle, Vector3 p_axis, Vector3 p_perp,
            Color p_color)
        {
            _parentTransformHandle = p_parentTransformHandle;
            _axis = p_axis;
            _perp = p_perp;
            _defaultColor = p_color;

            InitializeMaterial();

            transform.SetParent(p_parentTransformHandle.transform, false);

            GameObject o = new GameObject();
            o.transform.SetParent(transform, false);
            MeshRenderer mr = o.AddComponent<MeshRenderer>();
            mr.material = _material;
            MeshFilter mf = o.AddComponent<MeshFilter>();
            mf.mesh = MeshUtils.CreateCone(p_axis.magnitude * SIZE, .02f, .02f, 8, 1);
            MeshCollider mc = o.AddComponent<MeshCollider>();
            mc.sharedMesh = MeshUtils.CreateCone(p_axis.magnitude * SIZE, .1f, .02f, 8, 1);
            o.transform.localRotation = Quaternion.FromToRotation(Vector3.up, p_axis);

            o = new GameObject();
            o.transform.SetParent(transform, false);
            mr = o.AddComponent<MeshRenderer>();
            mr.material = _material;
            mf = o.AddComponent<MeshFilter>();
            mf.mesh = MeshUtils.CreateBox(.25f, .25f, .25f);
            mc = o.AddComponent<MeshCollider>();
            o.transform.localRotation = Quaternion.FromToRotation(Vector3.up, p_axis);
            o.transform.localPosition = p_axis * SIZE;

            return this;
        }

        protected void Update()
        {
            transform.GetChild(0).localScale = new Vector3(1, 1+delta, 1);
            transform.GetChild(1).localPosition = _axis * (SIZE * (1 + delta));
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
            Debug.Log(delta);
            Vector3 scaleSnap = _parentTransformHandle.scaleSnap;
            Vector3 scale;
            if (_parentTransformHandle.snappingType == HandleSnappingType.RELATIVE)
            {
                scale = _startScale + Vector3.Scale(_startScale, _axis) * delta;
                if (scaleSnap.x != 0) scale.x = Mathf.Round(scale.x / scaleSnap.x) * scaleSnap.x;
                if (scaleSnap.y != 0) scale.y = Mathf.Round(scale.y / scaleSnap.y) * scaleSnap.y;
                if (scaleSnap.z != 0) scale.z = Mathf.Round(scale.z / scaleSnap.z) * scaleSnap.z;
            }
            else
            {
                scale = Vector3.Scale(_startScale, _axis) * delta;
                if (scaleSnap.x != 0) scale.x = Mathf.Round(scale.x / scaleSnap.x) * scaleSnap.x;
                if (scaleSnap.y != 0) scale.y = Mathf.Round(scale.y / scaleSnap.y) * scaleSnap.y;
                if (scaleSnap.z != 0) scale.z = Mathf.Round(scale.z / scaleSnap.z) * scaleSnap.z;
                scale += _startScale;
            }
            
            _parentTransformHandle.target.localScale = scale;
            
            base.Interact(p_previousPosition);
        }

        public override void StartInteraction(Vector3 p_hitPoint)
        {
            base.StartInteraction(p_hitPoint);
            _startScale = _parentTransformHandle.target.localScale;
        }
    }
}