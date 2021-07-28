using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Funinja : Enemy
{
    public void NothingPersonnel()
    {
        transform.position = AnguraController.Instance.transform.position - lookDirection + Vector3.up;
    }

    public void TeleSound()
    {
        AudioManager.Instance.PlayOneShot("Instant Transmission");
    }
}
