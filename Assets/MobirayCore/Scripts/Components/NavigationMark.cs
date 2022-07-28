using UnityEngine;

namespace MobirayCore.Scripts.Components
{
    
    [RequireComponent(typeof(RectTransform))]
    public class NavigationMark : MonoBehaviour
    {
        public Transform target;
        public GameObject view;
        public bool isRotating;

        private RectTransform _rectTransform; 
        private RectTransform _canvasTransform;

        private Camera _camera;

        private bool _hasTarget;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasTransform = _rectTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

            _rectTransform.pivot = isRotating ? new Vector2(1, .5f) : new Vector2(.5f, .5f);
            
            _camera = Camera.main;
            
            SetTarget(target);
        }

        private void Update()
        {
            if (_hasTarget)
            {
                NavigateTo(target.position);
            }
        }

        public void SetTarget(Transform target)
        {
            if (target)
            {
                this.target = target;
                _hasTarget = true;
            }
        }

        public void NavigateTo(Vector3 position)
        {
            var localInCameraPosition = _camera.transform.InverseTransformPoint(position);

            view.SetActive(!IsInViewport(_camera.WorldToViewportPoint(position)));

            var screenPosition = _camera.WorldToScreenPoint(position);

            if (localInCameraPosition.z <= 0)
            {
                screenPosition = InvertScreenPosition(screenPosition);
            }

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTransform,
                    screenPosition, null, out var localPosition))
            {
                ClampInRect(localPosition, _canvasTransform.rect);
                if (isRotating)
                {
                    _rectTransform.localEulerAngles = Vector3.forward *
                                                      Mathf.Atan2(localPosition.y, localPosition.x) * Mathf.Rad2Deg;
                }
            }
        }

        private Vector3 InvertScreenPosition(Vector3 screen)
        {
            return new Vector3(Screen.width - screen.x, Screen.height - screen.y, screen.z);
        }

        private void ClampInRect(Vector3 point, Rect rect)
        {
            if (!rect.Contains(point))
            {
                var xBound = point.x > rect.xMax ? rect.xMax : rect.xMin;
                var yBound = point.y > rect.yMax ? rect.yMax : rect.yMin;
                
                if (!rect.Contains(Vector3.right * point.x))
                {
                    point = new Vector3(xBound, point.y * xBound / point.x, point.z);
                }

                if (!rect.Contains(Vector3.up * point.y))
                {
                    point = new Vector3(point.x * yBound / point.y, yBound, point.z);
                }
            }

            if (!isRotating)
            {
                _rectTransform.pivot =
                    new Vector2((point.x - rect.xMin) / rect.width, (point.y - rect.yMin) / rect.height);
            }

            _rectTransform.localPosition = point;
        }

        private static bool IsInViewport(Vector3 viewportPosition)
        {
            return IsIn01(viewportPosition.x) && IsIn01(viewportPosition.y);
        }

        private static bool IsIn01(float a)
        {
            return a >= 0 && a <= 1;
        }
    }
}