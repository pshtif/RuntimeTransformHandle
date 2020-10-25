using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 20.10.2020
     */
    public class PositionPlane : HandleBase
    {
        protected Vector3 _startPosition;
        protected Vector3 _axis;
        protected Vector3 _perp;
        protected Plane _plane;
        protected Vector3 _interactionOffset;
        
        public PositionPlane Initialize(RuntimeTransformHandle p_runtimeHandle, Vector3 p_axis, Vector3 p_perp, Color p_color)
        {
            _parentTransformHandle = p_runtimeHandle;
            _defaultColor = p_color;
            _axis = p_axis;
            _perp = p_perp;

            InitializeMaterial();

            transform.SetParent(p_runtimeHandle.transform, false);

            GameObject o = new GameObject();
            o.transform.SetParent(transform, false);
            MeshRenderer mr = o.AddComponent<MeshRenderer>();
            mr.material = _material;
            MeshFilter mf = o.AddComponent<MeshFilter>();
            mf.mesh = MeshUtils.CreateBox(.02f, .5f, 0.5f);
            MeshCollider mc = o.AddComponent<MeshCollider>();
            o.transform.localRotation = Quaternion.FromToRotation(Vector3.up, _perp);
            o.transform.localPosition = _axis * .25f;

            return this;
        }

        public override void Interact(Vector3 p_previousPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float d = 0.0f;
            _plane.Raycast(ray, out d);
            
            Vector3 hitPoint = ray.GetPoint(d);

            Vector3 offset = hitPoint + _interactionOffset - _startPosition;
            Vector3 snapping = _parentTransformHandle.positionSnap;
            float snap = Vector3.Scale(snapping, _axis).magnitude;
            if (snap != 0 && _parentTransformHandle.snappingType == HandleSnappingType.RELATIVE)
            {
                if (snapping.x != 0) offset.x = Mathf.Round(offset.x / snapping.x) * snapping.x;
                if (snapping.y != 0) offset.y = Mathf.Round(offset.y / snapping.y) * snapping.y;
                if (snapping.z != 0) offset.z = Mathf.Round(offset.z / snapping.z) * snapping.z;
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

        public override void StartInteraction()
        {
            Vector3 rperp = _parentTransformHandle.space == HandleSpace.LOCAL
                ? _parentTransformHandle.target.rotation * _perp
                : _perp;
            
            _plane = new Plane(rperp, _parentTransformHandle.target.position);
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float d = 0.0f;
            _plane.Raycast(ray, out d);
            
            Vector3 hitPoint = ray.GetPoint(d);
            _startPosition = _parentTransformHandle.target.position;
            _interactionOffset = _startPosition - hitPoint;
        }
    }
}