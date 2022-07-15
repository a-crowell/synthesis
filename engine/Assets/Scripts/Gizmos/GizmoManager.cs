
using UnityEngine;
using Synthesis.ModelManager;
using SynthesisAPI.Utilities;

/// <summary>
/// Manages Gizmos and ensures only one gizmo spawns at a time.
/// </summary>
public static class GizmoManager 
{

    private static GameObject gizmo = null;
    public static GameObject currentGizmo
    {
        get => gizmo;
    }
    /// <summary>
    /// Activates a Gizmo on a given Transform parent object
    /// </summary>
    /// <param name="Gizmo"></param>
    /// <param name="parent"></param>
    /// <param name="forceClose"></param>
    /// <returns></returns>
    public static bool SpawnGizmo(GameObject Gizmo, Transform parent, Vector3 location)
    {
        if (gizmo != null)
                Object.Destroy(gizmo);

        parent.transform.position = location;
        var g = Object.Instantiate(Gizmo,parent);//set transform
        gizmo = g;
        return true;
    }
    /// <summary>
    /// Destroys the Gizmo
    /// </summary>
    public static void ExitGizmo()
    {
        if (gizmo != null && gizmo.transform != null)
        {
            Transform parent = gizmo.transform.parent;
            if (parent != null && parent.CompareTag("gamepiece"))
            {
                PracticeMode.EndConfigureGamepieceSpawnpoint();
            }
        }

        Object.Destroy(gizmo);
    }
    public static void OnEnter()
    {
        if (gizmo.transform.parent.CompareTag("robot"))
        {
            PracticeMode.SetInitialState(gizmo.transform.parent.gameObject);
            Shooting.ConfigureGamepieces();
        } else if (gizmo.transform.parent.CompareTag("gamepiece"))
        {
            PracticeMode.EndConfigureGamepieceSpawnpoint();
        }
        Object.Destroy(gizmo);
    }
}
