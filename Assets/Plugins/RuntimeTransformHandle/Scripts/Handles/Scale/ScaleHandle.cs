using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 20.10.2020
     */
    public class ScaleHandle : MonoBehaviour
    {
        protected RuntimeTransformHandle _parentTransformHandle;
        protected List<ScaleAxis> _axes;
        protected ScaleGlobal _globalAxis;
        
        public ScaleHandle Initialize(RuntimeTransformHandle p_parentTransformHandle)
        {
            _parentTransformHandle = p_parentTransformHandle;
            transform.SetParent(_parentTransformHandle.transform, false);

            _axes = new List<ScaleAxis>();
            
            if (_parentTransformHandle.axes == HandleAxes.X || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<ScaleAxis>()
                    .Initialize(_parentTransformHandle, Vector3.right, Color.red));
            
            if (_parentTransformHandle.axes == HandleAxes.Y || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<ScaleAxis>()
                    .Initialize(_parentTransformHandle, Vector3.up, Color.green));

            if (_parentTransformHandle.axes == HandleAxes.Z || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<ScaleAxis>()
                    .Initialize(_parentTransformHandle, Vector3.forward, Color.blue));

            if (_parentTransformHandle.axes != HandleAxes.X && _parentTransformHandle.axes != HandleAxes.Y && _parentTransformHandle.axes != HandleAxes.Z)
            {
                _globalAxis = new GameObject().AddComponent<ScaleGlobal>()
                    .Initialize(_parentTransformHandle, HandleBase.GetVectorFromAxes(_parentTransformHandle.axes), Color.white);
                
                _globalAxis.InteractionStart += OnGlobalInteractionStart;
                _globalAxis.InteractionUpdate += OnGlobalInteractionUpdate;
                _globalAxis.InteractionEnd += OnGlobalInteractionEnd;
            }

            return this;
        }

        void OnGlobalInteractionStart()
        {
            foreach (ScaleAxis axis in _axes)
            {
                axis.SetColor(Color.yellow);
            }
        }

        void OnGlobalInteractionUpdate(float p_delta)
        {
            foreach (ScaleAxis axis in _axes)
            {
                axis.delta = p_delta;
            }
        }

        void OnGlobalInteractionEnd()
        {
            foreach (ScaleAxis axis in _axes)
            {
                axis.SetDefaultColor();
                axis.delta = 0;
            }
        }

        public void Destroy()
        {
            foreach (ScaleAxis axis in _axes)
                Destroy(axis.gameObject);

            if (_globalAxis) Destroy(_globalAxis.gameObject);
            
            Destroy(this);
        }
    }
}