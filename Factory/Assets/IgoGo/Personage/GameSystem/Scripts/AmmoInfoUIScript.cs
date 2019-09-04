using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class AmmoInfoUIScript : MonoBehaviour
{
    public Text cerrentAmmoText;
    public Text allAmmoText;

    public Image shootDelayTimer;
    public Image reloadDelayTimer;

    private float shootTimer = 0;
    private float shootTimerMax = 1;

    private float reloadTimer = 0;
    private float reloadTimerMax = 1;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void ImageCurrentAmmo(int current)
    {
        cerrentAmmoText.text = current.ToString();
    }

    public void ImageAllAmmo(int all)
    {
        allAmmoText.text = all.ToString();
    }

    public void ShootDelay(float timer)
    {
        shootTimerMax = timer;
        this.shootTimer = 0;
        shootDelayTimer.fillAmount = 0;
    }
    public void ReloadDelay(float timer)
    {
        reloadTimerMax = timer;
        this.reloadTimer = 0;
        reloadDelayTimer.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (shootTimer < shootTimerMax)
        {
            shootTimer += Time.deltaTime;
            shootDelayTimer.fillAmount = shootTimer / shootTimerMax;
        }
        if (reloadTimer < reloadTimerMax)
        {
            reloadTimer += Time.deltaTime;
            reloadDelayTimer.fillAmount = reloadTimer / reloadTimerMax;
        }
    }


}
