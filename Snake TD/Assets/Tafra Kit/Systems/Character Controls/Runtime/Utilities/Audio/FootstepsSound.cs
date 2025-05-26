using UnityEngine;

namespace TafraKit.CharacterControls
{
    public class FootstepsSound : MonoBehaviour
    {
        [SerializeField] private SFXClips footstepSFX;

        private int leftFootstepSFXID;
        private int rightFootstepSFXID;

        public void LeftFootstepContact()
        {
            //TODO: while this prevents the multiple movement animations playing in the blend tree from playing multiple sounds at once...
            //...it's not the ideal option as if the footsteps are faster than the audio, then it will skip beats.
            if(SFXPlayer.IsPlaying(leftFootstepSFXID)) return;

            leftFootstepSFXID = SFXPlayer.Play(footstepSFX);
        }
        public void RightFootstepContact()
        {
            //TODO: while this prevents the multiple movement animations playing in the blend tree from playing multiple sounds at once...
            //...it's not the ideal option as if the footsteps are faster than the audio, then it will skip beats.
            if(SFXPlayer.IsPlaying(rightFootstepSFXID)) return;

            rightFootstepSFXID = SFXPlayer.Play(footstepSFX);
        }
    }
}