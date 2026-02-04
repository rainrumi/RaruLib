using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RaruLib
{
    [RequireComponent(typeof(PlayableDirector))]
    public class CommonTimelineManager : MonoBehaviour
    {
        [SerializeField] PlayableDirector _timeline;
        [SerializeField] float startTime = 0.0f;

        private PlayableDirector timeline => (_timeline != null) ? _timeline : gameObject.GetComponent<PlayableDirector>();

        // ˆê’â~
        public void Pause()
        {
            timeline.Pause();
        }

        // Ä¶ŠJn
        public void PlayStart()
        {
            timeline.time = startTime;
            timeline.Play();
        }

        // ˆê’â~‚©‚ç‚ÌÄŠJ
        public void Resume()
        {
            timeline.Resume();
        }

        // I—¹
        public void Stop()
        {
            timeline.Stop();
        }

        /*
         *
         */

        // ˆê’â~ó‘Ô‚Ìê‡‚ÍÄŠJ
        public void ResumeIfPaused()
        {
            if(timeline.state == PlayState.Paused)
            {
                timeline.Resume();
            }
            else
            {
                Debug.Log("ˆê’â~ó‘Ô‚Å‚Í‚ ‚è‚Ü‚¹‚ñ", gameObject);
            }
        }

    }
}