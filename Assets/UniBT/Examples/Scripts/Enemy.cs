using UnityEngine;
using UnityEngine.AI;

namespace UniBT.Examples.Scripts
{
    public class Enemy : MonoBehaviour
    {
        public float Hate { get; private set; }

        public bool Attacking { get; private set; }

        [SerializeField]
        private Transform player;
        
        private Rigidbody rigid;
        
        private NavMeshAgent navMeshAgent;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            var distance = Vector3.Distance(transform.position, player.position);
            if (distance < 8)
            {
                Hate = 20;
            } else if (distance < 12)
            {
                Hate = 8;
            }
            else
            {
                Hate = 0;
            }
        }

        public void Attack(float force)
        {
            Attacking = true;
            navMeshAgent.enabled = false;
            rigid.isKinematic = false;
            rigid.AddForce(Vector3.up * force, ForceMode.Impulse);
        }

        private void OnCollisionStay(Collision other)
        {
            // TODO other.collider.name cause GC.Alloc by Object.GetName
            if (Attacking && other.collider.name == "Ground" && Mathf.Abs(rigid.velocity.y) < 0.1)
            {
                CancelAttack();
            }
        }

        public void CancelAttack()
        {
            navMeshAgent.enabled = true;
            rigid.isKinematic = true;
            Attacking = false;
        }
        
    }
}