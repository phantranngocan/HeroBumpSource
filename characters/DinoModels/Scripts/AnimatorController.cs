using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

namespace Suriyun
{
    public class AnimatorController : MonoBehaviour
    {

        public Animator[] animators;
        public GameObject[] dinos;
        private GameObject dino = null;
        private int dinoIdx = 0;
        private void Start()
        {
            CreateDino();
        }

        public void SetFloat(string parameter = "key,value")
        {
            char[] separator = { ',', ';' };
            string[] param = parameter.Split(separator);

            string name = param[0];
            float value = (float)Convert.ToDouble(param[1]);

            Debug.Log(name + " " + value);

            foreach (Animator a in animators)
            {
                a.SetFloat(name, value);
            }
        }
        public void SetInt(string parameter = "key,value")
        {
            char[] separator = { ',', ';' };
            string[] param = parameter.Split(separator);

            string name = param[0];
            int value = Convert.ToInt32(param[1]);

            Debug.Log(name + " " + value);

            foreach (Animator a in animators)
            {
                a.SetInteger(name, value);
            }
        }

        public void SetBool(string parameter = "key,value")
        {
            char[] separator = { ',', ';' };
            string[] param = parameter.Split(separator);

            string name = param[0];
            bool value = Convert.ToBoolean(param[1]);

            Debug.Log(name + " " + value);

            foreach (Animator a in animators)
            {
                a.SetBool(name, value);
            }
        }

        public void SetTrigger(string parameter = "key,value")
        {
            char[] separator = { ',', ';' };
            string[] param = parameter.Split(separator);

            string name = param[0];

            Debug.Log(name);

            foreach (Animator a in animators)
            {
                a.SetTrigger(name);
            }
        }

        public void OnNextDino()
        {
            dinoIdx++;
            if (dinoIdx >= dinos.Length)
            {
                dinoIdx = 0;
            }
            Destroy(dino);
            NetworkMgr.inst.dinoid = dinoIdx;
            CreateDino();
        }
        public void OnPreviousDino()
        {
            dinoIdx--;
            if (dinoIdx < 0)
            {
                dinoIdx = dinos.Length - 1;
            }
            Destroy(dino);
            NetworkMgr.inst.dinoid = dinoIdx;
            CreateDino();
        }
        public void OnHome()
        {
            SceneManager.LoadScene("home");
        }
        private void CreateDino(){
            dino = Instantiate(dinos[NetworkMgr.inst.dinoid]);
            dino.transform.position = new Vector3(0, 0, 0);
            gameObject.GetComponent<MouseOrbitImproved1>().target = dino.transform;
        }
        
    }
}