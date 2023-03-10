using UnityEngine;
using BoschingMachine.Bipeal;

namespace BoschingMachine.Bipedal
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class Biped : MonoBehaviour
    {
        [SerializeField] BipedalMovement movement;
        [SerializeField] Transform head;

        public Rigidbody Rigidbody { get; private set; }
        public BipedalMovement Movement => movement;

        public virtual Vector3 MoveDirection { get; }
        public virtual bool Jump { get; }
        public virtual Vector2 LookRotation { get; }
        public Transform Head => head;

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void OnEnable ()
        {

        }

        protected virtual void OnDisable ()
        {

        }

        protected virtual void Start ()
        {

        }

        protected virtual void FixedUpdate ()
        {
            Movement.Move(Rigidbody, MoveDirection, Jump);
        }

        protected virtual void Update ()
        {
            Movement.Look(Rigidbody, head, LookRotation);
        }

        protected virtual void LateUpdate ()
        {

        }
    }
}
