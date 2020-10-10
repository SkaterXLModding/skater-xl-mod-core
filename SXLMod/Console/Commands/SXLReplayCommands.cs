using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SXLMod.Console
{
    class SXLReplayCommands
    {
        private static List<Tuple<Vector3, Quaternion, float>> m_replayCameraBookmarks = new List<Tuple<Vector3, Quaternion, float>>();

        [RegisterCommand(Name = "replay_bookmark", Help = "Sets A Bookmark for camera position and FOV. Must Be in FREE CAM mode.", Hint = "replay_bookmark <list>|<add>|<go> <int index>|<remove> <int index>|<clear>", ArgMin = 1, ArgMax = 2)]
        static void CommandReplayBookmark(CommandArg[] args)
        {
            Type currentState = GameManagement.GameStateMachine.Instance.CurrentState.GetType();
            if (currentState != typeof(GameManagement.ReplayState))
            {
                Debug.LogWarning("Can not set camera bookmark outside of replay mode.");
                return;
            }

            string command = args[0].String.ToLower();
            ReplayEditor.CameraMode mode = ReplayEditor.ReplayEditorController.Instance.cameraController.mode;

            switch (args.Count())
            {
                case 1:
                    if (command == "list")
                    {
                        for(int i=0; i < SXLReplayCommands.m_replayCameraBookmarks.Count(); i++)
                        {
                            Tuple<Vector3, Quaternion, float> item = SXLReplayCommands.m_replayCameraBookmarks.ElementAt(i);
                            Debug.Log($"{i}: Transform: {item.Item1} | Rotation: {item.Item2} | FOV: {item.Item3}");
                        }
                    }
                    else if (command == "add")
                    {
                        if (mode == ReplayEditor.CameraMode.Free)
                        {
                            Camera cam = Camera.main;
                            SXLReplayCommands.m_replayCameraBookmarks.Add(new Tuple<Vector3, Quaternion, float>(cam.gameObject.transform.position, cam.gameObject.transform.rotation, cam.fieldOfView));
                            Debug.Log($"Set Camera Bookmark at Index {SXLReplayCommands.m_replayCameraBookmarks.Count() - 1}");
                        }
                        else
                        {
                            Debug.LogWarning("Camera is not in FREE mode.");
                        }
                    }
                    else if (command == "clear")
                    {
                        SXLReplayCommands.m_replayCameraBookmarks.Clear();
                    }
                    break;
                case 2:
                    int index = args[1].Int;
                    if (command == "go")
                    {
                        if (mode == ReplayEditor.CameraMode.Free)
                        {
                            Camera cam = Camera.main;

                            try
                            {
                                Tuple<Vector3, Quaternion, float> currentIdx = SXLReplayCommands.m_replayCameraBookmarks.ElementAt(index);
                                cam.gameObject.transform.position = currentIdx.Item1;
                                cam.gameObject.transform.rotation = currentIdx.Item2;
                                cam.fieldOfView = currentIdx.Item3;
                                Debug.Log($"Set Camera to bookmark at index {index}.");
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                Debug.LogWarning("Bookmark at index {index} is out of range or does not exist.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Camera is not in FREE mode.");
                        }
                    }
                    else if (command == "remove")
                    {
                        try
                        {
                            SXLReplayCommands.m_replayCameraBookmarks.RemoveAt(index);
                            Debug.Log($"Removed Camera bookmark at index {index}.");
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            Debug.LogWarning("Bookmark at index {index} is out of range or does not exist.");
                        }

                    }
                    break;
            }
        }
    }
}
