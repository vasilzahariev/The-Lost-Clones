using UnityEngine;
using System.Collections;

public class Bolt : MonoBehaviour
{
    #region Properties

    [HideInInspector]
    public float Speed;

    #endregion

    #region Fields

    private Blaster blaster;

    #endregion

    #region MonoMethods

    private void Awake()
    {
        this.blaster = this.gameObject.GetComponentInParent<Blaster>();

        this.transform.rotation = this.gameObject.transform.parent.rotation;

        this.gameObject.transform.parent = null;

        Vector3 targetPos = this.blaster.GetWielder().Target.transform.position;

        Vector3 lookAtPos = new Vector3(targetPos.x,
                                        targetPos.y + this.transform.position.y,
                                        targetPos.z);

        this.transform.LookAt(lookAtPos, Vector3.up);
    }

    private void Update()
    {
        this.Speed = this.blaster.BulletSpeed;
    }

    private void FixedUpdate()
    {
        this.Move();
    }

    #endregion

    #region Methods

    private void Move()
    {
        this.transform.Translate(Vector3.forward * this.Speed * Time.fixedDeltaTime);
    }

    #endregion

    #region Collision

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != this.blaster &&
            !other.gameObject.CompareTag("Player"))
            StartCoroutine(this.WaitBeforeDeath(0f));

        if (other.gameObject.CompareTag("Player"))
            StartCoroutine(this.WaitBeforeDeath(.05f));
    }

    private IEnumerator WaitBeforeDeath(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);

        Destroy(this.gameObject);
    }

    #endregion
}
