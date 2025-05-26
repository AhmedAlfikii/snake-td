using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;

public class CinematicsController : MonoBehaviour
{
    public void StartCinematicScene(string sceneName)
    {
        Cinematics.StartCinematicScene(sceneName);
    }
    public void StartCinematicSceneInstant(string sceneName)
    {
        Cinematics.StartCinematicSceneInstant(sceneName);
    }
    
    public void EndCinematicScene(string sceneName)
    {
        Cinematics.EndCinematicScene(sceneName);
    }
    public void EndCinematicSceneInstant(string sceneName)
    {
        Cinematics.EndCinematicSceneInstant(sceneName);
    }
}
