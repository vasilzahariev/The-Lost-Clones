using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bolt : MonoBehaviour
{
    #region Properties
    public float Speed { get; set; }

    public float Damage { get; private set; }

    #endregion

    #region Fields

    private Blaster _blaster;

    private float[] _deflectionXs;
    private float[] _deflectionYs;

    private bool _isParied;

    #endregion

    #region MonoMethods

    private void Awake()
    {
        this._blaster = this.gameObject.GetComponentInParent<Blaster>();

        this._deflectionXs = new float[]
        {
            -67.5f,
            -45f,
            -20f,
            20f,
            45f,
            67.5f
        };

        this._deflectionYs = new float[]
        {
            90f,
            135f,
            150f,
            180f,
            210f,
            225f,
            270f
        };

        this.transform.rotation = this.gameObject.transform.parent.rotation;

        this.gameObject.transform.parent = null;

        Vector3 targetPos = this._blaster.GetWielder().Target.GetComponent<IShootable>().GetShootAt().position;

        Vector3 lookAtPos = new Vector3(targetPos.x,
                                        targetPos.y,
                                        targetPos.z);

        this.transform.LookAt(lookAtPos, Vector3.up);
    }

    private void Start()
    {
        StartCoroutine(DestroyIfItHasntCollidedAfterSeconds(5f));
    }

    private void Update()
    {
        float speed = this._blaster.BulletSpeed;

        this.Speed = this._isParied ? speed * 2f : speed;
        this.Damage = this._blaster.DamagePerHit;
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

    private IEnumerator DestroyIfItHasntCollidedAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);

        Destroy(this.gameObject);
    }

    private T[] Shuffle<T>(T[] arr)
    {
        int size = arr.Length;

        for (int index = 0; index < size; index++)
        {
            int newIndex = Random.Range(0, size - 1);

            T current = arr[index];
            T other = arr[newIndex];

            arr[newIndex] = current;
            arr[index] = other;
        }

        return arr;
    }

    private float GetARandomFromValues(float[] values)
    {
        int index = Random.Range(0, values.Length - 1);

        return values[index];
    }

    private Vector3 GetXAndYForDeflection()
    {
        this._deflectionXs = this.Shuffle<float>(this._deflectionXs);
        this._deflectionYs = this.Shuffle<float>(this._deflectionYs);

        float x = this.GetARandomFromValues(this._deflectionXs);
        float y = this.GetARandomFromValues(this._deflectionYs);
        float z = 0f;

        return new Vector3(x, y, z);
    }

    #endregion

    #region Collision

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != this._blaster &&
            !other.gameObject.CompareTag("Player") &&
            other.gameObject.GetComponent<Bolt>() == null)
            StartCoroutine(this.WaitBeforeDeath(0f));

        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            LightsaberController lightsaberController = player.GetLightsaberController();

            if (lightsaberController.IsBlocking)
            {
                Vector3 rotEulers;

                if (Time.time - lightsaberController.BlockingStarTime < LightsaberController.TIME_TO_PARRY)
                {
                    Vector3 wielderPos = this._blaster.GetWielder().GetShootAt().position;

                    Vector3 lookAtPos = new Vector3(wielderPos.x,
                                                    wielderPos.y,
                                                    wielderPos.z);

                    this.transform.LookAt(lookAtPos, Vector3.up);
                    this._isParied = true;
                }
                else
                {
                    rotEulers = this.GetXAndYForDeflection();

                    this.transform.Rotate(rotEulers);
                }
            }
            else
            {
                player.TakeDamage(this.Damage, _blaster.GetWielder().gameObject);
                StartCoroutine(this.WaitBeforeDeath(.05f));
            }
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            enemy.TakeDamage(this.Damage, _blaster.GetWielder().gameObject);

            StartCoroutine(this.WaitBeforeDeath(.0f));
        }
    }

    private IEnumerator WaitBeforeDeath(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);

        Destroy(this.gameObject);
    }

    #endregion
}
