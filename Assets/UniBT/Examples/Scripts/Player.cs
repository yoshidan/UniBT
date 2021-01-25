using Cinemachine;
using UnityEngine;

namespace UniBT.Examples.Scripts
{
    public class Player: MonoBehaviour
    {

        [SerializeField]
        private CinemachineFreeLook cameraFreeLook;

        private float forwardSpeed = 0.0f;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }
        
        private void Update()
        {
            var x = Input.GetAxisRaw("Horizontal") ;
            var z = Input.GetAxisRaw("Vertical");
            UpdateRotation(x,z);
            UpdatePosition(x, z);
        }
        
        private void UpdatePosition(float x, float z)
        {
            var moveInput = new Vector2(x, z);
            if (moveInput.sqrMagnitude > 1f) moveInput.Normalize();
            var isMoving = !Mathf.Approximately(moveInput.sqrMagnitude, 0f);

            var desiredForwardSpeed = moveInput.magnitude * 4f;
            var acceleration = isMoving ? 10f : 20f;
            
            // v = at
            forwardSpeed = Mathf.MoveTowards(forwardSpeed, desiredForwardSpeed, acceleration * Time.deltaTime);

            // forward
            var movement = _transform.forward * (forwardSpeed * Time.deltaTime);
            _transform.position += movement;
        }
        
        private void UpdateRotation(float x, float y)
        {
            Vector3 localMovementDirection = new Vector3(x, 0f, y).normalized;
            if (Mathf.Approximately(x, 0) && Mathf.Approximately(y, 0))
            {
                return;
            }

            Vector3 forward = Quaternion.Euler(0f, cameraFreeLook.m_XAxis.Value, 0f) * Vector3.forward;
            forward.y = 0f;
            forward.Normalize();

            Quaternion targetRotation;

            if (Mathf.Approximately(Vector3.Dot(localMovementDirection, Vector3.forward), -1.0f))
            {
                targetRotation = Quaternion.LookRotation(-forward);
            }
            else
            {
                Quaternion cameraToInputOffset = Quaternion.FromToRotation(Vector3.forward, localMovementDirection);
                targetRotation = Quaternion.LookRotation(cameraToInputOffset * forward);
            }

            _transform.rotation = targetRotation;
        }

    }
}