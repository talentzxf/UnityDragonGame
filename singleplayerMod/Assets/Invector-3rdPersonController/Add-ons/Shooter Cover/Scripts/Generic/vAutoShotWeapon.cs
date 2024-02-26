using Invector.vShooter;

public class vAutoShotWeapon : vShooterWeaponBase
{

    protected virtual void Update()
    {
        if (this.enabled) Shoot();
    }
}
