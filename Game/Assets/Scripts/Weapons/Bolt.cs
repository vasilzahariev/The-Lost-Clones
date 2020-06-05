using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bolt : MonoBehaviour
{
    #region Properties

    [HideInInspector]
    public float Speed;

    [HideInInspector]
    public float Damage;

    #endregion

    #region Fields

    private Blaster blaster;

    private float[] deflectionXs;
    private float[] deflectionYs;

    private bool isParied;

    #endregion

    #region MonoMethods

    private void Awake()
    {
        this.blaster = this.gameObject.GetComponentInParent<Blaster>();

        this.deflectionXs = new float[]
        {
            -67.5f,
            -45f,
            -20f,
            20f,
            45f,
            67.5f
        };

        this.deflectionYs = new float[]
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

        Vector3 targetPos = this.blaster.GetWielder().Target.GetComponent<IShootable>().GetShootAt().position;

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
        float speed = this.blaster.BulletSpeed;

        this.Speed = this.isParied ? speed * 2f : speed;
        this.Damage = this.blaster.DamagePerHit;
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
        this.deflectionXs = this.Shuffle<float>(this.deflectionXs);
        this.deflectionYs = this.Shuffle<float>(this.deflectionYs);

        float x = this.GetARandomFromValues(this.deflectionXs);
        float y = this.GetARandomFromValues(this.deflectionYs);
        float z = 0f;

        return new Vector3(x, y, z);
    }

    #endregion

    #region Collision

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != this.blaster &&
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

                if (Time.time - lightsaberController.BlockingStarTime < LightsaberController.TIMETOPARRY)
                {
                    Vector3 wielderPos = this.blaster.GetWielder().GetShootAt().position;

                    Vector3 lookAtPos = new Vector3(wielderPos.x,
                                                    wielderPos.y,
                                                    wielderPos.z);

                    this.transform.LookAt(lookAtPos, Vector3.up);
                    this.isParied = true;
                }
                else
                {
                    rotEulers = this.GetXAndYForDeflection();

                    this.transform.Rotate(rotEulers);
                }
            }
            else
            {
                player.TakeDamage(this.Damage);
                StartCoroutine(this.WaitBeforeDeath(.05f));
            }
        }
    }

    private IEnumerator WaitBeforeDeath(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);

        Destroy(this.gameObject);
    }

    #endregion
}
