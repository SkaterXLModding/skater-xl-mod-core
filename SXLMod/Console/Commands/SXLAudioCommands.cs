using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SXLMod.Console
{
    class SXLAudioCommands
    {
        [RegisterCommand(Name = "audio_distance", Help = "Set all audio sample min/max distances", Hint = "audio_distance <float min> <float max>", ArgMin = 2, ArgMax = 2)]
        static void CommandAudioDistance(CommandArg[] args)
        {
            List<AudioSource> audioSources = GameObject.FindObjectsOfType<AudioSource>().ToList();
            foreach(AudioSource source in audioSources)
            {
                source.minDistance = args[0].Float;
                source.maxDistance = args[1].Float;
            }
        }

        [RegisterCommand(Name = "audio_doppler_level", Help = "Set the doppler level value for all audio samples", Hint = "audio_doppler_factor <float>", ArgMin = 1, ArgMax = 1)]
        static void CommandAudioDopplerLevel(CommandArg[] args)
        {
            List<AudioSource> audioSources = GameObject.FindObjectsOfType<AudioSource>().ToList();
            foreach (AudioSource source in audioSources)
            {
                source.dopplerLevel = args[0].Float;
            }
        }
    }
}
