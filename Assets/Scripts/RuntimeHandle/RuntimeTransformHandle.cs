using System.Collections.Generic;
using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 21.10.2020
     */
    public class RuntimeTransformHandle : MonoBehaviour
    {
        public static float MOUSE_SENSITIVITY = 1;
        
        public HandleAxes axes = HandleAxes.XYZ;
        public HandleSpace space = HandleSpace.LOCAL;
        public HandleType type = HandleType.POSITION;
        public HandleSnappingType snappingType = HandleSnappingType.RELATIVE;

        public Vector3 positionSnap = Vector3.zero;
        public float rotationSnap = 0;
        public Vector3 scaleSnap = Vector3.zero;

        public bool autoScale = false;
        public float autoScaleFactor = 1;
        public Camera handleCamera;

        private Vector3 _previousMousePosition;
        private HandleBase _previousAxis;
        
        private HandleBase _draggingHandle;

        private HandleType _previousType;
        private HandleAxes _previousAxes;

        private PositionHandle _positionHandle;
        private RotationHandle _rotationHandle;
        private ScaleHandle _scaleHandle;

        public Transform target;

        void Start()
        {
            if (handleCamera == null)
                handleCamera = Camera.main;

            _previousType = type;

            if (target == null)
                target = transform;

            CreateHandles();
        }

        void CreateHandles()
        {
            switch (type)
            {
                case HandleType.POSITION:
                    _positionHandle = gameObject.AddComponent<PositionHandle>().Initialize(this);
                    break;
                case HandleType.ROTATION:
                    _rotationHandle = gameObject.AddComponent<RotationHandle>().Initialize(this);
                    break;
                case HandleType.SCALE:
                    _scaleHandle = gameObject.AddComponent<ScaleHandle>().Initialize(this);
                    break;
            }
        }

        void Clear()
        {
            _draggingHandle = null;
            
            if (_positionHandle) _positionHandle.Destroy();
            if (_rotationHandle) _rotationHandle.Destroy();
            if (_scaleHandle) _scaleHandle.Destroy();
        }

        void Update()
        {
            if (autoScale)
                transform.localScale =
                    Vector3.one * (Vector3.Distance(handleCamera.transform.position, transform.position) * autoScaleFactor) / 15;
            
            if (_previousType != type || _previousAxes != axes)
            {
                Clear();
                CreateHandles();
                _previousType = type;
                _previousAxes = axes;
            }

            HandleBase handle = null;
            Vector3 hitPoint = Vector3.zero;
            GetHandle(ref handle, ref hitPoint);

            HandleOverEffect(handle);

            if (Input.GetMouseButton(0) && _draggingHandle != null)
            {
                _draggingHandle.Interact(_previousMousePosition);
            }

            if (Input.GetMouseButtonDown(0) && handle != null)
            {
                _draggingHandle = handle;
                _draggingHandle.StartInteraction(hitPoint);
            }

            if (Input.GetMouseButtonUp(0) && _draggingHandle != null)
            {
                _draggingHandle.EndInteraction();
                _draggingHandle = null;
            }

            _previousMousePosition = Input.mousePosition;

            transform.position = target.transform.position;
            if (space == HandleSpace.LOCAL || type == HandleType.SCALE)
            {
                transform.rotation = target.transform.rotation;
            }
            else
            {
                transform.rotation = Quaternion.identity;
            }
        }

        void HandleOverEffect(HandleBase p_axis)
        {
            if (_draggingHandle == null && _previousAxis != null && _previousAxis != p_axis)
            {
                _previousAxis.SetDefaultColor();
            }

            if (p_axis != null && _draggingHandle == null)
            {
                p_axis.SetColor(Color.yellow);
            }

            _previousAxis = p_axis;
        }

        private void GetHandle(ref HandleBase p_handle, ref Vector3 p_hitPoint)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            if (hits.Length == 0)
                return;

            foreach (RaycastHit hit in hits)
            {
                p_handle = hit.collider.gameObject.GetComponentInParent<HandleBase>();

                if (p_handle != null)
                {
                    p_hitPoint = hit.point;
                    return;
                }
            }
        }

        static public RuntimeTransformHandle Create(Transform p_target, HandleType p_handleType)
        {
            RuntimeTransformHandle runtimeTransformHandle = new GameObject().AddComponent<RuntimeTransformHandle>();
            runtimeTransformHandle.target = p_target;
            runtimeTransformHandle.type = p_handleType;

            return runtimeTransformHandle;
        }
    }
}