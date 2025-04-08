using UnityEngine;
//using System.Collections;

public class AttackInput : MonoBehaviour
{
    public GameSetting gameSetting;

    private void Awake()
    {
        //StartCoroutine(LateAwake());
    }

    private void OnEnable()
    {
        //StartCoroutine(LateOnEnable());
    }

    private void Start()
    {
        //StartCoroutine(LateStart());
    }


    //public IEnumerator LateAwake()
    //{
    //    if (Input.GetKeyDown(gameSetting.attack))
    //    {
    //        AttackScript.Instance.AppendAttack();
    //    }

    //    yield return new WaitForSeconds(0.1f);

    //    if (Input.GetKeyDown(gameSetting.attack))
    //    {
    //        AttackScript.Instance.AppendAttack();
    //    }
    //}

    //public IEnumerator LateOnEnable()
    //{
    //    if (Input.GetKeyDown(gameSetting.attack))
    //    {
    //        AttackScript.Instance.AppendAttack();
    //    }

    //    yield return new WaitForSeconds(0.1f);

    //    if (Input.GetKeyDown(gameSetting.attack))
    //    {
    //        AttackScript.Instance.AppendAttack();
    //    }
    //}

    //public IEnumerator LateStart()
    //{
    //    if (Input.GetKeyDown(gameSetting.attack))
    //    {
    //        AttackScript.Instance.AppendAttack();
    //    }

    //    yield return new WaitForSeconds(0.1f);

    //    if (Input.GetKeyDown(gameSetting.attack))
    //    {
    //        AttackScript.Instance.AppendAttack();
    //    }
    //}


    void Update()
    {
        if (Input.GetKeyDown(gameSetting.attack))
        {
            AttackScript.Instance.AppendAttack();
        }
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(gameSetting.attack))
        {
            AttackScript.Instance.AppendAttack();
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(gameSetting.attack))
        {
            AttackScript.Instance.AppendAttack();
        }
    }
}
