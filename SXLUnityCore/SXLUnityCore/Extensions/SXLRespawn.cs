using UnityEngine;

public static class SXLRespawn
{
    public static void Teleport(this Respawn respawn, Vector3 teleportPosition, Quaternion teleportRotation)
    {
        PlayerController.Instance.skaterController.skaterTargetTransform.position = PlayerController.Instance.skaterController.animBoardTargetTransform.position;
        respawn.puppetMaster.targetRoot.position = teleportPosition;
        respawn.puppetMaster.targetRoot.rotation = teleportRotation;
        respawn.puppetMaster.Teleport(teleportPosition, teleportRotation, true);
    }

    public static void Teleport(this Respawn respawn, Transform teleportXform)
    {
        Teleport(respawn, teleportXform.position, teleportXform.rotation);
    }
}
