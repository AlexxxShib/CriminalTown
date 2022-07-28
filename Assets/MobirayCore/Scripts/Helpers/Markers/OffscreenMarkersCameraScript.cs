﻿using System;
using System.Collections.Generic;
 using Mobiray.Common;
 using Mobiray.Helpers;
 using UnityEngine;

namespace Mobiray.Markers
{
    public enum NavIcon
    {
        NONE,
        END_POINT,
        CARGO_POINT,
        ENEMY_POINT,
    }

    public class OffscreenMarkersCameraScript : MonoBehaviour
    {
        public Texture IconNavPoint;

        [Space] 
        public float IconsRelateiveSize = 15;
        public float DistanceToMiles = 0.05f;
        
        [Space] 
        public float RelativeMarginTop;
        public float RelativeMarginBottom;
        public float RelativeMarginLeftRight;
        public float MinDistanceVisible = 20;
        
        [Space]
        public Font TextFont;
        public float RelativeFontSize;

        private void Awake()
        {
            ToolBox.Add(this);
        }

        public Camera _camera;
        private List<OffscreenMarker> _trackedObjects = new List<OffscreenMarker>();

        // private SettingsGame settings;
        private Transform playerEntity;

        private GUIStyle textStyle;

        public void Register(OffscreenMarker marker, Transform helicopter)
        {
            if (!_trackedObjects.Contains(marker))
            {
                _trackedObjects.Add(marker);

                if (!marker.ArrowPoint)
                {
                    var arrowPoint = new GameObject($"ArrowPoint:{marker.gameObject.name}");
                    arrowPoint.transform.parent = helicopter.transform;
                    arrowPoint.transform.localPosition = new Vector3(0, 0, 0);
                    
                    arrowPoint.AddComponent<LookAtScript>().SetPoint(marker.transform);

                    marker.ArrowPoint = arrowPoint.transform;
                }
            }
            else
            {
                Debug.LogWarning("EntTrackerLib: The tracked objects list already contains " + marker, marker);
            }

            textStyle = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = {textColor = Color.white},
                font = TextFont,
                fontSize = (int) (_camera.pixelRect.height / RelativeFontSize),
                // fontStyle = FontStyle.Bold
            };
        }

        public void ClearMarkers()
        {
            _trackedObjects.Clear();
        }

        private void Start()
        {
            // settings = ToolBox.Get<SettingsGame>();
            // playerEntity = ToolBox.Get<GameController>().Character.transform;
        }

        private bool IsVisible(GameObject go, out float distance)
        {
            distance = (go.transform.position - playerEntity.transform.position).magnitude;
            return distance < MinDistanceVisible;
        }

        private Vector3 FixBehindCamera(Vector3 p)
        {
            var transform1 = _camera.transform;
            Vector3 d = p - transform1.position;
            Vector3 n = transform1.forward;
            float ds = Vector3.Dot(d, n);

            if (ds > 0)
            {
                return p;
            }

            // make sure that we are never on the camera plane
            if (ds == 0)
            {
                return p + n * 0.00001f;
            }

            return p - 2 * ds * n;
        }

        void OnGUI()
        {
            if (!Event.current.type.Equals(EventType.Repaint))
            {
                return;
            }

            Rect camRect = _camera.pixelRect;
            Vector2 iconSize = new Vector2(camRect.height / IconsRelateiveSize, camRect.height / IconsRelateiveSize);
            Vector2 iconExt = iconSize / 2;
            
            // Debug.Log($"iconExt: {iconExt}");

            var marginTop = camRect.height / RelativeMarginTop;
            var marginBottom = camRect.height / RelativeMarginBottom;
            var marginLeftRight = camRect.width / RelativeMarginLeftRight;
            
            // Debug.Log($"markers : {_trackedObjects.Count}");
            
            // GUI.DrawTexture(new Rect(100, 100, 100, 100), IconHome);

            for (int i = _trackedObjects.Count - 1; i >= 0; i--)
            {
                OffscreenMarker marker = _trackedObjects[i];
                if (!marker || !marker.gameObject)
                {
                    _trackedObjects.RemoveAt(i);
                    continue;
                }

                float distance = 1;

                if (marker.Navigation == NavIcon.NONE || IsVisible(marker.gameObject, out distance))
                {
                    continue;
                }

                Vector3 wp = FixBehindCamera(marker.transform.position);
                // Debug.Log($"wp: {wp}");
                Vector2 markerPos = _camera.WorldToScreenPoint(wp);
                // Debug.Log($"mrkScrPos: {mrkScrPos}");
                markerPos.y = camRect.height - markerPos.y;
                // Debug.Log($"mrkScrPos: {mrkScrPos}");

                // var iconPosX = Mathf.Clamp(mrkScrPos.x, iconExt.x + marginLeftRight,
                    // camRect.width - iconExt.x - marginLeftRight);
                
                // Debug.Log($"iconPosX: {iconPosX}");
                
                var minPos = new Vector2(iconExt.x + marginLeftRight, iconExt.y + marginTop);
                var maxPos = new Vector2(camRect.width - iconExt.x - marginLeftRight, 
                    camRect.height - iconExt.y - marginBottom);

                if (minPos.x < markerPos.x && markerPos.x < maxPos.x &&
                    minPos.y < markerPos.y && markerPos.y < maxPos.y)
                {
                    continue;
                }

                var iconPos = new Vector2(
                    Mathf.Clamp(markerPos.x, minPos.x, maxPos.x), 
                    Mathf.Clamp(markerPos.y, minPos.y, maxPos.y));
                
                var iconRect = new Rect(
                    iconPos.x - iconExt.x, iconPos.y - iconExt.y, 
                    iconSize.x, iconSize.y);
                
                var iconCenter = new Vector2(
                    iconRect.xMin + iconRect.width * 0.5f, 
                    iconRect.yMin + iconRect.height * 0.5f);

                var arrowPointAngle = marker.ArrowPoint.rotation.eulerAngles;

                var sign = Mathf.Sign(arrowPointAngle.y > 180 ? 
                    arrowPointAngle.y - 360 : arrowPointAngle.y);
                
                var iconAngle = sign * arrowPointAngle.x + 90;
                
                if (sign < 0)
                {
                    iconAngle += 180;
                }
                
                // Debug.Log($"arrow point: {arrowPointAngle} sign:{sign} icon angle: {iconAngle}");
                
                Rect labelRect = iconRect;
                labelRect.y += iconSize.y;
                
                switch (marker.Navigation)
                {
                    case NavIcon.END_POINT:
                        Matrix4x4 matrixBackup = GUI.matrix;
                        
                        GUIUtility.RotateAroundPivot(iconAngle, iconCenter);
                        GUI.DrawTexture(iconRect, IconNavPoint);
                        
                        GUI.matrix = matrixBackup;
                        
                        GUI.Label(labelRect, GetLabelText(distance), textStyle);
                        break;
                    
                    /*case NavIcon.NEXT:
                        GUI.DrawTexture(ri, IconIsland);
                        GUI.Label(labelrect, GetLabelText(distance), textStyle);
                        break;
                    case NavIcon.BONUS:
                        GUI.DrawTexture(ri, IconBonus);
                        GUI.Label(labelrect, GetLabelText(distance), textStyle);
                        break;*/
                }
            }
        }

        private string GetLabelText(float distance)
        {
            int intDistance = (int) (distance * DistanceToMiles);

            if (intDistance <= 1)
            {
                // return "one mile";
                return string.Empty;
            }
            
            return intDistance + " miles";
        }
    }
}