using Cinemachine;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera standardCam;
    public CinemachineVirtualCamera combatCam;
    public CinemachineVirtualCamera zoomedCombatCam;

    public void SetStandard()
    {
        standardCam.Priority = 10;
        combatCam.Priority = 5;
        zoomedCombatCam.Priority = 5;
    }

    public void SetCombat(bool zoomed)
    {
        standardCam.Priority = 5;
        combatCam.Priority = 5;
        zoomedCombatCam.Priority = 5;
        
        if (zoomed)
        {
            zoomedCombatCam.Priority = 10;
        }
        else
        {
            combatCam.Priority = 10;
        }
        
        
    }
}
