using UnityEngine;
using System.Collections;

public class B1DroidEvenHandler : MonoBehaviour
{
    #region Fields

    private B1Droid _b1;
    private E5 _blaster;

    #endregion

    #region Mono Methods

    private void Awake()
    {
        _b1 = this.gameObject.GetComponent<B1Droid>();
        _blaster = (E5) _b1.Weapon;
    }

    #endregion

    #region Death

    public void LetEnemyDie()
    {
        _b1.CanDie = true;
    }

    #region Stealth Death

    public void ChangeCollider()
    {
        Rigidbody rg = _b1.gameObject.GetComponent<Rigidbody>();

        rg.AddForce(_b1.transform.forward * rg.mass * 10f, ForceMode.Impulse);
        rg.AddForce(_b1.transform.up * rg.mass * 1.5f, ForceMode.Impulse);

        StartCoroutine(this.ChangeColliderValuesAfterTime());
    }

    private IEnumerator ChangeColliderValuesAfterTime()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        CapsuleCollider collider = _b1.gameObject.GetComponent<CapsuleCollider>();
        Rigidbody rg = _b1.GetComponent<Rigidbody>();

        Vector3 newCenter = new Vector3(collider.center.x,
                                        0.275f,
                                        collider.center.z);

        collider.direction = 2;
        //collider.radius /= 5f;
        collider.center = newCenter;

        rg.mass = 1f;
    }

    #endregion

    #endregion
}
